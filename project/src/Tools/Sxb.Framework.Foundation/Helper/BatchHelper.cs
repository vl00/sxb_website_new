using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Framework.Foundation.Helper
{

    public class BatchHelper
    {
        public class BatchBuilder<T>
        {
            Batch<T> batch;
            public BatchBuilder(int capacity = 10000, int batchSize = 2000)
            {
                batch = new Batch<T>(capacity, batchSize);
            }

            public BatchBuilder<T> From(Func<int, Task<IEnumerable<T>>> getEntitiesFunc)
            {
                batch.GetEntitiesFunc = getEntitiesFunc;
                return this;
            }

            public BatchBuilder<T> From(IEnumerable<T> entities)
            {
                batch.GetEntitiesFunc = pageIndex =>
                    pageIndex == 1 ? Task.FromResult(entities) : Task.FromResult(Enumerable.Empty<T>());

                return this;
            }

            public BatchBuilder<T> Convert(Func<T, T> converter)
            {
                batch.ConvertAction = converter;
                return this;
            }

            public BatchBuilder<T> Handle(Func<IEnumerable<T>, Task> handle)
            {
                batch.HandleAction = handle;
                return this;
            }

            public Batch<T> Build()
            {
                return batch;
            }
        }

        public class TimeWatch
        {
            protected Stopwatch Stopwatch { get; }
            string filename = $"./time-watch-{DateTime.Today:yyyyMMdd}.txt";
            int count = 0;
            public TimeWatch()
            {
                Stopwatch = Stopwatch.StartNew();
                Stopwatch.Reset();
            }

            public void Before()
            {
                count++;
                Stopwatch.Restart();
                WriteLine(string.Format("第{0}次执行开始", count));
            }

            public void After()
            {
                Stopwatch.Stop();
                WriteLine(string.Format("第{0}次执行结束,time(s)={1}", count, Stopwatch.Elapsed.TotalSeconds));
            }

            public void WriteLine(string msg)
            {
                using (StreamWriter stream = File.AppendText(filename))
                {
                    stream.WriteLine(msg);
                }
            }
        }

        public class Batch<T> : TimeWatch
        {
            public bool IsFinishGetEntities { get; private set; }

            public bool IsNoMoreData => IsFinishGetEntities && Quque.Count == 0;

            public int BatchSize { get; private set; } = 2000;

            public int Capacity { get; }

            public ConcurrentQueue<T> Quque { get; private set; }

            public int PageIndex { get; private set; } = 1;

            public Batch(int capacity, int batchSize)
            {
                Quque = new ConcurrentQueue<T>();
                Capacity = capacity;
                BatchSize = batchSize;
            }

            public Func<int, Task<IEnumerable<T>>> GetEntitiesFunc { get; set; }
            public Func<IEnumerable<T>, Task> HandleAction { get; set; }
            public Func<T, T> ConvertAction { get; set; }

            public void Run()
            {
                RunAsync().GetAwaiter().GetResult();
            }

            public async Task RunAsync()
            {
                if (Capacity == 0)
                {
                    throw new InvalidOperationException(nameof(Capacity));
                }
                if (BatchSize == 0)
                {
                    throw new InvalidOperationException(nameof(BatchSize));
                }

                try
                {
                    var task1 = GetEntities();
                    await Task.WhenAll(task1, HandleAsync());
                }
                catch (AggregateException ex)
                {
                    System.Diagnostics.Debug.WriteLine("main ex:{0}", ex.Message);
                    foreach (var item in ex.InnerExceptions)
                    {
                        System.Diagnostics.Debug.WriteLine(item.Message);
                    }
                    throw ex;
                }
            }


            public async Task GetEntities()
            {
                IEnumerable<T> entities = Enumerable.Empty<T>();
                do
                {
                    foreach (var item in entities)
                    {
                        //超出等待
                        while (Quque.Count >= Capacity)
                            await Task.Delay(200);

                        Quque.Enqueue(item);
                    }

                    Before();
                    try
                    {
                        entities = await GetEntitiesFunc.Invoke(PageIndex);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("GetEntities:{0}", ex.Message);
                    }
                    After();

                    PageIndex++;
                } while (entities != null && entities.Any());

                IsFinishGetEntities = true;
            }

            public async Task HandleAsync()
            {
                try
                {
                    List<T> items = new List<T>(BatchSize);
                    while (!IsNoMoreData)
                    {
                        //等待获取新数据
                        while (!IsFinishGetEntities && Quque.Count == 0)
                            await Task.Delay(200);

                        while (Quque.Count != 0 && items.Count < BatchSize)
                        {
                            if (Quque.TryDequeue(out T t))
                            {
                                if (ConvertAction == null
                                    || (t = ConvertAction.Invoke(t)) != null)
                                {
                                    items.Add(t);
                                }
                            }
                            else
                                await Task.Delay(20);
                        }

                        if (items != null && items.Count != 0)
                        {
                            await HandleAction.Invoke(items);
                            items.Clear();
                        }
                        else
                        {
                            await Task.Delay(200);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("HandleAsync:{0}", ex.Message);
                    throw ex;
                }
            }
        }
    }
}

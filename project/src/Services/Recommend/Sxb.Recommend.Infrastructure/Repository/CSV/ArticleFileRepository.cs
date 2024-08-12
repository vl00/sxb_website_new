using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Utilities;
using Sxb.Infrastructure.Core.Extensions;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Enum;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.Repository.CSV
{
    public class ArticleFileRepository : IArticleFileRepository
    {
        private const string cacheKey = "articles";
        private List<(Article, EntityChangeType)> datas = new List<(Article, EntityChangeType)>();

        ILogger<ArticleFileRepository> _logger;
        IMemoryCache _memoryCache;
        IMediator _mediator;
        private readonly static object _filelock = new object();
        string _dataPath;
        string _logPath;

        public ArticleFileRepository(IOptions<ArticleCSVOption> options
            , ILogger<ArticleFileRepository> logger
            , IMediator mediator
            , IMemoryCache memoryCache = null)
        {
            this._dataPath = options.Value.DataPath;
            this._logPath = options.Value.LogPath;
            _logger = logger;
            _mediator = mediator;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 初始化仓储(初始化基础数据，不考虑过多的东西)
        /// </summary>
        public void Init(IEnumerable<Article> articles)
        {
            lock (_filelock)
            {
                if (File.Exists(_dataPath))
                {
                    File.Delete(_dataPath);
                }
                File.Create(_dataPath).Close();
                int counter = 0;
                DateTime time = DateTime.Now;
                using (StreamWriter sw = new StreamWriter(_dataPath, true, Encoding.UTF8))
                {
                    foreach (var article in articles)
                    {
                        sw.WriteLine(article.ToCSV());
                        counter++;
                    }

                }
                WriteLog(time, $"初始化仓储{counter}");

            }
        }


        public void Append(Article article)
        {

            lock (_filelock)
            {
                if (datas.Any(s => s.Item1.Id == article.Id && s.Item2 == EntityChangeType.Add) == false)
                {
                    datas.Add((article, EntityChangeType.Add));
                }
                else
                {
                    _logger.LogDebug($"{article.Id} 已存在。跳过Append操作。");
                }

            }


        }


        public void Update(Article article)
        {
            lock (_filelock)
            {
                var index = datas.FindIndex(s => s.Item1.Id == article.Id);
                if (index > 0)
                {
                    datas[index] = (article, EntityChangeType.Modifiy);
                }
                else
                {
                    datas.Add((article, EntityChangeType.Modifiy));
                }
            }

        }


        public void SaveChange()
        {
            lock (_filelock)
            {
                if (!File.Exists(_dataPath))
                {
                    throw new Exception($"找不到:{_dataPath}");
                }
                var lines = File.ReadAllLines(_dataPath).ToList();
                var effectDatas = new List<Article>();
                foreach (var data in datas)
                {
                    if (data.Item2 == EntityChangeType.Add)
                    {
                        var index = lines.FindIndex(s => new Article(s).Id == data.Item1.Id);
                        if (index < 0)
                        {
                            lines.Add(data.Item1.ToCSV());
                            effectDatas.Add(data.Item1);
                        }

                        continue;
                    }
                    if (data.Item2 == EntityChangeType.Modifiy)
                    {
                        var index = lines.FindIndex(s => new Article(s).Id == data.Item1.Id);
                        if (index > 0)
                        {
                            lines[index] = data.Item1.ToCSV();
                            effectDatas.Add(data.Item1);
                        }

                        continue;
                    }

                }
                File.WriteAllLines(_dataPath, lines);
                var articlesCache = _memoryCache.Get<List<Article>>(cacheKey);
                if (articlesCache != null)
                {
                    foreach (var item in effectDatas)
                    {
                        var findIndex = articlesCache.FindIndex(a => a.Id == item.Id);
                        if (findIndex > 0)
                        {
                            articlesCache[findIndex] = item;
                        }
                        else
                        {
                            articlesCache.Add(item);
                        }
                    }
                    _memoryCache.Set(cacheKey, articlesCache, new MemoryCacheEntryOptions().SetSize(articlesCache.Count));

                }

                WriteLog(DateTime.Now, $"保存成功，本次操作条数{datas.Count}");
                _mediator.DispatchDomainEventsAsync(datas.Select(d => d.Item1)).GetAwaiter().GetResult();
                datas.Clear();


            }

        }

        public IEnumerable<Article> Query(Func<Article, bool> where)
        {
            var articles = _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                var articles = Read().ToList();
                entry.SetSize(articles.Count);
                return articles;

            });
            return articles.Where(where);
        }


        void WriteLog(DateTime time, string remark)
        {
            using (var sw = new StreamWriter(this._logPath, true, Encoding.UTF8))
            {
                sw.WriteLine(string.Format("{0},{1}", time, remark));
            }
        }


        IEnumerable<Article> Read()
        {
            if (!File.Exists(_dataPath))
            {
                throw new Exception($"找不到文件.Paht={_dataPath}");
            }
            using (StreamReader sr = new StreamReader(_dataPath, Encoding.UTF8))
            {
                string line;
                while ((line = (sr.ReadLine())) != null)
                {
                    var article = new Article(line);
                    yield return article;
                }

            }

        }

        public bool Exists()
        {
            return File.Exists(_dataPath);
        }
    }
}

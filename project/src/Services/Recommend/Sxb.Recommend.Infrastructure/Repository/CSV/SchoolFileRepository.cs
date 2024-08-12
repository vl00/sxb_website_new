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
    public class SchoolFileRepository : ISchoolFileRepository
    {

        private const string cacheKey = "schools";
        private List<(School, EntityChangeType)> datas = new List<(School, EntityChangeType)>();

        ILogger<SchoolFileRepository> _logger;
        IMemoryCache _memoryCache;
        IMediator _mediator;
        private readonly static object _filelock = new object();
        string _dataPath;
        string _logPath;

        public SchoolFileRepository(IOptions<SchoolCSVOption> options
            , ILogger<SchoolFileRepository> logger
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
        public void Init(IEnumerable<School> schools)
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
                    foreach (var school in schools)
                    {
                        sw.WriteLine(school.ToCSV());
                        counter++;
                    }

                }
                WriteLog(time, $"初始化仓储{counter}");

            }
        }


        public void Append(School school)
        {

            lock (_filelock)
            {
                if (datas.Any(s => s.Item1.Id == school.Id && s.Item2 == EntityChangeType.Add) == false)
                {
                    datas.Add((school, EntityChangeType.Add));
                }
                else
                {
                    _logger.LogDebug($"{school.Id} 已存在。跳过Append操作。");
                }

            }


        }


        public void Update(School school)
        {
            lock (_filelock)
            {
                var index = datas.FindIndex(s => s.Item1.Id == school.Id);
                if (index > 0)
                {
                    datas[index] = (school, EntityChangeType.Modifiy);
                }
                else
                {
                    datas.Add((school, EntityChangeType.Modifiy));
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
                var effectDatas = new List<School>();
                foreach (var data in datas)
                {
                    if (data.Item2 == EntityChangeType.Add)
                    {
                        var index = lines.FindIndex(s => new School(s).Id == data.Item1.Id);
                        if (index < 0)
                        {
                            lines.Add(data.Item1.ToCSV());
                            effectDatas.Add(data.Item1);
                        }

                        continue;
                    }
                    if (data.Item2 == EntityChangeType.Modifiy)
                    {
                        var index = lines.FindIndex(s => new School(s).Id == data.Item1.Id);
                        if (index > 0)
                        {
                            lines[index] = data.Item1.ToCSV();
                            effectDatas.Add(data.Item1);
                        }

                        continue;
                    }

                }
                File.WriteAllLines(_dataPath, lines);
                var schoolsCache = _memoryCache.Get<List<School>>(cacheKey);
                if (schoolsCache != null)
                {
                    foreach (var item in effectDatas)
                    {
                        var findIndex = schoolsCache.FindIndex(a => a.Id == item.Id);
                        if (findIndex > 0)
                        {
                            schoolsCache[findIndex] = item;
                        }
                        else
                        {
                            schoolsCache.Add(item);
                        }
                    }
                    _memoryCache.Set(cacheKey, schoolsCache,new MemoryCacheEntryOptions().SetSize(schoolsCache.Count));

                }
                WriteLog(DateTime.Now, $"保存成功，本次操作条数{datas.Count}");
                _mediator.DispatchDomainEventsAsync(datas.Select(d => d.Item1)).GetAwaiter().GetResult();
                datas.Clear();


            }

        }

        public IEnumerable<School> Query(Func<School, bool> where)
        {
            var schools = _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                var schools = Read().ToList();
                entry.SetSize(schools.Count);
                return schools;
            });
            return schools.Where(where);
        }


        void WriteLog(DateTime time, string remark)
        {
            using (var sw = new StreamWriter(this._logPath, true, Encoding.UTF8))
            {
                sw.WriteLine(string.Format("{0},{1}", time, remark));
            }
        }


        IEnumerable<School> Read()
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
                    var school = new School(line);
                    yield return school;
                }

            }

        }

        public bool Exists()
        {
            return File.Exists(_dataPath);
        }
    }
}

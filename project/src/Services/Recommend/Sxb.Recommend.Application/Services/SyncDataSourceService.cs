using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public class SyncDataSourceService : ISyncDataSourceService
    {

        IHostEnvironment _hostEnvironment;
        ILogger<SyncDataSourceService> _logger;
        ISchoolFileRepository _schoolFileRepository;
        IArticleFileRepository _articleFileRepository;
        ISchoolRepository _schoolRepository;
        IArticleRepository _articleRepository;
        IRecommendService _recommendService;
        ISchoolMapRepository _schoolMapRepository;
        IArticleMapRepository _articleMapRepository;
        ISchoolMapService _schoolMapService;
        IArticleMapService _articleMapService;
        private static readonly object _syncArticleLock = new object();
        private static readonly object _syncSchoolLock = new object();

        public SyncDataSourceService(IHostEnvironment hostEnvironment
            , ILogger<SyncDataSourceService> logger
            , ISchoolFileRepository schoolFileRepository
            , IArticleFileRepository articleFileRepository
            , IArticleRepository articleRepository
            , ISchoolRepository schoolRepository
            , IRecommendService recommendService
            , ISchoolMapRepository schoolMapRepository
            , IArticleMapRepository articleMapRepository
            , ISchoolMapService schoolMapService
            , IArticleMapService articleMapService)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _schoolFileRepository = schoolFileRepository;
            _articleFileRepository = articleFileRepository;
            _articleRepository = articleRepository;
            _schoolRepository = schoolRepository;
            _recommendService = recommendService;
            _schoolMapRepository = schoolMapRepository;
            _articleMapRepository = articleMapRepository;
            _schoolMapService = schoolMapService;
            _articleMapService = articleMapService;
        }

        /// <summary>
        /// 同步学校副本，只会同步到当天。
        /// </summary>
        /// <returns></returns>
        public void SyncSchoolsCopies()
        {
            string timelogFilePath = Path.Combine("Data", $"schools_synclog_{_hostEnvironment.EnvironmentName}.log");
            if (!Directory.Exists(Path.Combine("Data")))
            {
                Directory.CreateDirectory("Data");
            }
            SyncSchoolsCopies(timelogFilePath);
        }


        /// <summary>
        /// 同步学校副本，只会同步到当天。
        /// </summary>
        /// <returns></returns>
        public void SyncSchoolsCopies(string syncLogFilePath)
        {
            lock (_syncSchoolLock)
            {
                _logger.LogDebug("开始同步学校数据源。");
                Task.Run(async () =>
                {
                    int offset = 0, limit = 100;
                    DateTime updateTime = DateTime.Now;
                    if (_schoolFileRepository.Exists())
                    {
                        DateTime preUpdateTime = await GetPreSyncTime(syncLogFilePath) ?? SqlDateTime.MinValue.Value;
                        List<School> schools;
                        int addCounter = 0, updateCounter = 0;
                        List<School> upsertSchools = new List<School>();
                        while ((schools = (await _schoolRepository.GetValidAfterAsync(preUpdateTime, offset, limit)).ToList())?.Any() == true)
                        {
                            foreach (var school in schools)
                            {
                                var existsSchool = _schoolFileRepository.Query(s => s.Id == school.Id).FirstOrDefault();
                                if (existsSchool != null)
                                {
                                    //更新
                                    _schoolFileRepository.Update(school);
                                    updateCounter++;
                                }
                                else
                                {
                                    //新增
                                    _schoolFileRepository.Append(school);
                                    addCounter++;
                                }
                            }
                            _schoolFileRepository.SaveChange();
                            upsertSchools.AddRange(schools);
                            if (schools.Count < limit)
                            {
                                break;
                            }
                            offset += limit;
                        }
                        List<Guid> schoolIds = new List<Guid>();
                        foreach (var school in upsertSchools)
                        {
                            var schoolmapsfromdb = await _schoolMapRepository.GetSchoolMaps(school);
                            var schoolmapsfromgenerate = await _schoolMapService.UpsertSchoolMaps(school);
                            schoolIds.AddRange(schoolmapsfromdb.Select(s => s.SIdP));
                            schoolIds.AddRange(schoolmapsfromgenerate.Select(s => s.SIdS));

                        }
                        if (schoolIds.Any())
                        {
                            await _schoolMapService.UpsertSchoolMaps(schoolIds.Distinct());
                        }
                        await WriteSyncLog(updateTime, $"本次新增:{addCounter}条；更新{updateCounter}条。", syncLogFilePath);


                    }
                    else
                    {
                        //初始化数据源
                        _schoolFileRepository.Init(GetAllSchools());
                        await WriteSyncLog(updateTime, "首次导入学校。", syncLogFilePath);
                    }

                }).GetAwaiter().GetResult();
                _logger.LogDebug("同步结束。");
            }
        }



        public void SyncArticleCopies()
        {
            string synclogFilePath = Path.Combine("Data", $"articles_synclog_{_hostEnvironment.EnvironmentName}.log");

            if (!Directory.Exists(Path.Combine("Data")))
            {
                Directory.CreateDirectory("Data");
            }
            SyncArticleCopies(synclogFilePath);
        }

        public void SyncArticleCopies(string syncLogFilePath)
        {
            lock (_syncArticleLock)
            {
                _logger.LogDebug("开始同步文章数据源。");
                Task.Run(async () =>
                {
                    int offset = 0, limit = 10000;
                    DateTime updateTime = DateTime.Now;
                    if (_articleFileRepository.Exists())
                    {
                        //更新
                        DateTime preUpdateTime = await GetPreSyncTime(syncLogFilePath) ?? SqlDateTime.MinValue.Value;
                        List<Article> articles;
                        int addCounter = 0, updateCounter = 0;
                        List<Article> upsertArticles = new List<Article>();
                        while ((articles = (await _articleRepository.GetArticles(preUpdateTime, offset, limit)).ToList())?.Any() == true)
                        {
                            foreach (var article in articles)
                            {
                                var existsArticle = _articleFileRepository.Query(s => s.Id == article.Id).FirstOrDefault();
                                if (existsArticle != null)
                                {
                                    //更新
                                    _articleFileRepository.Update(article);
                                    updateCounter++;
                                }
                                else
                                {
                                    //新增
                                    _articleFileRepository.Append(article);
                                    addCounter++;
                                }
                            }
                            _articleFileRepository.SaveChange();
                            upsertArticles.AddRange(articles);
                            if (articles.Count < limit)
                            {
                                break;
                            }
                            offset += limit;
                        }
                        List<Guid> articleIds = new List<Guid>();
                        foreach (var article in upsertArticles)
                        {
                            var articlemapsfromdb = await _articleMapRepository.GetArticleMaps(article);
                            var articlemapsfromgenerate = await _articleMapService.UpsertArticelMaps(article);
                            articleIds.AddRange(articlemapsfromdb.Select(s => s.AIdP));
                            articleIds.AddRange(articlemapsfromgenerate.Select(s => s.AIdS));
                        }
                        if (articleIds.Any())
                        {
                            await _articleMapService.UpsertArticelMaps(articleIds.Distinct());
                        }
                        await WriteSyncLog(updateTime, $"本次新增:{addCounter}条；更新{updateCounter}条。", syncLogFilePath);

                    }
                    else
                    {
                        //初始化数据源
                        _articleFileRepository.Init(GetAllArticles());
                        await WriteSyncLog(updateTime, "首次导入学校。", syncLogFilePath);

                    }

                }).GetAwaiter().GetResult();
                _logger.LogDebug("同步结束。");
            }

        }



        IEnumerable<School> GetAllSchools()
        {
            int offset = 0, limit = 100000;
            IEnumerable<School> schools;
            while ((schools = _schoolRepository.GetValidAfterAsync(SqlDateTime.MinValue.Value, offset, limit).GetAwaiter().GetResult())?.Any() == true)
            {
                foreach (var school in schools)
                {
                    _logger.LogDebug($"正在同步{school.Id}");
                    yield return school;
                }
                offset += limit;
            }

        }


        IEnumerable<Article> GetAllArticles()
        {
            int offset = 0, limit = 100000;
            IEnumerable<Article> articles;
            while ((articles = _articleRepository.GetArticles(SqlDateTime.MinValue.Value, offset, limit).GetAwaiter().GetResult())?.Any() == true)
            {
                foreach (var article in articles)
                {
                    _logger.LogDebug($"正在同步{article.Id}");
                    yield return article;
                }
                offset += limit;
            }

        }

        /// <summary>
        /// 记录时间日志
        /// </summary>
        /// <param name="time"></param>
        /// <param name="remark"></param>
        /// <param name="filepath"></param>
        async Task WriteSyncLog(DateTime time, string remark, string filepath)
        {
            using (var sw = new StreamWriter(filepath, true, Encoding.UTF8))
            {
                await sw.WriteLineAsync(string.Format("{0},{1}", time, remark));
            }
        }

        /// <summary>
        /// 获取上次同步日期
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        async Task<DateTime?> GetPreSyncTime(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            var lines = await File.ReadAllLinesAsync(filePath);
            string endLine = lines.LastOrDefault();
            if (!string.IsNullOrEmpty(endLine))
            {
                var timelog = endLine.Split(",");
                return DateTime.Parse(timelog[0]);
            }
            else
            {
                return null;
            }
        }






    }
}

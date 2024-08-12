using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Value;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sxb.Infrastructure.Core.Extensions;
using Sxb.Recommend.Application.MapFeatureComputeRules;
using Sxb.Recommend.Domain.Enum;

namespace Sxb.Recommend.Application.Services
{
    public class RecommendService : IRecommendService
    {

        IMapFeatureRepository _mapFeatureRepository;
        IServiceProvider _serviceProvider;
        ILogger<RecommendService> _logger;
        IHostEnvironment _hostEnvironment;
        ISchoolMapRepository _schoolMapRepository;
        IArticleMapRepository _articleMapRepository;
        ISchoolFileRepository _schoolFileRepository;
        IArticleFileRepository _articleFileRepository;
        IComputeScoreService _computeScoreService;


        public RecommendService(IMapFeatureRepository mapFeatureRepository
            , IServiceProvider serviceProvider
            , ILogger<RecommendService> logger
            , IHostEnvironment hostEnvironment
            , ISchoolMapRepository schoolMapRepository
            , IArticleRepository articleRepository
            , ISchoolFileRepository schoolFileRepository
            , IArticleFileRepository articleFileRepository
            , IArticleMapRepository articleMapRepository
            , IComputeScoreService computeScoreService)
        {
            _mapFeatureRepository = mapFeatureRepository;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _schoolMapRepository = schoolMapRepository;
            _schoolFileRepository = schoolFileRepository;
            _articleFileRepository = articleFileRepository;
            _articleMapRepository = articleMapRepository;
            _computeScoreService = computeScoreService;
        }


        public async Task<IEnumerable<Guid>> GetRecommendArticles(Guid aid, int offset, int limit)
        {
            IEnumerable<ArticleMap> articleMaps = await _articleMapRepository.GetArticleMaps(aid, offset, limit);
            return articleMaps.Select(s=>s.AIdS);
        }



        public async Task<IEnumerable<ArticleMapValue>> GetRecommendArticles(Article article, int offset, int limit)
        {
            bool HasArticleMaps = await _articleMapRepository.HasArticleMaps(article);
            IEnumerable<ArticleMap> articleMaps;
            if (!HasArticleMaps)
            {
                articleMaps = await _computeScoreService.GetArticleMaps(article);
                await _articleMapRepository.InsertManyAsync(articleMaps);
                articleMaps = articleMaps.Skip(offset).Take(limit);
            }
            else
            {
                articleMaps = await _articleMapRepository.GetArticleMaps(article, offset, limit);
            }

            var articleIds = articleMaps.Select(s => s.AIdS).ToList();
            var effectArticles = _articleFileRepository.Query(s => articleIds.Any(schoolId => schoolId == s.Id)).ToList();

            return articleMaps.Select(s =>
            {
                var articleP = article;
                var articleS = effectArticles.FirstOrDefault(article => article.Id == s.AIdS);
                return new ArticleMapValue()
                {
                    ArticleP = articleP,
                    ArticleS = articleS,
                    Remark = s.Remark,
                    Score = s.Score
                };
            });
        }


        public async Task<IEnumerable<SchoolScore>> GetRecommendSchools(SchoolFilterDefinition filterDefinition)
        {
          return await  _computeScoreService.GetSchoolScoresByFilter(filterDefinition);
        }

        public async Task<IEnumerable<Guid>> GetRecommendSchools(Guid eid, int offset, int limit)
        {
            IEnumerable<SchoolMap> schoolMaps = await _schoolMapRepository.GetSchoolMaps(eid, offset, limit);
            return schoolMaps.Select(s => s.SIdS);
        }





        public async Task<IEnumerable<SchoolMapValue>> GetRecommendSchools(School school, int offset, int limit)
        {
            bool HasSchoolMaps = await _schoolMapRepository.HasSchoolMaps(school);
            IEnumerable<SchoolMap> schoolMaps;
            if (!HasSchoolMaps)
            {
                schoolMaps = await _computeScoreService.GetSchoolMaps(school);
                await _schoolMapRepository.InsertManyAsync(schoolMaps);
                schoolMaps = schoolMaps.Skip(offset).Take(limit);
            }
            else
            {
                schoolMaps = await _schoolMapRepository.GetSchoolMaps(school, offset, limit);
            }

            var schoolIds = schoolMaps.Select(s => s.SIdS).ToList();
            var effectSchools = _schoolFileRepository.Query(s => schoolIds.Any(schoolId => schoolId == s.Id)).ToList();

            return schoolMaps.Select(s =>
            {
                var schoolP = school;
                var schoolS = effectSchools.FirstOrDefault(school => school.Id == s.SIdS);
                return new SchoolMapValue()
                {
                    SchoolP = schoolP,
                    SchoolS = schoolS,
                    Remark = s.Remark,
                    Score = s.Score
                };
            });
        }





        public IEnumerable<School> QuerySchools(Func<School, bool> where)
        {
            if (_schoolFileRepository.Exists())
            {
                return _schoolFileRepository.Query(where);
            }
            else {
                return Enumerable.Empty<School>();
            }

        }
        public IEnumerable<Article> QueryArticles(Func<Article, bool> where)
        {
            if (_articleFileRepository.Exists())
            {
                return _articleFileRepository.Query(where);
            }
            else {
                return Enumerable.Empty<Article>();
            }
        }

        

        /// <summary>
        /// 保存学校匹配结果
        /// 先将生成结果缓存到本地文件（避免中断丢失）
        /// 然后将结果插入到mongodb
        /// </summary>
        /// <returns></returns>
        public async Task InsertSchoolMaps()
        {
            var schools = _schoolFileRepository.Query(s => 1 == 1);
            int skip = 0, take = 10000;
            int total = schools.Count();
            int takeCount = (total + take - 1) / take;
            int[] skips = new int[takeCount];
            for (int i = 0; i < takeCount; i++)
            {
                skips[i] = skip;
                skip += take;
            }
            string path = Path.Combine("Data", $"schoolmaps_{_hostEnvironment.EnvironmentName}.csv");
            string schoolMapDonepath = Path.Combine("Data", "SchoolMapDone.txt");
            if (!Directory.Exists(Path.Combine("Data")))
            {
                Directory.CreateDirectory("Data");
            }
            using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8))
            {
                foreach (var _skip in skips)
                {
                    //List<Task> tasks = new List<Task>();
                    var smallBatch = schools.Skip(_skip).Take(take);
                    foreach (var school in smallBatch)
                    {
                        if (ReadSchoolMapDone(schoolMapDonepath).Any(s => s == school.Id))
                        {
                            _logger.LogDebug($"跳过{school.Id}");
                            continue;
                        }
                        //tasks.Add(Task.Run(async () =>
                        //{

                        var schoolmaps = await ExcuteTime(async () =>
                        {
                            return await _computeScoreService.GetSchoolMaps(school);
                        });
                        foreach (var schoolmap in schoolmaps)
                        {
                            string row = schoolmap.ToCSV();
                            await sw.WriteLineAsync(row);
                        }
                        WriteSchoolMapDone(school, schoolMapDonepath);
                        //}));
                        //if (tasks.Count == Environment.ProcessorCount)
                        //{
                        //    foreach (var task in tasks)
                        //    {
                        //        await task;
                        //    }
                        //    tasks.Clear();
                        //}
                    }

                }


            }

            _logger.LogDebug($"本次插入结果总数：{ReadLocalSchoolMaps(path).Count()}");
            int resultTotal = ReadLocalSchoolMaps(path).Count();
            foreach (var schoolMap in ReadLocalSchoolMaps(path))
            {
                _logger.LogDebug($"当前正在插入：{schoolMap.ToCSV()}");
                await _schoolMapRepository.InsertAsync(schoolMap);
                _logger.LogDebug($"剩余:{ --resultTotal}条");
            }
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            if (File.Exists(schoolMapDonepath))
            {
                File.Delete(schoolMapDonepath);
            }

        }


        void WriteSchoolMapDone(School school, string path)
        {

            using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8))
            {
                sw.WriteLine(school.Id);
            }
        }

        IEnumerable<Guid> ReadSchoolMapDone(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
                {
                    string line;
                    while ((line = (sr.ReadLine())) != null)
                    {
                        yield return Guid.Parse(line);
                    }

                }
            }
        }

        async Task<T> ExcuteTime<T>(Func<Task<T>> func)
        {
            var now1 = DateTime.Now;
            var result = await func();
            var now2 = DateTime.Now;
            _logger.LogDebug($"总耗时:{(now2 - now1).TotalMilliseconds}毫秒");
            return result;
        }
        IEnumerable<SchoolMap> ReadLocalSchoolMaps(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
                {
                    string line;
                    while ((line = (sr.ReadLine())) != null)
                    {
                        yield return new SchoolMap(line);
                    }

                }
            }


        }







    }

}

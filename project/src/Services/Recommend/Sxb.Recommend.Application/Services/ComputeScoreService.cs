using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sxb.Recommend.Application.MapFeatureComputeRules;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Value;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public class ComputeScoreService : IComputeScoreService
    {
        ILogger<ComputeScoreService> _logger;
        IMapFeatureRepository _mapFeatureRepository;
        ISchoolFileRepository _schoolFileRepository;
        IArticleFileRepository _articleFileRepository;
        IServiceProvider _serviceProvider;
        IMemoryCache _memoryCache;
        public ComputeScoreService(ILogger<ComputeScoreService> logger
            , IMapFeatureRepository mapFeatureRepository
            , ISchoolFileRepository schoolFileRepository
            , IArticleFileRepository articleFileRepository
            , IServiceProvider serviceProvider
            , IMemoryCache memoryCache)
        {
            _logger = logger;
            _mapFeatureRepository = mapFeatureRepository;
            _schoolFileRepository = schoolFileRepository;
            _articleFileRepository = articleFileRepository;
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<SchoolMap>> GetSchoolMaps(School school)
        {

            var features = await _mapFeatureRepository.GetMapFeaturesAsync(1);
            int top = 100;
            List<SchoolMap> top100 = new List<SchoolMap>();
            var computeSchools = _schoolFileRepository.Query(s => s.IsOnline && s.City == school.City && s.Id != school.Id);
            var validCount = computeSchools.Count();
            int offset = 0;
            int limit = 100000;
            int pageCount = (validCount + limit - 1) / limit;
            int[] offsets = new int[pageCount];
            for (int i = 0; i < offsets.Length; i++)
            {
                offsets[i] = offset;
                offset += limit;
            }
            int susccessCounter = 0;
            object lockobj = new object();
            _logger.LogDebug($"本次处理总数:{validCount}");
            List<Task> tasks = new List<Task>();
            foreach (var _offset in offsets)
            {
                Task task = Task.Run(async () =>
                {
                    List<SchoolMap> top100temp = new List<SchoolMap>();
                    _logger.LogDebug($"Task[{Task.CurrentId}]；学校：{school.Id}；当前Offset：{_offset}");
                    var validSchools = computeSchools.Skip(_offset).Take(limit);
                    _logger.LogDebug($"Task[{Task.CurrentId}]Done.本次处理条数：{validSchools.Count()}");
                    foreach (var validSchool in validSchools)
                    {
                        List<string> addScoreRemarks = new List<string>();
                        double totalScore = 0;
                        //特征匹配计分
                        foreach (var feature in features)
                        {
                            Type computeRuleType = SearchReflectType($"Sxb.Recommend.Application.MapFeatureComputeRules.{feature.ComputeRuleName}");
                            if (computeRuleType == null)
                            {
                                throw new ArgumentNullException($"找不到该特性对应的特性规则，[{feature.ComputeRuleName}]");
                            }
                            ISchoolComputeRule schoolComputeRule = _serviceProvider.GetService(computeRuleType) as ISchoolComputeRule;
                            if (schoolComputeRule == null)
                            {
                                throw new ArgumentNullException($"无法将[{computeRuleType.FullName}]转换为[ISchoolComputeRule]。");
                            }
                            double score = await schoolComputeRule.Compute(school, validSchool, feature);
                            totalScore += score;
                            addScoreRemarks.Add($"{feature.Alias}加{score}分");
                        }
                        if (top100temp.Count < top)
                        {
                            //如果没有找到比当前分数更大的，直接进列表。
                            top100temp.Add(new SchoolMap(
                             school.Id,
                             validSchool.Id,
                             totalScore,
                             string.Join(";", addScoreRemarks)
                            ));
                        }
                        else{
                            var minItemIndex = top100temp.FindIndex(s => s.Score <= totalScore);
                            if (minItemIndex >= 0)
                            {
                                top100temp[minItemIndex] = new SchoolMap(
                                 school.Id,
                                 validSchool.Id,
                                 totalScore,
                                 string.Join(";", addScoreRemarks)
                                );
                            }
                        }

                    }
                    lock (lockobj)
                    {
                        _logger.LogDebug("正在处理当前任务结果。");
                        if (top100.Any() == true)
                        {
                            var temp = top100temp.Where(s => s.Score >= top100.Min(s1 => s1.Score) && s.Score <= top100.Max(s1 => s1.Score)).ToList();
                            top100.AddRange(temp);
                            top100 = top100.OrderByDescending(s => s.Score).Skip(0).Take(top).ToList();
                        }
                        else {
                            top100.AddRange(top100temp);
                        }

                        susccessCounter++;
                    }
                });
                tasks.Add(task);
            }
            foreach (var task in tasks)
            {
                await task;
            }
            return top100;
        }
        public async Task<IEnumerable<ArticleMap>> GetArticleMaps(Article article)
        {
            var features = await _mapFeatureRepository.GetMapFeaturesAsync(2);
            int top = 100;
            var computeArticles = _articleFileRepository.Query(a => a.IsOnline && a.Id != article.Id);
            List<ArticleMap> top100 = new List<ArticleMap>();
            var validCount = computeArticles.Count();
            int offset = 0;
            int limit = 100000;
            int pageCount = (validCount + limit - 1) / limit;
            int[] offsets = new int[pageCount];
            for (int i = 0; i < offsets.Length; i++)
            {
                offsets[i] = offset;
                offset += limit;
            }
            int susccessCounter = 0;
            object lockobj = new object();
            _logger.LogDebug($"本次处理总数:{validCount}");
            List<Task> tasks = new List<Task>();
            foreach (var _offset in offsets)
            {
                Task task = Task.Run(async () =>
                {
                    List<ArticleMap> top100temp = new List<ArticleMap>();
                    _logger.LogDebug($"Task[{Task.CurrentId}]；文章：{article.Id}；当前Offset：{_offset}");
                    var articleSs = computeArticles.Skip(_offset).Take(limit);
                    _logger.LogDebug($"Task[{Task.CurrentId}]Done.本次处理条数：{articleSs.Count()}");
                    foreach (var articleS in articleSs)
                    {
                        List<string> addScoreRemarks = new List<string>();
                        double totalScore = 0;
                        //特征匹配计分
                        foreach (var feature in features)
                        {
                            Type computeRuleType = SearchReflectType($"Sxb.Recommend.Application.MapFeatureComputeRules.{feature.ComputeRuleName}");
                            if (computeRuleType == null)
                            {
                                throw new ArgumentNullException($"找不到该特性对应的特性规则，[{feature.ComputeRuleName}]");
                            }
                            IArticleComputeRule schoolComputeRule = _serviceProvider.GetService(computeRuleType) as IArticleComputeRule;
                            if (schoolComputeRule == null)
                            {
                                throw new ArgumentNullException($"无法将[{computeRuleType.FullName}]转换为[ISchoolComputeRule]。");
                            }
                            double score = await schoolComputeRule.Compute(article, articleS, feature);
                            totalScore += score;
                            addScoreRemarks.Add($"{feature.Alias}加{score}分");
                        }
                        if (top100temp.Count < top)
                        {
                            //如果没有找到比当前分数更大的，直接进列表。
                            top100temp.Add(new ArticleMap(
                             article.Id,
                             articleS.Id,
                             totalScore,
                             string.Join(";", addScoreRemarks)
                            ));
                        }
                        else
                        {
                            var minItemIndex = top100temp.FindIndex(s => s.Score <= totalScore);
                            if (minItemIndex >= 0)
                            {
                                top100temp[minItemIndex] = new ArticleMap(
                                 article.Id,
                                 articleS.Id,
                                 totalScore,
                                 string.Join(";", addScoreRemarks)
                                );
                            }
                        }

                    }
                    lock (lockobj)
                    {
                        _logger.LogDebug("正在处理当前任务结果。");
                        if (top100.Any() == true)
                        {
                            var temp = top100temp.Where(s => s.Score >= top100.Min(s1 => s1.Score) && s.Score <= top100.Max(s1 => s1.Score)).ToList();
                            top100.AddRange(temp);
                            top100 = top100.OrderByDescending(s => s.Score).Skip(0).Take(top).ToList();
                        }
                        else
                        {
                            top100.AddRange(top100temp);
                        }
                        susccessCounter++;
                    }
                });
                tasks.Add(task);
            }
            foreach (var task in tasks)
            {
                await task;
            }
            return top100;
        }


        public async Task<IEnumerable<SchoolScore>> GetSchoolScoresByFilter(SchoolFilterDefinition filterDefinition)
        {
            var features = await _mapFeatureRepository.GetMapFeaturesAsync(3);
            int top = 100;
            List<SchoolScore> top100 = new List<SchoolScore>();
            var schools = _schoolFileRepository.Query(s => s.City == filterDefinition.LocationCity && s.IsOnline);
            var schoolsCount = schools.Count();
            int offset = 0;
            int limit = 100000;
            int pageCount = (schoolsCount + limit - 1) / limit;
            int[] offsets = new int[pageCount];
            for (int i = 0; i < offsets.Length; i++)
            {
                offsets[i] = offset;
                offset += limit;
            }
            int susccessCounter = 0;
            object lockobj = new object();
            _logger.LogDebug($"本次处理总数:{schoolsCount}");
            List<Task> tasks = new List<Task>();
            foreach (var _offset in offsets)
            {
                Task task = Task.Run(async () =>
                {
                    List<SchoolScore> top100temp = new List<SchoolScore>();
                    _logger.LogDebug($"Task[{Task.CurrentId}]；当前Offset：{_offset}");
                    var currentSchools = schools.Skip(_offset).Take(limit);
                    _logger.LogDebug($"Task[{Task.CurrentId}]Done.本次处理条数：{currentSchools.Count()}");
                    foreach (var validSchool in currentSchools)
                    {
                        List<string> addScoreRemarks = new List<string>();
                        double totalScore = 0;
                        //特征匹配计分
                        foreach (var feature in features)
                        {
                            Type computeRuleType = SearchReflectType($"Sxb.Recommend.Application.MapFeatureComputeRules.{feature.ComputeRuleName}");
                            if (computeRuleType == null)
                            {
                                throw new ArgumentNullException($"找不到该特性对应的特性规则，[{feature.ComputeRuleName}]");
                            }
                            ISchoolFilterDefinitionComputeRule schoolFilterDefinitionComputeRule = _serviceProvider.GetService(computeRuleType) as ISchoolFilterDefinitionComputeRule;
                            if (schoolFilterDefinitionComputeRule == null)
                            {
                                throw new ArgumentNullException($"无法将[{computeRuleType.FullName}]转换为[ISchoolComputeRule]。");
                            }
                            double score = await schoolFilterDefinitionComputeRule.Compute(filterDefinition, validSchool, feature);
                            totalScore += score;
                            addScoreRemarks.Add($"{feature.Alias}加{score}分");
                        }

                        if (top100temp.Count < top)
                        {
                            //如果没有找到比当前分数更大的，直接进列表。
                            top100temp.Add(new SchoolScore() { School = validSchool, Score = totalScore, Remark = string.Join(";", addScoreRemarks) });
                        }
                        else
                        {
                            var minItemIndex = top100temp.FindIndex(s => s.Score <= totalScore);
                            if (minItemIndex >= 0)
                            {
                                top100temp[minItemIndex] = new SchoolScore() { School = validSchool, Score = totalScore, Remark = string.Join(";", addScoreRemarks) };
                            }
                        }
                    }
                    lock (lockobj)
                    {
                        _logger.LogDebug("正在处理当前任务结果。");
                        if (top100.Any() == true)
                        {
                            var temp = top100temp.Where(s => s.Score >= top100.Min(s1 => s1.Score) && s.Score <= top100.Max(s1 => s1.Score)).ToList();
                            top100.AddRange(temp);
                            top100 = top100.OrderByDescending(s => s.Score).Skip(0).Take(top).ToList();
                        }
                        else
                        {
                            top100.AddRange(top100temp);
                        }
                        susccessCounter++;
                    }
                });
                tasks.Add(task);
            }
            foreach (var task in tasks)
            {
                await task;
            }
            return top100;
        }

        Type SearchReflectType(string fullName)
        {
            return _memoryCache.GetOrCreate(fullName, (cacheEntry) =>
             {
                 cacheEntry.SetSize(0);
                 return this.GetType().Assembly.GetType(fullName, false);
             });
        }

    }
}

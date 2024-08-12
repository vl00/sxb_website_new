using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Recommend.Application.Services;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Value;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Recommend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendController : ControllerBase
    {
        IRecommendService _recommendService;
        ISchoolMapService _schoolMapService;
        IArticleMapService _articleMapService;
        ISyncDataSourceService _syncDataSourceService;
        IComputeScoreService _computeScoreService;
        ILogger<RecommendController> _logger;
        public RecommendController(IRecommendService recommendService
            , ILogger<RecommendController> logger
            , IArticleMapService articleMapService
            , ISchoolMapService schoolMapService
            , ISyncDataSourceService syncDataSourceService
            , IComputeScoreService computeScoreService)
        {
            _recommendService = recommendService;
            _logger = logger;
            _articleMapService = articleMapService;
            _schoolMapService = schoolMapService;
            _syncDataSourceService = syncDataSourceService;
            _computeScoreService = computeScoreService;
        }

        [HttpGet]
        [Route("[action]")]
        [Description("当前推荐系统的基础信息")]
        public async Task<ResponseResult> DataInfo()
        {
            var schools = _recommendService.QuerySchools(s => s.IsOnline);
            var articles = _recommendService.QueryArticles(a => a.IsOnline);
            Random random = new Random();
            var computeSchoolTimeSpan = await ComputeExcuteTimeAsync<Task>(() =>
         {
             return _computeScoreService.GetSchoolMaps(schools.ToList()[random.Next(0, schools.Count())]);
         });
            var computeArticleTimeSpan = await ComputeExcuteTimeAsync<Task>(() =>
         {
             return _computeScoreService.GetArticleMaps(articles.ToList()[random.Next(0, articles.Count())]);
         });
            return ResponseResult.Success(new
            {
                schoolTotal = schools.Count(),
                articleTotal = articles.Count(),
                schoolRandomMilliseconds = computeSchoolTimeSpan.TotalMilliseconds,
                articleRandomMilliseconds = computeArticleTimeSpan.TotalMilliseconds

            });

        }

        [HttpGet]
        [Route("[action]")]
        [Description("获取推荐学校列表")]
        public async Task<ResponseResult> GetSchoolIds(Guid eid, int pageIndex, int pageSize)
        {
            var school = _recommendService.QuerySchools(school => school.Id == eid).FirstOrDefault();
            if (school == null)
            {
                return ResponseResult.Failed("找不到该学校，也许数据源未更新或者数据源不存在该学校。");
            }
            int offset = (pageIndex - 1) * pageSize;
            var schools = await _recommendService.GetRecommendSchools(school, offset, pageSize);
            return ResponseResult.Success(schools.Select(s=>s.SchoolS.Id));
        }

        [HttpPost]
        [Route("[action]/filter")]
        [Description("获取推荐学校列表")]
        public async Task<ResponseResult> GetSchoolIds([FromBody] SchoolFilterDefinition schoolFilterDefinition
        , [FromQuery] int pageIndex
        , [FromQuery] int pageSize)
        {
            int offset = (pageIndex - 1) * pageSize;
            var schoolScores = await _recommendService.GetRecommendSchools(schoolFilterDefinition);
            return ResponseResult.Success(schoolScores.Skip(offset).Take(pageSize).Select(s=>s.School.Id));
        }


        [HttpGet]
        [Route("[action]/ExtId")]
        [Description("获取推荐学校列表")]
        public async Task<ResponseResult> GetSchools(Guid eid, int offset, int limit)
        {

            var school = _recommendService.QuerySchools(school => school.Id == eid).FirstOrDefault();
            if (school == null)
            {
                return ResponseResult.Failed("找不到该学校，也许数据源未更新或者数据源不存在该学校。");
            }

            var schools = (await _recommendService.GetRecommendSchools(school, offset, limit)).ToList();

            return ResponseResult.Success(schools);
        }

        [HttpPost]
        [Route("[action]/filter")]
        [Description("获取推荐学校列表")]
        public async Task<ResponseResult> GetSchools([FromBody] SchoolFilterDefinition schoolFilterDefinition
            , [FromQuery] int offset
            , [FromQuery] int limit)
        {
            var schoolScores = await _recommendService.GetRecommendSchools(schoolFilterDefinition);
            return ResponseResult.Success(schoolScores.Skip(offset).Take(limit));
        }


        [HttpGet]
        [Route("[action]/AId")]
        [Description("获取推荐文章列表")]
        public async Task<ResponseResult> GetArticles(Guid aid, int offset, int limit)
        {
            var article = _recommendService.QueryArticles(article => article.Id == aid).FirstOrDefault();
            if (article == null)
            {
                return ResponseResult.Failed("找不到该文章，也许数据源未更新或者数据源不存在该文章。");
            }
            var articlesMapValues = await _recommendService.GetRecommendArticles(article, offset, limit);
            return ResponseResult.Success(articlesMapValues);
        }


        [HttpGet]
        [Route("[action]")]
        [Description("获取推荐文章列表")]
        public async Task<ResponseResult> GetArticleIds(Guid aid, int pageIndex, int pageSize)
        {
            var article = _recommendService.QueryArticles(article => article.Id == aid).FirstOrDefault();
            if (article == null)
            {
                return ResponseResult.Failed("找不到该文章，也许数据源未更新或者数据源不存在该文章。");
            }
            int offset = (pageIndex - 1) * pageSize;
            var articles = await _recommendService.GetRecommendArticles(article, offset, pageSize);
            return ResponseResult.Success(articles.Select(a=>a.ArticleS.Id));
        }



        [HttpGet]
        [Route("[action]")]
        [Description("更新单所学校推荐结果。")]
        public async Task<ResponseResult> UpdateSchool(Guid eid)
        {
            var school = _recommendService.QuerySchools(school => school.Id == eid).FirstOrDefault();
            if (school == null)
            {
                return ResponseResult.Failed("找不到该学校，也许数据源未更新或者数据源不存在该学校。");
            }
            var mapsResult = await _schoolMapService.UpsertSchoolMaps(school);
            return ResponseResult.Success(mapsResult);
        }



        [HttpGet]
        [Route("[action]")]
        [Description("更新单篇文章推荐结果。")]
        public async Task<ResponseResult> UpdateArticle(Guid aid)
        {
            var article = _recommendService.QueryArticles(article => article.Id == aid).FirstOrDefault();
            if (article == null)
            {
                return ResponseResult.Failed("找不到该文章，也许数据源未更新或者数据源不存在该文章。");
            }
            var mapsResult = await _articleMapService.UpsertArticelMaps(article);
            return ResponseResult.Success(mapsResult);
        }


        [HttpGet]
        [Route("[action]")]
        [Description("同步学校数据源")]
        public ResponseResult SyncSchoolSourse()
        {
            _syncDataSourceService.SyncSchoolsCopies();
            return ResponseResult.Success();
        }

        [HttpGet]
        [Route("[action]")]
        [Description("同步文章数据源")]
        public ResponseResult SyncArticleSourse()
        {
            _syncDataSourceService.SyncArticleCopies();
            return ResponseResult.Success();
        }



        /// <summary>
        /// 计算action执行时间
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        async Task<TimeSpan> ComputeExcuteTimeAsync<T>(Func<Task> action)
        {
            var start = DateTime.Now;
            await action();
            var end = DateTime.Now;
            return end - start;
        }


    }
}

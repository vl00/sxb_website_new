using Sxb.WenDa.Common.Enum;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.ElasticSearch;
using Sxb.WenDa.Query.ElasticSearch.Models;
using Sxb.WenDa.Query.ElasticSearch.QueryModels;
using Sxb.WenDa.Query.SQL.Repositories;
using Sxb.Framework.Foundation;
using Sxb.WenDa.Common.ResponseDto.Home;
using Sxb.Framework.Cache.Redis;
using Sxb.WenDa.API.Extensions;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.OtherAPIClient.School;
using Sxb.Framework.AspNetCoreHelper.CheckException;
using Sxb.WenDa.API.Utils;

namespace Sxb.WenDa.API.Application.Query
{
    public class LanmuQuery : ILanmuQuery
    {
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ISchoolApiService _schoolApiService;

        private readonly IQuestionRepository _questionRepository;
        private readonly ICustomLanmuDataRepository _customLanmuDataRepository;
        private readonly ISchoolQuestionRepository _schoolQuestionRepository;
        private readonly ICityCategoryRepository _cityCategoryRepository;

        public LanmuQuery(IEasyRedisClient easyRedisClient, ISchoolApiService schoolApiService, IQuestionRepository questionRepository, ICustomLanmuDataRepository customLanmuDataRepository, ISchoolQuestionRepository schoolQuestionRepository, ICityCategoryRepository cityCategoryRepository)
        {
            _easyRedisClient = easyRedisClient;
            _schoolApiService = schoolApiService;
            _questionRepository = questionRepository;
            _customLanmuDataRepository = customLanmuDataRepository;
            _schoolQuestionRepository = schoolQuestionRepository;
            _cityCategoryRepository = cityCategoryRepository;
        }

        /// <summary>
        /// 获取主站热门专栏
        /// 
        /// 默认显示对应专栏内回答数最多的6个问题。
        /// 若回答数一样，则按问答总点赞数从多到少排序。
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public async Task<List<HotSubjectItemDto>> GetHotSubjects(ArticlePlatform platform)
        {
            var key = string.Format(CacheKeys.HomeHotSubjects, platform, "0");
            int top = 6;

            return await _easyRedisClient.GetOrAddAsync(key, async () =>
            {
                var hotSubjects = await _customLanmuDataRepository.GetLanmuData<List<HotSubjectItemDto>>(key);

                //var hotSubjectIds = hotSubjects.Select(s => s.SubjectId);
                foreach (var item in hotSubjects)
                {
                    if (item.Questions == null || !item.Questions.Any())
                    {
                        item.Questions = await _questionRepository.GetLinkListAsync(QuestionOrderBy.ReplyDesc, platform, city: null, item.SubjectId, top);
                    }
                }

                return hotSubjects;
            }, TimeSpan.FromMinutes(60));
        }

        /// <summary>
        /// 获取子站热门专栏
        /// 
        /// 显示专栏内容最多赞的两条问答。
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public async Task<List<SubHotSubjectItemDto>> GetSubHotSubjects(ArticlePlatform platform, int? city)
        {
            var key = string.Format(CacheKeys.HomeHotSubjects, platform, city ?? 0);
            int top = 2;

            //return await _easyRedisClient.GetOrUpdateAsync(key, async () =>
            //{
            var hotSubjects = await _customLanmuDataRepository.GetLanmuData<List<SubHotSubjectItemDto>>(key);

            //var hotSubjectIds = hotSubjects.Select(s => s.SubjectId);
            foreach (var item in hotSubjects)
            {
                if (item.Questions == null || !item.Questions.Any())
                {
                    var questions = await _questionRepository.GetQuestionAndAnswerLinkListAsync(
                        QuestionOrderBy.LikeTotalDesc,
                        platform,
                        city,
                        item.SubjectId,
                        top);

                    item.Questions = questions.ToList();
                    foreach (var q in item.Questions)
                    {
                        q.AnswerContent = HtmlHelper.PlainHtml(q.AnswerContent);
                    }

                    if (item.Questions.Any(s => string.IsNullOrWhiteSpace(s.AnswerContent)))
                    {
                        Check.Throw("查询失败");
                    }
                }
            }

            return hotSubjects;
            //}, TimeSpan.FromMinutes(60));
        }


        /// <summary>
        /// 获取大家热议
        /// </summary>
        /// <returns></returns>
        public async Task<List<QuestionLinkDto>> GetHotQuestions()
        {
            int top = 6;
            var key = string.Format(CacheKeys.HomeHotQuestions, ArticlePlatform.Master);
            return await _easyRedisClient.GetOrUpdateAsync(key, async () =>
            {
                var hotQuestions = await _customLanmuDataRepository.GetLanmuData<List<QuestionLinkDto>>(key);

                if (hotQuestions == null || hotQuestions.Count == 0)
                {
                    hotQuestions = (await _questionRepository.GetHotsAsync(top)).ToList();
                }
                return hotQuestions;
            }, TimeSpan.FromMinutes(60));
        }


        /// <summary>
        /// 获取热门学校和问题
        /// 首页可由后台配置
        /// </summary>
        /// <see cref="BackgroundServices.HotSchoolBackgroundService"/>
        /// <param name="platform"></param>
        /// <param name="city"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<HotQuestionSchoolItemDto>> GetHotSchools(ArticlePlatform platform, int? city, int pageIndex, int pageSize)
        {
            var offset = (pageIndex - 1) * pageSize;
            var stop = offset + pageSize - 1;

            city = platform == ArticlePlatform.Master ? 0 : city;
            var key = string.Format(CacheKeys.HomeHotSchoolQuestions, platform, city ?? 0);

            var data = await _easyRedisClient.SortedSetRangeByRankWithScoresAsync<HotQuestionSchoolItemDto>(
                key,
                offset,
                stop);

            return data.Select(s => s.Key).ToList();
        }


        public async Task<List<SubCategoryItemDto>> GetSubCategories(ArticlePlatform platform, int city)
        {
            int top = 12;
            int maxChildCount = 5;//最多展示五个子类
            var key = string.Format(CacheKeys.PlatformHomeCategories, platform, city);
            return await _easyRedisClient.GetOrAddAsync(key, async () =>
            {
                //var categories = await _customLanmuDataRepository.GetLanmuData<List<SubCategoryItemDto>>(key);
                //Check.HasValue(categories, "请配置首页分类");

                var platformValue = platform.GetDefaultValue<long>();
                var data = await _cityCategoryRepository.GetChildrenWithChildren(city, platformValue, canFindSchool: false);

                var categories = new List<SubCategoryItemDto>();
                foreach (var item in data)
                {
                    var category = new SubCategoryItemDto(item);
                    category.Questions = await GetQuestions(platform, city, category.CategoryId, top);

                    //仅展示有Questinon的数据
                    if (!category.Questions.Any()) continue;

                    category.Children ??= new List<SubCategoryItemDto>();
                    foreach (var child in category.Children)
                    {
                        child.Questions = await GetQuestions(platform, city, child.CategoryId, top);
                    }

                    //仅展示有Questinon的数据
                    category.Children = category.Children.Where(s => s.Questions.Any()).Take(maxChildCount).ToList();
                    categories.Add(category);
                }

                return categories;
            }, TimeSpan.FromMinutes(60));
        }

        private async Task<IEnumerable<QuestionLinkDto>> GetQuestions(ArticlePlatform platform, int city, long categoryId, int top)
        {
            var questions = await _questionRepository.GetQuestionsAsync(
                                    platform,
                                    city,
                                    new List<long>() { categoryId },
                                    pageIndex: 1,
                                    pageSize: top);

            return questions.Select(s => new QuestionLinkDto(s));
        }
    }
}

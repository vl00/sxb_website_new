using Sxb.WenDa.Common.Enum;
using Sxb.Framework.Cache.Redis;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.OtherAPIClient.School;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Common.ResponseDto.Home;
using Sxb.WenDa.Query.SQL.Repositories;

namespace Sxb.WenDa.API.BackgroundServices
{
    public class HotSchoolBackgroundService : BaseBackgroundService
    {
        private readonly ILogger<HotSchoolBackgroundService> _logger;
        private readonly IEasyRedisClient _easyRedisClient;


        private readonly ICustomLanmuDataRepository _customLanmuDataRepository;
        private readonly ICityCategoryRepository _cityCategoryRepository;
        private readonly IQuestionQuery _questionQuery;

        /// <summary>
        /// 过期时间
        /// </summary>
        static TimeSpan Expire => TimeSpan.FromDays(2);

        public HotSchoolBackgroundService(
            IEasyRedisClient easyRedisClient,
            IQuestionQuery questionQuery,
            ILogger<HotSchoolBackgroundService> logger,
            ICustomLanmuDataRepository customLanmuDataRepository,
            ICityCategoryRepository cityCategoryRepository)
             : base(TimeSpan.FromMinutes(10))
        {
            _easyRedisClient = easyRedisClient;
            _questionQuery = questionQuery;
            _logger = logger;
            _customLanmuDataRepository = customLanmuDataRepository;
            _cityCategoryRepository = cityCategoryRepository;
        }

        protected override async Task ExecutingAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("HotSchoolBackgroundService执行:{0}", DateTime.Now);

            var platforms = new[] {
                ArticlePlatform.YouEr,
                ArticlePlatform.XiaoXue,
                ArticlePlatform.ZhongXue,
                ArticlePlatform.GaoZhong,
            };


            int top = 30;

            //主站
            await CacheData(top, ArticlePlatform.Master, city: 0);

            //子站, 分城市
            var cities = await _cityCategoryRepository.GetCitys();
            foreach (var platform in platforms)
            {
                foreach (var city in cities)
                {
                    await CacheData(top, platform, city.Id);
                }
            }
        }

        /// <summary>
        /// 放到redis
        /// </summary>
        /// <param name="top"></param>
        /// <param name="platform"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        private async Task CacheData(int top, ArticlePlatform platform, long city)
        {
            try
            {
                var key = string.Format(CacheKeys.HomeHotSchoolQuestions, platform, city);

                //运营配置的数据
                var schools = await _customLanmuDataRepository.GetLanmuData<List<HotQuestionSchoolItemDto>>(key)
                    ?? new List<HotQuestionSchoolItemDto>();
                //数据库中的数据
                schools.AddRange(await _questionQuery.GetSchoolsAsync(platform, top));


                double score = 0;

                //清除旧数据
                await _easyRedisClient.SortedSetRemoveRangeByRankAsync(key, 0, int.MaxValue);

                //添加新数据
                await _easyRedisClient.SortedSetAddAsync(
                    key,
                    schools.ToDictionary(s => s, s => score++));

                //设置有效期
                await _easyRedisClient.CacheRedisClient.Database.KeyExpireAsync(key, Expire);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "platform={platform},city={city}", platform.ToString(), city);
            }
        }
    }
}

using StackExchange.Redis;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.API.Utils;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Common.OtherAPIClient.School;
using Sxb.WenDa.Common.OtherAPIClient.User;
using Sxb.WenDa.Query.SQL.Repositories;

namespace Sxb.WenDa.API.Application.Query
{
    public class MyQuery : IMyQuery
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ISchoolApiService _schoolApiService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IUserApiService _userApiService;
        private readonly IAnswerRepository _answerRepository;

        public MyQuery(IQuestionRepository questionRepository, ISchoolApiService schoolApiService, IAnswerRepository answerRepository,
            ICommentRepository commentRepository, IUserApiService userApiService,
            IEasyRedisClient easyRedisClient)
        {
            _questionRepository = questionRepository;
            _commentRepository = commentRepository;
            _schoolApiService = schoolApiService;
            this._easyRedisClient = easyRedisClient;
            _userApiService = userApiService;
            _answerRepository = answerRepository;
        }

        public async Task<MyWendaVm> GetMyWenda(Guid userId)
        {
            var result = new MyWendaVm();

            var userDto = (await _userApiService.GetUsersDesc(new[] { userId })).FirstOrDefault();
            if (userDto == null) throw new ResponseResultException("not found", 404);
            result.Id = userDto.Id;
            result.Name = userDto.Name;
            result.HeadImg = userDto.HeadImg;

            try
            {
                result.IsReal = await _userApiService.IsRealUser(userId);
            }
            catch { }

            // QuestionCount
            {
                var c = await _easyRedisClient.GetStringAsync(string.Format(CacheKeys.MyQuestionCount, userId))
                    .ContinueWith<int?>(_ => int.TryParse(_.Result, out var _c) ? _c : null);
                if (c == null)
                {
                    c = await _questionRepository.GetMyQuestionCount(userId);

                    await _easyRedisClient.AddStringAsync(string.Format(CacheKeys.MyQuestionCount, userId), $"{c}", TimeSpan.FromSeconds(60 * 5));
                }
                result.QuestionCount = c ?? 0;
            }
            // AnswerCount
            {
                var c = await _easyRedisClient.GetStringAsync(string.Format(CacheKeys.MyAnswerCount, userId))
                    .ContinueWith<int?>(_ => int.TryParse(_.Result, out var _c) ? _c : null);
                if (c == null)
                {
                    c = await _answerRepository.GetMyAnswerCount(userId);

                    await _easyRedisClient.AddStringAsync(string.Format(CacheKeys.MyAnswerCount, userId), $"{c}", TimeSpan.FromSeconds(60 * 5));
                }
                result.AnswerCount = c ?? 0;
            }
            // GetLikeCount
            {
                var c = await _easyRedisClient.GetStringAsync(string.Format(CacheKeys.MyGetLikeCount, userId))
                    .ContinueWith(_ => int.TryParse(_.Result, out var _c) ? _c : -1);
                if (c == -1)
                {
                    c = 0;
                    c += await _answerRepository.GetMyAnswerGetLikeCount(userId);
                    c += await _commentRepository.GetMyCommentLikeCount(userId);

                    await _easyRedisClient.AddStringAsync(string.Format(CacheKeys.MyGetLikeCount, userId), $"{c}", TimeSpan.FromSeconds(60 * 3));
                }
                result.GetLikeCount = Math.Max(c, 0);
            }

            return result;
        }

    }
}

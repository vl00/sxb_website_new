using MediatR;
using Sxb.Framework.AspNetCoreHelper.CheckException;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.WenDa.API.Extensions;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;
using Sxb.WenDa.Infrastructure.Repositories;

namespace Sxb.WenDa.API.Application.Commands
{
    public class CollectCommandHandler : IRequestHandler<CollectCommand, ResponseResult>
    {
        private readonly IUserCollectInfoRepository _userCollectInfoRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IEasyRedisClient _easyRedisClient;

        public CollectCommandHandler(IQuestionRepository questionRepository, ISubjectRepository subjectRepository, IEasyRedisClient easyRedisClient,
            IUserCollectInfoRepository userCollectInfoRepository)
        {
            _userCollectInfoRepository = userCollectInfoRepository;
            _questionRepository = questionRepository;
            _subjectRepository = subjectRepository;
            _easyRedisClient = easyRedisClient;
        }

        public async Task<ResponseResult> Handle(CollectCommand request, CancellationToken cancellationToken)
        {
            Check.IsTrue(request.UserId != Guid.Empty);
            Check.IsTrue(request.DataId != Guid.Empty);

            //lock
            await Main(request);

            return ResponseResult.Success();
        }

        public async Task Main(CollectCommand request)
        {
            var userCollect = _userCollectInfoRepository.FirstOrDefault(s => s.UserId == request.UserId && s.DataId == request.DataId);
            if (userCollect == null)
            {
                userCollect = new UserCollectInfo(request.Type, request.UserId, request.DataId);
                await _userCollectInfoRepository.AddAsync(userCollect);
                await _userCollectInfoRepository.UnitOfWork.SaveEntitiesAsync();
            }
            else
                Check.IsTrue(userCollect.IsValid != request.IsValid, "请勿重复操作");

            //关注/取关
            var value = userCollect.SetValid(request.IsValid);
            await _userCollectInfoRepository.UpdateAsync(userCollect);



            //修改关注数量
            if (request.Type == UserCollectType.Question)
            {
                await QuestionCollectCountAsync(request.DataId, value);

                await _easyRedisClient.DelRedisKeys(new[]
                {
                    string.Format(CacheKeys.Question, request.DataId),
                });
            }
            else if (request.Type == UserCollectType.Subject)
            {
                await SubjectCollectCountAsync(request.DataId, value);

                await _easyRedisClient.DelRedisKeys(new[]
                {
                    string.Format(CacheKeys.Subject, request.DataId),
                });
            }
        }

        /// <summary>
        /// 修改问题关注数量
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task QuestionCollectCountAsync(Guid dataId, int value)
        {
            var question = await _questionRepository.GetAsync(dataId);
            Check.IsNotNull(question, "问题不存在");

            question.CalcCollectCount(value);

            await _questionRepository.UpdateAsync(question);
        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SubjectCollectCountAsync(Guid dataId, int value)
        {
            var subject = await _subjectRepository.GetAsync(dataId);
            Check.IsNotNull(subject, "专栏不存在");

            subject.CalcCollectCount(value);

            await _subjectRepository.UpdateAsync(subject);
        }


    }
}

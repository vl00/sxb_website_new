using MediatR;
using Sxb.Framework.AspNetCoreHelper.CheckException;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.WenDa.API.Extensions;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Domain.AggregateModel.LikeAggregate;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;
using Sxb.WenDa.Infrastructure.Repositories;

namespace Sxb.WenDa.API.Application.Commands
{
    public class LikeCommandHandler : IRequestHandler<LikeCommand, ResponseResult>
    {
        private readonly IUserLikeInfoRepository _userLikeInfoRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IEasyRedisClient _easyRedisClient;

        public LikeCommandHandler(IUserLikeInfoRepository userLikeInfoRepository, IQuestionRepository questionRepository, IAnswerRepository answerRepository, ICommentRepository commentRepository,
            IEasyRedisClient easyRedisClient)
        {
            _userLikeInfoRepository = userLikeInfoRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _commentRepository = commentRepository;
            _easyRedisClient = easyRedisClient;
        }

        public async Task<ResponseResult> Handle(LikeCommand request, CancellationToken cancellationToken)
        {
            Check.IsTrue(request.UserId != Guid.Empty);
            Check.IsTrue(request.DataId != Guid.Empty);

            //lock
            await Main(request);

            return ResponseResult.Success();
        }

        public async Task Main(LikeCommand request)
        {
            var userLike = _userLikeInfoRepository.FirstOrDefault(s => s.UserId == request.UserId && s.DataId == request.DataId);
            if (userLike == null)
            {
                userLike = new UserLikeInfo(request.Type, request.UserId, request.DataId);
                await _userLikeInfoRepository.AddAsync(userLike);
                await _userLikeInfoRepository.UnitOfWork.SaveEntitiesAsync();
            }
            else
                Check.IsTrue(userLike.IsValid != request.IsValid, "请勿重复操作");

            //点赞/取消点赞
            var value = userLike.SetValid(request.IsValid);
            await _userLikeInfoRepository.UpdateAsync(userLike);



            //修改点赞数量
            if (request.Type == UserLikeType.Answer)
            {
                await AnswerLikeCountAsync(request.DataId, value);

                await _easyRedisClient.DelRedisKeys(new[]
                {
                    string.Format(CacheKeys.AnswerLikeCount, request.DataId),
                    string.Format(CacheKeys.AnswerIsLikeByMe, request.DataId, request.UserId),
                    CacheKeys.QuestionsAll,
                    "wenda:subject:*",
                    //CacheKeys.QuestionAnswersPageListAll,
                });
            }
            else if (request.Type == UserLikeType.Comment)
            {
                await CommentLikeCountAsync(request.DataId, value);

                await _easyRedisClient.DelRedisKeys(new[]
                {
                    string.Format(CacheKeys.CommentLikeCount, request.DataId),
                    string.Format(CacheKeys.CommentIsLikeByMe, request.DataId, request.UserId),
                });
            }
        }

        /// <summary>
        /// 修改回答的点赞数量
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task AnswerLikeCountAsync(Guid dataId, int value)
        {
            var answer = await _answerRepository.GetAsync(dataId);
            Check.IsNotNull(answer, "回答不存在");
            var question = await _questionRepository.GetAsync(answer.Qid);
            Check.IsNotNull(question, "问题不存在");

            //修改回答的点赞数量
            answer.CalcLikeCount(value);
            //修改问题的所有回答总点赞数量
            question.CalcLikeTotalCount(value);

            await _answerRepository.UpdateAsync(answer);
            await _questionRepository.UpdateAsync(question);
        }


        /// <summary>
        /// 修改评论的点赞数量
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task CommentLikeCountAsync(Guid dataId, int value)
        {
            var comment = await _commentRepository.GetAsync(dataId);
            Check.IsNotNull(comment, "评论不存在");

            comment.CalcLikeCount(value);

            await _commentRepository.UpdateAsync(comment);
        }


    }
}

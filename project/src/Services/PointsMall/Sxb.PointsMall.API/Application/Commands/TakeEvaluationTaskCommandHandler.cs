using MediatR;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.PointsMall.API.Config;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class TakeEvaluationTaskCommandHandler : IRequestHandler<TakeViewEvaluationTaskCommand, ResponseResult>
    {
        private readonly ILogger<TakeEvaluationTaskCommandHandler> _logger;
        private readonly IEasyRedisClient _easyRedisClient;

        public TakeEvaluationTaskCommandHandler(ILogger<TakeEvaluationTaskCommandHandler> logger, IEasyRedisClient easyRedisClient)
        {
            _logger = logger;
            _easyRedisClient = easyRedisClient;
        }

        public async Task<ResponseResult> Handle(TakeViewEvaluationTaskCommand request, CancellationToken cancellationToken)
        {
            //lock
            return await MainAsync(request);
        }

        public async Task<ResponseResult> MainAsync(TakeViewEvaluationTaskCommand request)
        {
            var source = request.ToJson();
            var signature = MD5Helper.GetMD5(source, Configs.GetTaskRSATail());

            var key = string.Format(RedisKeys.TakeViewEvaluationTaskEncrptKey, request.UserId);
            var succeed = await _easyRedisClient.AddAsync(key, request, TimeSpan.FromHours(24));

            if (!succeed)
                return ResponseResult.Success(new {
                    key = signature
                });
            return ResponseResult.Failed("redis add fail");
        }
    }
}

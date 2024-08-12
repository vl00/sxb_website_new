using MediatR;
using Polly;
using StackExchange.Redis;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.API.Extensions;
using Sxb.WenDa.API.Utils;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.OtherAPIClient.Org;
using Sxb.WenDa.Common.OtherAPIClient.School;
using Sxb.WenDa.Common.OtherAPIClient.User;
using Sxb.WenDa.Query.SQL.Repositories;
using System.Diagnostics;

namespace Sxb.WenDa.API.Application.Commands
{
    public class UpSubjectViewCountCommandHandler : IRequestHandler<UpSubjectViewCountCommand, UpSubjectViewCountCommandResult>
    {
        readonly IQuestionRepository _questionRepository;
        readonly ICityCategoryQuery _cityCategoryQuery;
        readonly IQaSubjectRepository _qaSubjectRepository;
        readonly ICityCategoryRepository _cityCategoryRepository;
        readonly ILogger log;
        readonly ISchoolApiService _schoolApiService;
        readonly IEasyRedisClient _easyRedisClient;
        readonly IUserQuery _userQuery;

        public UpSubjectViewCountCommandHandler(IQuestionRepository questionRepository, ICityCategoryQuery cityCategoryQuery, 
            IQaSubjectRepository qaSubjectRepository, ICityCategoryRepository cityCategoryRepository, ILoggerFactory loggerFactory,
            IUserQuery userQuery, ISchoolApiService schoolApiService, IEasyRedisClient easyRedisClient,
            IServiceProvider services)
        {
            _questionRepository = questionRepository;
            _cityCategoryQuery = cityCategoryQuery;
            _qaSubjectRepository = qaSubjectRepository;
            _cityCategoryRepository = cityCategoryRepository;
            this.log = loggerFactory.CreateLogger(this.GetType());
            _userQuery = userQuery;
            _schoolApiService = schoolApiService;
            _easyRedisClient = easyRedisClient;
        }

        public async Task<UpSubjectViewCountCommandResult> Handle(UpSubjectViewCountCommand cmd, CancellationToken cancellationToken)
        {
            var result = new UpSubjectViewCountCommandResult();
            if (cmd.UpToDb) await HandleOnSyncToDB(cmd);
            else await Handle1(cmd);
            return result;
        }

        internal async Task Handle1(UpSubjectViewCountCommand cmd)
        {
            var task_user = _userQuery.GetUserGzWxDto(cmd.UserId);

            var k1 = string.Format(CacheKeys.SubjectViewCountIncr, cmd.SubjectId);            
            try
            {
                var tasks = new List<Task>();
                var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                {
                    Task t = batch.StringIncrementAsync(k1, cmd.IncrCount);
                    tasks.Add(t);
                    t = batch.KeyExpireAsync(k1, TimeSpan.FromSeconds(60 * 5));
                    tasks.Add(t);
                }
                batch.Execute();
                await Task.WhenAll(tasks);                
            }
            catch (Exception ex)
            {
                log.LogError(ex, "更新subject viewcount失败 {@cmd}", cmd);
                throw;
            }
            
            // q
            AsyncUtils.StartNew(new UpSubjectViewCountCommand { SubjectId = cmd.SubjectId, UpToDb = true, IncrCount = cmd.IncrCount });

            await task_user.AwaitNoErr();
        }

        internal async Task HandleOnSyncToDB(UpSubjectViewCountCommand cmd)
        {
            var k1 = string.Format(CacheKeys.SubjectViewCountIncr, cmd.SubjectId);
            var k0 = string.Format(CacheKeys.SubjectViewCount0, cmd.SubjectId);

            var lck1f = new StecRedisLock1Factory1(_easyRedisClient);
            await using var lck1 = await lck1f.LockAsync(string.Format(CacheKeys.Wenda_lck_UpSubjectViewCount_OnSyncToDB, cmd.SubjectId));
            if (lck1.IsAvailable != true) return;

            //var incr = cmd.IncrCount;
            var incr = await _easyRedisClient.GetStringAsync(k1).ContinueWith(t => int.TryParse(t.Result, out var _i) ? _i : 0);
            if (incr == 0) return;

            // up db
            var (c0, c1) = await _qaSubjectRepository.UpSubjectViewCount(cmd.SubjectId, incr);

            // up cache            
            try
            {
                var tasks = new List<Task>();
                var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                {
                    Task t = batch.StringSetAsync(k0, c1, TimeSpan.FromSeconds(60 * 60 * 2));
                    tasks.Add(t);
                    t = batch.StringIncrementAsync(k1, 0 - incr);
                    tasks.Add(t);
                    t = batch.KeyExpireAsync(k1, TimeSpan.FromSeconds(60 * 5));
                    tasks.Add(t);
                }
                batch.Execute();
                await Task.WhenAll(tasks);
            }
            catch { }
        }
    }
}

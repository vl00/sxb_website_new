using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.User.API.Application.Query;
using Sxb.User.API.RequestContract.Collect;
using System;
using System.Threading.Tasks;

namespace Sxb.User.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SubscribeRemindsController : ControllerBase
    {
        private readonly ISubscribeRemindQuery _subscribeRemindQuery;

        public SubscribeRemindsController(ISubscribeRemindQuery subscribeRemindQuery)
        {
            _subscribeRemindQuery = subscribeRemindQuery;
        }

        [HttpGet("userIds")]
        public async Task<ResponseResult> GetUserIdsAsync(string groupCode, Guid subjectId, int pageIndex, int pageSize)
        {
            var result = await _subscribeRemindQuery.GetUserIdsAsync(groupCode, subjectId, pageIndex, pageSize);
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 查询是否已经订阅
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet("exists")]
        public async Task<ResponseResult> Exists(string groupCode, Guid subjectId, Guid userId)
        {
            //groupCode = "BigK";
            //var userId = User.Identity.GetID();
            if (userId == default)
            {
                return ResponseResult.DefaultFailed();
            }

            var isSubscribe = await _subscribeRemindQuery.ExistsAndSubscribeFwhAsync(groupCode, subjectId, userId);
            return ResponseResult.Success(new {
                IsSubscribe = isSubscribe
            });
        }
    }
}

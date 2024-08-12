using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.User.API.Application.Query;
using Sxb.User.API.RequestContract.Collect;
using Sxb.User.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.User.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        readonly IUserQuery _userQuery;

        public TestController(IUserQuery userQuery)
        {
            _userQuery = userQuery;
        }

        #region 用户id,名,头像,(达人)描述
        /// <summary>
        /// 用户id,名,头像,(达人)描述
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [HttpPost(nameof(GetUsersDesc))]
        [ProducesResponseType(typeof(TestController_GetUsersDesc_Return), 200)]
        public async Task<ResponseResult> GetUsersDesc(Guid[] userIds)
        {
            var r = await _userQuery.GetUsersDesc2(userIds?.Distinct());
            return ResponseResult.Success(new
            {
                Items = r.Select(x =>
                {
                    x.HeadImg = null;
                    return x;
                })
            });
        }
        class TestController_GetUsersDesc_Return
        {
            public IEnumerable<UserDescDto> Items { get; set; }
        }
        #endregion

        
    }
}

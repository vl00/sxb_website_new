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
    public class UsersController : ControllerBase
    {
        readonly IUserQuery _userQuery;

        public UsersController(IUserQuery userQuery)
        {
            _userQuery = userQuery;
        }

        [HttpGet("CheckIsSubscribe")]
        public async Task<ResponseResult> CheckIsSubscribe(Guid userId)
        {
            return ResponseResult.Success(new
            {
                IsSubscribe = await _userQuery.CheckIsSubscribe(userId)
            });
        }

        #region 是否真实用户
        /// <summary>
        /// 是否真实用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet(nameof(IsRealUser))]
        [ProducesResponseType(typeof(UsersController_IsRealUser_Return), 200)]
        public async Task<ResponseResult> IsRealUser(Guid userId)
        {
            var isReal = await _userQuery.IsRealUser(userId);
            return ResponseResult.Success(new { isReal });
        }
        class UsersController_IsRealUser_Return
        {
            public bool isReal { get; set; }
        }
        #endregion

        #region 用户是否绑定了手机号
        /// <summary>
        /// 用户是否绑定了手机号
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet(nameof(IsUserBindMobile))]
        [ProducesResponseType(typeof(UsersController_IsUserBindMobile_Return), 200)]
        public async Task<ResponseResult> IsUserBindMobile(Guid userId)
        {
            var isBind = await _userQuery.IsUserBindMobile(userId);
            return ResponseResult.Success(new { isBind });
        }
        class UsersController_IsUserBindMobile_Return
        {
            public bool isBind { get; set; }
        }
        #endregion

        /// <summary>
        /// get用户的wx的unionid
        /// </summary>
        /// <param name="id">userid or unionid</param>
        /// <returns></returns>
        [HttpGet("wx/unionid")]
        [ProducesResponseType(typeof(UserWxUnionIdDto), 200)]
        public async Task<ResponseResult> GetUserWxUnionId(string id)
        { 
            var r = await _userQuery.GetUserWxUnionId(id);
            return ResponseResult.Success(r);
        }

        #region 用户id,名,头像,(达人)描述
        /// <summary>
        /// 用户id,名,头像,(达人)描述
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [HttpPost(nameof(GetUsersDesc))]
        [ProducesResponseType(typeof(UsersController_GetUsersDesc_Return), 200)]
        public async Task<ResponseResult> GetUsersDesc(Guid[] userIds)
        {
            var r = await _userQuery.GetUsersDesc(userIds?.Distinct());
            return ResponseResult.Success(new UsersController_GetUsersDesc_Return { Items = r });
        }
        class UsersController_GetUsersDesc_Return
        {
            public IEnumerable<UserDescDto> Items { get; set; }
        }
        #endregion

        /// <summary>
        /// 同学段的达人信息 <br/>
        /// 用户id,名,头像,(达人)描述
        /// </summary>
        /// <param name="grade">学段,年级</param>
        /// <param name="top"></param>
        /// <returns></returns>
        [HttpGet(nameof(GetTopNTalentUserByGrade))]
        [ProducesResponseType(typeof(Page<TalentUserDescDto>), 200)]
        public async Task<ResponseResult> GetTopNTalentUserByGrade(int grade, int top)
        {
            var r = await _userQuery.GetTopNTalentUserByGrade(grade, top);
            return ResponseResult.Success(new Page<TalentUserDescDto>(r, r.Count()));
        }

        /// <summary>
        /// 随机n个虚拟用户 <br/>
        /// 用户id,名,头像,(达人)描述
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        [HttpGet(nameof(GetTopNRandVirtualUser))]
        [ProducesResponseType(typeof(Page<UserDescDto>), 200)]
        public async Task<ResponseResult> GetTopNRandVirtualUser(int top)
        {
            var r = await _userQuery.GetTopNRandVirtualUser(top);
            return ResponseResult.Success(new Page<UserDescDto>(r, r.Count()));
        }
        
        /// <summary>
        /// 获取用户昵称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost("nicknames")]
        [ProducesResponseType(typeof(IEnumerable<(Guid, string)>), 200)]
        public async Task<ResponseResult> GetNicknames([FromBody]IEnumerable<Guid> ids)
        {
            var data = await _userQuery.GetNicknames(ids);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 获取用户昵称和服务号openid
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost("WxNicknames")]
        [ProducesResponseType(typeof(UserWxFwhDto), 200)]
        public async Task<ResponseResult> GetFwhOpenIdAndNicknames([FromBody]IEnumerable<Guid> ids)
        {
            var data = await _userQuery.GetFwhOpenIdAndNicknames(ids);
            return ResponseResult.Success(data);
        }

    }
}

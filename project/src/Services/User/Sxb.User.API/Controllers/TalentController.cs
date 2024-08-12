using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.User.API.Application.Commands;
using Sxb.User.API.Application.Query;
using Sxb.User.API.Application.Query.ViewModels;
using Sxb.User.API.RequestContract.Talent;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.User.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TalentController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly IMyTalentQueries _myTalentQueries;
        public TalentController(IMediator mediator, IMyTalentQueries myTalentQueries)
        {
            _mediator = mediator;
            _myTalentQueries = myTalentQueries;
        }

        /// <summary>
        /// 通过达人码成为达人
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<long> InviteTalentByCode([FromBody] TalentInviteByCodeCommand cmd)
        {
            return await _mediator.Send(cmd, HttpContext.RequestAborted);
        }
        /// <summary>
        /// 申请达人
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Guid> ApplyTalent([FromBody] TalentApplyCommand cmd)
        {
            return await _mediator.Send(cmd, HttpContext.RequestAborted);
        }



        /// <summary>
        /// 查询达人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MyTalentViewModel> GetTalent([FromQuery] Guid userId)
        {
            return await Task.FromResult(_myTalentQueries.GetTalent(userId));
        }

        [HttpPost]
        public async Task<ResponseResult> GetByUserID(Guid userID)
        {
            var result = ResponseResult.Failed();
            if (userID == default) return result;
            var find = await _myTalentQueries.GetByUserID(userID);
            if (find?.ID != default)
            {
                return ResponseResult.Success(find);
            }
            return result;
        }

        [HttpPost]
        public async Task<ResponseResult> ListByUserIDs(ListByUserIDsRequest request)
        {
            var result = ResponseResult.Failed();
            if (request == null || request.UserIDs == default || !request.UserIDs.Any()) return result;
            var finds = await _myTalentQueries.ListByUserIDs(request.UserIDs);
            if (finds?.Any() == true)
            {
                return ResponseResult.Success(finds);
            }
            return result;
        }

        /// <summary>
        /// 获取学部绑定的付费达人用户ID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GetExtensionTalentUserID([FromBody] GetExtensionTalentUserIDRequest request)
        {
            var result = ResponseResult.Failed("param error");
            if (request == null || request.EID == default) return result;
            var userID = await _myTalentQueries.GetExtensionTalentUserID(request.EID);
            if (userID != default) result = ResponseResult.Success(userID);
            return result;
        }

        [HttpGet]
        public IActionResult Error()
        {
            throw new Exception("这是个模拟异常");
        }
    }
}

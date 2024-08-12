using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PaidQA.API.Application.Query;
using Sxb.PaidQA.API.RequestContact.Talent;
using Sxb.PaidQA.Common.EntityExtend;
using Sxb.PaidQA.Common.OtherAPIClient.User;
using System;
using System.Threading.Tasks;

namespace Sxb.PaidQA.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TalentController : Controller
    {
        readonly ITalentQuery _talentQuery;
        readonly IUserAPIClient _userAPIClient;
        public TalentController(ITalentQuery talentQuery, IUserAPIClient userAPIClient)
        {
            _talentQuery = talentQuery;
            _userAPIClient = userAPIClient;
        }

        /// <summary>
        /// 获取付费问答达人详情
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GetByUserID(Guid userID)
        {
            var result = ResponseResult.Failed();
            if (userID == default) return result;
            var find = await _talentQuery.GetDetail(userID);
            if (find?.TalentUserID != default) return ResponseResult.Success(find);
            return result;
        }
        [HttpPost]
        public async Task<ResponseResult> RandomByGrade(RandomByGradeRequest request)
        {
            var result = ResponseResult.Failed("params invalid");
            if (request == null) return result;
            var schoolTalentUserID = Guid.Empty;
            TalentDetailExtend find = default;
            if (request.EID.HasValue)
            {
                schoolTalentUserID = await _userAPIClient.GetExtensionTalentUserID(request.EID.Value);
            }
            if (schoolTalentUserID != default)
            {
                find = await _talentQuery.GetDetail(schoolTalentUserID);
            }
            if (find == default)
            {
                if (request.Grade > 0)
                {
                    find = await _talentQuery.RandomByGrade(request.Grade, request.IsInteral);
                }
            }
            if (find != default) result = ResponseResult.Success(find);
            return result;
        }

        [HttpGet]
        public IActionResult Error()
        {
            throw new Exception("这是个模拟异常");
        }
    }
}

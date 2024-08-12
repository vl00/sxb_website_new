using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.School.API.Application.Query;
using Sxb.School.API.RequestContact.School;
using Sxb.School.Common;
using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.API.RequestContact.CallSchoolApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.API.Controllers
{
    /// <summary>
    /// 内部调用
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class CallSchoolApiController : ControllerBase
    {
        readonly IEasyRedisClient _easyRedisClient;
        readonly ICallSchoolApiQuery _callSchoolApiQuery;

        public CallSchoolApiController(ICallSchoolApiQuery callSchoolApiQuery,
            IEasyRedisClient easyRedisClient)
        { 
            _easyRedisClient = easyRedisClient;
            _callSchoolApiQuery = callSchoolApiQuery;
        }

        /// <summary>
        /// 根据学部eid查询学校名,id,no
        /// </summary>
        /// <param name="eids">学部eid或学部短id</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(APIResult<GetSchoolsIdAndNameQryResult>), 200)]
        public async Task<ResponseResult> GetSchoolsIdAndName(string[] eids)
        {
            var result = new GetSchoolsIdAndNameQryResult();
            result.Items = await _callSchoolApiQuery.GetSchoolsIdAndName(eids);
            return ResponseResult.Success(result);
        }


    }
}

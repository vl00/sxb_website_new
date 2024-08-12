using Less.Html;
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
using Sxb.School.Common.OtherAPIClient.Article;
using Sxb.School.Common.OtherAPIClient.Comment;
using Sxb.School.Common.OtherAPIClient.Comment.Models;
using Sxb.School.Common.OtherAPIClient.PaidQA;
using Sxb.School.Common.OtherAPIClient.PaidQA.Model.EntityExtend;
using Sxb.School.Common.OtherAPIClient.User;
using Sxb.School.Common.OtherAPIClient.WxWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sxb.School.Common.OtherAPIClient.Comment.Model.Entity;

namespace Sxb.School.API.Controllers
{
    public partial class SchoolController : ControllerBase
    {
        /// <summary>
        /// 根据学部eid查询学校总评
        /// </summary>
        /// <param name="eids">学部eid或学部短id</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(APIResult<QueryGeneralCommentByEidsResponse>), 200)]
        public async Task<ResponseResult> QueryGeneralCommentByeids(string[] eids)
        {
            var result = new QueryGeneralCommentByEidsResponse();
            result.Items = await _schoolGeneralCommentQuery.QueryGeneralCommentByeids(eids);
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 根据年级显示院校库中总评分数最高的前10所学校
        /// </summary>
        /// <param name="grade">不传grade则不筛选</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(APIResult<QueryGeneralCommentByEidsResponse>), 200)]
        public async Task<ResponseResult> QueryTop10GeneralCommentByGrade(int grade = 0)
        {
            var result = new QueryGeneralCommentByEidsResponse();
            result.Items = await _schoolGeneralCommentQuery.QueryTop10GeneralCommentByGrade(grade);
            return ResponseResult.Success(result);
        }
    }
}

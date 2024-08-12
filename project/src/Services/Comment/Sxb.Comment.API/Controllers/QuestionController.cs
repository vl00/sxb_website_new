using Microsoft.AspNetCore.Mvc;
using Sxb.Comment.API.Application.Query;
using Sxb.Comment.API.RequestContract.Question;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Comment.API.Controllers
{
    /// <summary>
    /// 问答相关API
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class QuestionController : Controller
    {
        readonly IQuestionQuery _questionQuery;
        /// <summary>
        /// 问答相关API
        /// </summary>
        /// <param name="questionQuery"></param>
        public QuestionController(IQuestionQuery questionQuery)
        {
            _questionQuery = questionQuery;
        }

        /// <summary>
        /// 获取精选问答
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="userID">用户ID</param>
        /// <param name="take">获取条数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> Get(Guid eid, Guid userID,int take = 1)
        {
            var result = ResponseResult.Failed();
            if (eid == default) return result;
            var finds = await _questionQuery.ListByEIDs(new Guid[] { eid }, userID, take);
            if (finds?.Any() == true)
            {
                result = ResponseResult.Success(finds);
            }
            return result;
        }

        /// <summary>
        /// 获取问答统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> ListQuestionTotals(ListQuestionTotalsRequest request)
        {
            var result = ResponseResult.Failed();
            if (request == null || request.EID == default) return result;
            var finds = await _questionQuery.ListTotalsByEID(request.EID);
            if (finds?.Any() == true)
            {
                result = ResponseResult.Success(finds.Where(p => p.SchoolSectionID == default).Select(p => new
                {
                    p.SchoolSectionID,
                    p.Total,
                    TotalTypeName = p.TotalType.GetDescription()
                }));
            }
            return result;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Sxb.Comment.API.Application.Query;
using Sxb.Comment.API.RequestContract.Comment;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Comment.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CommentController : Controller
    {
        readonly ICommentQuery _commentQuery;
        readonly ISchoolQuery _schoolQuery;
        public CommentController(ICommentQuery commentQuery, ISchoolQuery schoolQuery)
        {
            _commentQuery = commentQuery;
            _schoolQuery = schoolQuery;
        }

        [HttpPost]
        public async Task<ResponseResult> GetSchoolSelectedComment([FromBody] GetSchoolSelectedCommentRequest request)
        {
            var result = ResponseResult.Failed();
            if (request == null || request.EID == default) return result;
            var finds = await _commentQuery.GetCommentsByEID(request.EID, request.UserID, request.Take);
            if (finds != default)
            {
                result = ResponseResult.Success(finds);
            }
            return result;
        }

        [HttpPost]
        public async Task<ResponseResult> GetCommentTotal(GetCommentTotalRequest request)
        {
            var result = ResponseResult.Failed();
            if (request == null || request.SID == default) return result;
            var eids = await _schoolQuery.ListValidEIDsAsync(request.SID);
            if (eids == default || !eids.Any()) return result;
            var finds = await _commentQuery.GetCommentTotalByEIDsAsync(eids);
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

        /// <summary>
        /// 获取学校评分(家长评价)共n人点评
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(APIResult<GetSchoolScoreCommentCountByEidsResponse>), 200)]
        public async Task<ResponseResult> GetSchoolScoreCommentCountByEids(GetSchoolScoreCommentCountByEidsRequest request)
        {
            // 参考 https://m.sxkid.com/api/schoolscore/GetCommentScoreStatistics?extID=

            var ls = await _commentQuery.GetSchoolScoreCommentCountByEids(request.Eids);
            return ResponseResult.Success(ls);
        }

    }
}


using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.User.API.Application.Query;
using Sxb.User.API.RequestContract.Collect;
using System.Threading.Tasks;

namespace Sxb.User.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        readonly ICollectionQuery _collectionQuery;
        public CollectionController(ICollectionQuery collectionQuery)
        {
            _collectionQuery = collectionQuery;
        }

        [HttpPost]
        public async Task<ResponseResult> CheckIsCollected(CheckIsCollectRequest request)
        {
            var result = ResponseResult.Failed();
            if (request.DataID == default || request.UserID == default) return result;
            return ResponseResult.Success(new
            {
                IsCollected = await _collectionQuery.CheckIsCollected(request.DataID, request.UserID)
            });

        }
    }
}

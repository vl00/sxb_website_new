using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.API.Application.Query
{
    public interface IMyQuery
    {
        /// <summary>我的-提问数+回答数+获赞数</summary>
        Task<MyWendaVm> GetMyWenda(Guid userId);





    }
}
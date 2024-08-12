using Sxb.WeWorkFinance.API.Application.Queries.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.Queries
{
    public interface IWeixinQueries
    {
        Task<AdviserGroupViewModel> GetAdviserGroupAsync(string unionId);
        Task<List<AdviserGroupViewModel>> GetAllAdviserGroupAsync();
        Task<string> GetWeixinAccessTokenAsync(string appid);
        Task<bool> ExistCustomer(string name);
        Task<List<string>> GetCustomerNames(string adviserUnionId);
        Task<CustomerViewModel> GetCustomerDetail(string inviterUnionId);
        Task<InviteStatisticalViewModel> GetInviteStatisticalData(string unionId);
        Task<InviterUserStatisticalViewModel> GetInviteStatisticalData2(string unionId, int pageNo, int pageSize);

        Task<List<InviteUserListViewModel>> GetInviteUserList(DateTime startTime, DateTime endTime);
        Task<InviteUserPointViewModel> GetInviteUserPoint(string unionId, DateTime startTime, DateTime endTime);
        Task<TakeOffPointViewModel> GetTakeoffPoint(string unionId);

        Task<bool> InsertChatData(List<ChatDataViewModel> chatDatas);

        Task<long> GetChatDataLastSeq();
    }
}

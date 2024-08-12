
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter.Model;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter
{
    public interface IFinanceCenterAPIClient
    {
        /// <summary>
        /// 入账
        /// 冻结金额内部接口调用直接入账
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<APIResult<WalletResponse>> FreezeAmountIncome(WalletRequest request);

        /// <summary>
        /// 解冻
        /// 解冻内部接口直接入账的冻结金额
        /// </summary>
        /// <param name="freezeMoneyInLogId"></param>
        /// <returns></returns>
        Task<APIResult<string>> InsideUnFreezeAmount(string freezeMoneyInLogId);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.GaoDeng
{
    public interface IGaoDengService
    {

        /// <summary>
        /// 获取高灯签约页面
        /// </summary>
        /// <returns></returns>
        string GetSignPage(UserBaseInfo userBaseInfo);



        /// <summary>
        /// 创建结算单
        /// </summary>
        /// <param name="balance"></param>
        /// <param name="callback_url"></param>
        /// <returns></returns>
        Task<ResponseResult> CreateForBatch(List<SettlementCreate> createForBatchRequest);

        /// <summary>
        /// 查询结算单
        /// </summary>
        /// <param name="order_random_code"></param>
        /// <param name="settlement_code"></param>
        /// <returns></returns>
        Task<ResponseResult<Settlement>> GetBalance(string order_random_code, string settlement_code);

        /// <summary>
        /// 结算单退款
        /// </summary>
        /// <param name="settlement_code"></param>
        /// <param name="order_random_code"></param>
        /// <param name="callback_url"></param>
        /// <returns></returns>
        Task<ResponseResult> RefundBalance(List<string> settlement_code = null, List<string> order_random_code = null);

        /// <summary>
        /// 获取用户认证结果
        /// </summary>
        /// <returns></returns>
        Task GetIdentityAuditResult();

        /// <summary>
        /// 获取用户签约情况
        /// </summary>
        /// <returns></returns>
        Task<ResponseResult<IEnumerable<AgreementResult>>> BatchQueryAgreement(BatchQueryAgreementRequest request);

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="ciyperText"></param>
        /// <returns></returns>
        string Decrytion(string ciyperText);



        T CallBackDecode<T>(string callbackBody) where T:class;




    }
}

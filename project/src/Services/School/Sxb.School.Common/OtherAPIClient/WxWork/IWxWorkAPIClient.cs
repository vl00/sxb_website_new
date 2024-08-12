using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sxb.School.Common.OtherAPIClient.WxWork
{
    /// <summary>
    /// 企业微信相关服务
    /// </summary>
    public interface IWxWorkAPIClient
    {
        /// <summary>
        /// 发送企业微信消息
        /// </summary>
        /// <param name="userIDs">用户IDs</param>
        /// <param name="content">消息内容</param>
        /// <param name="agentID">代理ID</param>
        /// <returns></returns>
        Task<bool> SendWxWorkMsgAsync(IEnumerable<string> userIDs, string content, string agentID);
    }
}

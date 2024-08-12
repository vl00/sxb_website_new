using System.Threading.Tasks;

namespace Sxb.SignActivity.Common.OtherAPIClient.Marketing
{
    public interface IOperationAPIClient
    {
        Task<SMSAPIResult> SMSApi(string templateId, string[] phones, string[] templateParams);
    }
}
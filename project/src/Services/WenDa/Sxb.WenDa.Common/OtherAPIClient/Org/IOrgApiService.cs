
namespace Sxb.WenDa.Common.OtherAPIClient.Org
{
    public interface IOrgApiService
    {
        Task DelRedisKeys(IEnumerable<string> keys, int waitSec = 2);
    }
}

using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public interface ICustomLanmuDataRepository
    {
        Task<CustomLanmuDataDto> GetAsync(string name);
        Task<T> GetLanmuData<T>(string key);
    }
}
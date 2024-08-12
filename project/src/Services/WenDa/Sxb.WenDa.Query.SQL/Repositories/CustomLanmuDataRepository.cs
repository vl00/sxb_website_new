using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.Foundation;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public class CustomLanmuDataRepository : ICustomLanmuDataRepository
    {
        readonly LocalQueryDB _queryDB;

        public CustomLanmuDataRepository(LocalQueryDB queryDB)
        {
            _queryDB = queryDB;
        }

        public async Task<T> GetLanmuData<T>(string key)
        {
            T data = default;
            var lanmu = await GetAsync(key);

            if (lanmu != null && lanmu.Ctn != null)
            {
                data = lanmu.Ctn.FromJsonSafe<T>();
            }

            if (data == null)
            {
                data = Activator.CreateInstance<T>();
            }
            return data;
        }

        public async Task<CustomLanmuDataDto> GetAsync(string name)
        {
            return await _queryDB.SlaveConnection
                .QuerySet<CustomLanmuData>()
                .Where(s => s.IsValid == true)
                .Where(s => s.Lanmu == name)
                .Select(s => new CustomLanmuDataDto()
                {
                    Name = s.Lanmu,
                    Ctn = s.Ctn,
                })
                .GetAsync<CustomLanmuDataDto>()
                ;
        }
    }
}

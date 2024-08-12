using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.WenDa.Common.Enum;
using Sxb.Framework.Foundation;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.DB;
using System.Linq;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public class UserCollectInfoRepository : IUserCollectInfoRepository
    {
        readonly LocalQueryDB _queryDB;

        public UserCollectInfoRepository(LocalQueryDB queryDB)
        {
            _queryDB = queryDB;
        }


        public async Task<IEnumerable<Guid>> GetCollectedDataIds(UserCollectType type, IEnumerable<Guid> ids, Guid userId)
        {
            if (ids == null || !ids.Any() || userId == default)
                return Enumerable.Empty<Guid>();

            var sql = $@"
                select DataId from UserCollectInfo where IsValid=1 and Type=@type and UserId=@userId and DataId in @ids
            ";
            return await _queryDB.SlaveConnection.QueryAsync<Guid>(sql, new { ids, userId, type });
        }

    }
}

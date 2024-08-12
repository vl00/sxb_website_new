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
    public class UserCategoryAttentionRepository : IUserCategoryAttentionRepository
    {
        readonly LocalQueryDB _queryDB;

        public UserCategoryAttentionRepository(LocalQueryDB queryDB)
        {
            _queryDB = queryDB;
        }

        public async Task<IEnumerable<long>> GetUserCategoryIdsAsync(Guid userId)
        {
            return await _queryDB.SlaveConnection.QuerySet<UserCategoryAttention>()
                .Where(s => s.UserId == userId && s.IsValid == true)
                .Select(s => s.CategoryId)
                .ToIEnumerableAsync<long>()
                ;
        }


        public async Task<IEnumerable<Guid>> GetUserByAttentionSameCategory(long categoryId, int top)
        {
            var sql = $@"
                select top {top} userid 
                from UserCategoryAttention where IsValid=1 and CategoryId=@categoryId
                order by CreateTime
            ";
            var ls = await _queryDB.SlaveConnection.QueryAsync<Guid>(sql, new { categoryId });
            return ls;
        }

    }
}

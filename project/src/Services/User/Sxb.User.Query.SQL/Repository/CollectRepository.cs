using Dapper;
using Sxb.User.Query.SQL.DB;
using Sxb.User.Query.SQL.IRepository;
using System;
using System.Threading.Tasks;

namespace Sxb.User.Query.SQL.Repository
{
    public class CollectRepository : ICollectRepository
    {
        SchoolUserDB _userDB;
        public CollectRepository(SchoolUserDB schoolUserDB)
        {
            _userDB = schoolUserDB;
        }

        public async Task<bool> CheckIsCollected(Guid dataID, Guid userID)
        {
            if (dataID == default || userID == default) return false;
            var str_SQL = "Select TOP 1 1 from collection WHERE dataID = @dataID and userID = @userID;";
            var find = await _userDB.SlaveConnection.QuerySingleAsync<int>(str_SQL, new { dataID, userID });
            return find > 0;
            //return await Task.Run(() =>
            //{
            //    return _userDB.SlaveConnection.QuerySet<CollectionInfo>().Where(p => p.DataID == dataID && p.UserID == userID).Count() > 0;
            //});
        }
    }
}

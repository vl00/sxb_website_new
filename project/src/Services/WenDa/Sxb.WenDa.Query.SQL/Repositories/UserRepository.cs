using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public class UserRepository : IUserRepository
    {
        readonly LocalQueryDB _queryDB;

        public UserRepository(LocalQueryDB queryDB)
        {
            _queryDB = queryDB;
        }

        public async Task<RealUser> GetRealUser(Guid userId)
        {
            var sql = "select * from RealUser where UserId=@userId ";
            var v = await _queryDB.SlaveConnection.QueryFirstOrDefaultAsync<RealUser>(sql, new { userId });
            return v;
        }

        public async Task SaveRealUser(RealUser rUser)
        {
            var sql = @"
                if not exists(select 1 from RealUser where UserId=@UserId) begin
                    insert into RealUser(UserId,IsRealUser,HasJoinWxEnt)
                    values(@UserId,@IsRealUser,@HasJoinWxEnt)
                end else begin
                    update RealUser set IsRealUser=@IsRealUser,HasJoinWxEnt=@HasJoinWxEnt
                    where UserId=@UserId
                end
            ";
            await _queryDB.Connection.ExecuteAsync(sql, rUser);
        }

        public async Task<IEnumerable<(Guid, bool)>> GetsIsLikeByMe(IEnumerable<Guid> ids, Guid userId, UserLikeType likeType)
        {
            var sql = $@"
                select u.DataId,u.IsValid as IsLikeByMe
                from UserLikeInfo u                 
                where u.Type=@ty and u.UserId=@userId {(ids?.Any() == true ? "and u.DataId in @ids" : "and 1=0")}
            ";
            var ls = await _queryDB.SlaveConnection.QueryAsync<(Guid, bool)>(sql, new { ids = ids.Select(_ => _.ToString()), userId, ty = (byte)likeType });
            return ls;
        }

    }
}
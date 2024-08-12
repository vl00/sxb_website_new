using Dapper;
using Sxb.Framework.Foundation;
using Sxb.User.Common.DTO;
using Sxb.User.Query.SQL.DB;
using Sxb.User.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.User.Query.SQL.Repository
{
    public class UserRepository : IUserRepository
    {
        SchoolUserDB _userDB;
        public UserRepository(SchoolUserDB schoolUserDB)
        {
            _userDB = schoolUserDB;
        }

        public async Task<bool> GetSubscribe(Guid userId)
        {
            var str_SQL = "Select TOP 1 1 from openid_weixin WHERE Valid = 1 and UserId = @userId;";
            var find = await _userDB.SlaveConnection.QuerySingleAsync<int>(str_SQL, new { userId });
            return find > 0;
        }

        public async Task<bool> IsRealUser(Guid userid)
        {
            var sql = "select top 1 channel from userinfo where id=@userid";
            var v = await _userDB.SlaveConnection.QueryFirstOrDefaultAsync<string>(sql, new { userid });
            return v != "1";
        }

        public async Task<bool> IsUserBindMobile(Guid userId)
        {
            var sql = "select top 1 1 from [userinfo] where id=@UserId and mobile is not null";
            var i = await _userDB.SlaveConnection.QueryFirstOrDefaultAsync<int?>(sql, new { userId });
            return i == 1;
        }

        public async Task<IEnumerable<UserDescDto>> GetUsersDesc(IEnumerable<Guid> userIds)
        {
            var result = new List<UserDescDto>();
            for (var ls = userIds; ls.Any(); ls = ls.Skip(200))
            {
                var arr = ls.Take(200).ToArray();

                var sql = @"
                    select u.id,u.nickname as name,u.HeadImgUrl as HeadImg, t.certification_identity as CertificationIdentity,t.certification_title as CertificationTitle,
                        t.certification_preview as CertificationPreview
                    from userinfo u left join talent t 
                        on u.id=t.user_id and t.isdelete=0 and t.status=1 and t.certification_status=1
                    where 1=1 and u.id in @userIds
                ";
                var v = await _userDB.SlaveConnection.QueryAsync<UserDescDto>(sql, new { userIds = arr.Select(_ => _.ToString()) });
                result.AddRange(v);
            }
            return result;
        }
        public async Task<IEnumerable<UserDescDto>> GetUsersDesc2(IEnumerable<Guid> userIds)
        {
            var result = new List<UserDescDto>();
            for (var ls = userIds; ls.Any(); ls = ls.Skip(200))
            {
                var arr = ls.Take(200).ToArray();

                var sql = @"
                    select 'eeeee'as _e, u.id,u.nickname as name,u.HeadImgUrl as HeadImg, t.certification_identity as CertificationIdentity,t.certification_title as CertificationTitle,
                        t.certification_preview as CertificationPreview
                    from userinfo u left join talent t 
                        on u.id=t.user_id and t.isdelete=0 and t.status=1 and t.certification_status=1
                    where 1=1 and u.id in @userIds
                ";
                //
                //!! dapper版本可能会使guid被翻译成sql_variant类型,进而在sql in语句里拖慢查询
                //
                var v = await _userDB.SlaveConnection.QueryAsync<UserDescDto>(sql, new { userIds = arr });
                result.AddRange(v);
            }
            return result;
        }

        public async Task<UserWxUnionIdDto> GetUserWxUnionId(Guid userId = default, string unionId = default)
        {
            var sql = $"select * from unionid_weixin where valid=1 {(userId != default ? "and userid=@userId" : "")} {(unionId != default ? "and unionId=@unionId" : "")} ";
            var v = await _userDB.SlaveConnection.QueryFirstOrDefaultAsync<UserWxUnionIdDto>(sql, new { userId, unionId });
            return v;
        }

        public async Task<IEnumerable<(Guid UserId, bool IsInternal)>> GetTopNTalentUserIdByGrade(int grade, int top)
        {
            var sql = $@"
                select top {top} t.user_id,t.IsInternal --,*
                from talent t
                left join interest i on t.user_id=i.userid 
                where t.isdelete=0 and t.status=1 and t.certification_status=1
                and i.grade_{grade}=1
            ";
            var ls = await _userDB.SlaveConnection.QueryAsync<(Guid, bool)>(sql, new { grade });
            return ls;
        }

        public async Task<IEnumerable<Guid>> GetTopNRandVirtualUserId(int top)
        {
            var sql = $@"
                select top {top} u.id from userinfo u 
                where u.channel=1 and id>=NEWID()
            ";
            var ls = await _userDB.SlaveConnection.QueryAsync<Guid>(sql);
            return ls;
        }

        public async Task<IEnumerable<(Guid, string)>> GetNicknames(IEnumerable<Guid> ids)
        {
            var sql = $@"select u.Id, u.NickName from UserInfo u where u.Id in {ids.ToSQLInString()}";
            return await _userDB.SlaveConnection.QueryAsync<(Guid, string)>(sql);
        }

        public async Task<IEnumerable<UserWxFwhDto>> GetFwhOpenIdAndNicknames(IEnumerable<Guid> ids)
        {
            var isSubscribe = true;
            var sql = $@"
select U.Id as UserId, U.NickName, O.OpenId, O.Valid AS IsSubscribe
FROM 
	UserInfo U
	INNER JOIN dbo.openid_weixin O ON O.UserId = U.id AND O.AppName='fwh'
WHERE
    O.Valid = @isSubscribe 
    AND U.Id in {ids.ToSQLInString()}
";
            return await _userDB.SlaveConnection.QueryAsync<UserWxFwhDto>(sql, new { isSubscribe });
        }
    }

    
}

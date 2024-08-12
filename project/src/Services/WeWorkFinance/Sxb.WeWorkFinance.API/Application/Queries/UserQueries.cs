using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Sxb.WeWorkFinance.API.Application.Queries.ViewModels;

namespace Sxb.WeWorkFinance.API.Application.Queries
{
    public class UserQueries : IUserQueries
    {
        private string _connectionString = string.Empty;

        public UserQueries(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }

        public async Task<UserInfoViewModel> GetUserInfo(string unionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var result = await connection.QueryFirstOrDefaultAsync<UserInfoViewModel>(
                   @"SELECT uw.userID,uw.unionID,u.nickname from userInfo u
                    LEFT JOIN unionid_weixin uw on uw.userID =  u.id
                    where uw.unionID = @unionId;"
                        , new { unionId }
                    );

                if (result == null)
                    return null;

                return result;
            }
        }


        public async Task<bool> AddUserInfo(string unionId,Guid userId,string nickName, int? sex,string headImgUrl)
        {
            using (var con = new SqlConnection(_connectionString))
            {

                var sql = "select count(1) from iSchoolUser.dbo.userinfo where cast(nickname as varbinary(2000))=cast(@nickName as varbinary(2000)) and len(nickname)=len(@nickName);";
                var rr = con.QueryFirstOrDefault<int>(sql,new { nickName });

                string wxName = nickName;
                if (rr > 0)
                {
                    nickName += userId.ToString().Substring(0, 5);
                }
                con.Open();
                IDbTransaction tran = con.BeginTransaction();
                try
                {
                    var sql1 = @"INSERT INTO iSchoolUser.[dbo].[userInfo]([id], [nationCode], [mobile], [password], [nickname], [regTime], [loginTime], [blockage], [headImgUrl], [sex], [remark]) 
                            VALUES(@userId, NULL, NULL, NULL, @nickName, getdate(),getdate(), '0', @headImgUrl, @sex, '一元购活动拉新'); ";
                    var sql2 = "INSERT INTO iSchoolUser.[dbo].[unionid_weixin]([unionID], [userID], [nickname], [valid]) VALUES (@unionId, @userId,@wxName, '1');";


                    await con.ExecuteAsync(sql1, new { userId,  nickName,  sex,  headImgUrl }, tran, null, null);
                    await con.ExecuteAsync(sql2, new { unionId, userId, wxName }, tran, null, null);
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return false;
                }
            }
        }


        public async Task<List<UserInfoViewModel>> GetUserFwhOpenId(List<string> unionIds)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<UserInfoViewModel>(
                   @"SELECT uw.userID,uw.unionID,u.nickname,ow.openID from userInfo u
                    LEFT JOIN unionid_weixin uw on uw.userID =  u.id
                    LEFT JOIN openid_weixin ow on ow.userID =  u.id and  ow.appName = 'fwh'
                    where uw.unionID in @unionIds;"
                        , new { unionIds }
                    );
                return result.ToList();
            }
        }

    }
}

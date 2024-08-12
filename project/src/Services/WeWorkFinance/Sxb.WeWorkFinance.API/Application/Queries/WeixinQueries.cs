using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Sxb.WeWorkFinance.API.Application.ES.SearchModels;
using Sxb.WeWorkFinance.API.Application.Queries.ViewModels;

namespace Sxb.WeWorkFinance.API.Application.Queries
{
    public class WeixinQueries: IWeixinQueries
    {
        private string _connectionString = string.Empty;

        public WeixinQueries(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }

        public async Task<AdviserGroupViewModel> GetAdviserGroupAsync(string unionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<AdviserGroupViewModel>(
                   @"select top 1 g.unionId,g.groupQrCodeUrl,g.CustomerId workgroup,c.city
                        FROM AdviserGroup g
                        left join Customer c on c.id = g.CustomerId
                        WHERE g.unionId = @unionId"
                        , new { unionId }
                    );

                if (result == null)
                    return null;

                return result;
            }
        }


        public async Task<bool> ExistCustomer(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<int>(
                   @"select top 1 count(1)
                        FROM Customer
                        WHERE name like @name"
                        , new { name = "%"+ name +"%"}
                    );

                return result > 0;
            }
        }

        public async Task<List<string>> GetCustomerNames(string adviserUnionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<string>(
                   @"SELECT c.name from AdviserGroup ag
                    LEFT JOIN Customer c on c.id = ag.CustomerId
                    where ag.UnionId = @adviserUnionId"
                        , new { adviserUnionId }
                    );

                return (result??"").Split("|").Where(q=> !string.IsNullOrWhiteSpace(q)).ToList();
            }
        }
        public async Task<CustomerViewModel> GetCustomerDetail(string inviterUnionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<CustomerViewModel>(
                   @"SELECT top 1 c.id CustomerId,c.name CustomerName,c.city,c.type from AdviserGroup ag
                    LEFT JOIN Customer c on c.id = ag.CustomerId
					LEFT JOIN CustomerQrCode qr on qr.AdviserId = ag.UnionId 
                    where qr.InviterId = @inviterUnionId order BY qr.CreateTime asc"
                        , new { inviterUnionId }
                    );

                return result;
            }
        }


        public async Task<List<AdviserGroupViewModel>> GetAllAdviserGroupAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<AdviserGroupViewModel>(
                   @"select unionId,groupQrCodeUrl
                        FROM AdviserGroup
                        WHERE isTest = 0;"
                    );
                return result.ToList();
            }
        }

        public async Task<string> GetWeixinAccessTokenAsync(string appid)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<string>(
                   @"select top 1 [value]
                        FROM [iSchool].[dbo].[keyValue]
                        WHERE [key] = @appid"
                        , new { appid }
                    );
                return result;
            }
        }


        public async Task<InviteStatisticalViewModel> GetInviteStatisticalData(string unionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<InviteStatisticalViewModel>(
                   @"select tt.ParentUserid unionId,tt.total
                    ,totaluser = STUFF((SELECT ',' + name
                    FROM Contact AS p1
                    WHERE p1.ParentUserid = tt.ParentUserid and p1.IsLastActivity = 0 FOR XML PATH('')), 1, 1, ''),
                    tt.validtotal,
                    validuser = STUFF((SELECT ',' + name
                    FROM Contact AS p1
                    WHERE p1.ParentUserid = tt.ParentUserid and p1.IsJoinGroup = 1 and p1.Gender = 2 and p1.IsLastActivity = 0 FOR XML PATH('')), 1, 1, ''),
                    tt.unvalidtotal,
										tt.notjointotal,
										notjoinuser = STUFF((SELECT ',' + name
                    FROM Contact AS p1
                    WHERE p1.ParentUserid = tt.ParentUserid and p1.IsJoinGroup = 0 and p1.IsLastActivity = 0 FOR XML PATH('')), 1, 1, ''),
                    tt.notladytotal,
                    notladyuser = STUFF((SELECT ',' + p1.name
                    FROM Contact AS p1
                    WHERE p1.ParentUserid = tt.ParentUserid and p1.Gender <> 2 and p1.IsLastActivity = 0 FOR XML PATH('')), 1, 1, ''),
                    (select count(1) from Contact cc where cc.UnionId in (SELECT gm.UnoinId from GroupMember gm where cc.unionId = gm.UnoinId) and cc.ParentUserid = tt.ParentUserid and cc.IsJoinGroup = 0 and cc.IsLastActivity = 0) as beforejointotal,
                    beforejoinuser = STUFF((SELECT ',' + cc.name
                    from Contact cc where cc.UnionId in (SELECT gm.UnoinId from GroupMember gm where cc.unionId = gm.UnoinId) and cc.ParentUserid = tt.ParentUserid and cc.IsJoinGroup = 0 and cc.IsLastActivity = 0 FOR XML PATH('')), 1, 1, '')
                    from 
                    (
                    SELECT ParentUserid, 
                    count(1) total,
                    sum(case when IsJoinGroup = 1 and Gender = 2  then 1 else 0 end) validtotal,
										sum(case when IsJoinGroup = 0 or Gender <> 2  then 1 else 0 end) unvalidtotal,
                    sum(case when IsJoinGroup = 0 then 1 else 0 end) notjointotal,
                    sum(case when Gender <> 2 then 1 else 0 end) notladytotal
                    FROM Contact c
                    where ParentUserid = @unionId and
                    IsLastActivity = 0
                    GROUP BY ParentUserid
                    ) tt;"
                        , new { unionId }
                    );
                return result;
            }
        }



        public async Task<InviterUserStatisticalViewModel> GetInviteStatisticalData2(string unionId,int pageNo,int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                int limit = pageSize;
                int offset = (--pageNo) * pageSize;

                var result = await connection.QueryFirstOrDefaultAsync<InviterUserStatisticalViewModel>(
                   @"	SELECT ParentUserid, 
                    count(1) total,
                    sum(case when IsJoinGroup = 1 and Gender = 2  then 1 else 0 end) validtotal,
					sum(case when IsJoinGroup = 0 or Gender <> 2  then 1 else 0 end) unvalidtotal,
                    sum(case when IsJoinGroup = 0 then 1 else 0 end) notjointotal,
                    sum(case when Gender <> 2 then 1 else 0 end) notladytotal
                    FROM Contact c
                    where ParentUserid = @unionId and
                    IsLastActivity = 0
                    GROUP BY ParentUserid;"
                       , new { unionId }
                   );

                if (result == null) return null;

                var list = await connection.QueryAsync<InviterUserDataViewModel>(
                    @"SELECT ParentUserid unionId, name,Avatar,
	                case when IsJoinGroup = 1 and Gender = 2 and (SELECT count(1) from ExitGroup ex where ex.UnionId = c.UnionId and ex.ParentUserid = c.ParentUserid)>0 then 4
									when IsJoinGroup = 1 and Gender = 2 then 1 
                    when IsJoinGroup = 0 and (SELECT count(gm.UnoinId) from GroupMember gm where gm.UnoinId = c.UnionId)>0 then 3
	                when IsJoinGroup = 0 then 2
	                when Gender <> 2 then 3 else -1 end InviteType,
	                case when IsJoinGroup = 1 and Gender = 2 and (SELECT count(1) from ExitGroup ex where ex.UnionId = c.UnionId and ex.ParentUserid = c.ParentUserid)>0 then '已退群' 
									when IsJoinGroup = 1 and Gender = 2   then '邀请成功' 
                  when IsJoinGroup = 0 and (SELECT count(gm.UnoinId) from GroupMember gm where gm.UnoinId = c.UnionId)>0 then '好友非新人'
	                when Gender <> 2 then '好友非女性' 	
	                when IsJoinGroup = 0 then '好友还没入群'
	                else '其他' end InviteTypeDesc
	                FROM Contact c
	                where ParentUserid = @unionId and IsLastActivity = 0
	                order by CreateTime asc OFFSET @offset rows fetch next @limit rows only;;"
                        , new { unionId, limit, offset }
                    );

                result.List = list.ToList();
                return result;
            }
        }

        /// <summary>
        /// 被扣除积分
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public async Task<TakeOffPointViewModel> GetTakeoffPoint(string unionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var list = await connection.QueryAsync<TakeOffPointViewModel>(
                    @"select c.ParentUserid unionId,count(1) ExitGroupCount,
                    (
                    select sum( case when ReturnPoint >= 0 then InvalidPoint else -ReturnPoint end ) from LogCorrectPoint cp where cp.InviterUnionId = c.ParentUserid
                    ) TakeOffPoint
                    from Contact c
                    where IsJoinGroup = 1 and Gender = 2 and (SELECT count(1) from ExitGroup ex where ex.UnionId = c.UnionId and ex.ParentUserid = c.ParentUserid)>0 and c.ParentUserid = @unionId
                    GROUP BY c.ParentUserid"
                        , new { unionId }
                    );

                return list.FirstOrDefault() ;
            }
        }


        public async Task<List<InviteUserListViewModel>> GetInviteUserList(DateTime startTime,DateTime endTime)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var list = await connection.QueryAsync<InviteUserListViewModel>(
                    @"select c.ParentUserid InviterUnionId,wx.userID,
                    count(1) TotalPoint,isnull(cp.EndTime,@startTime) LastEndTime
                    from Contact c
                    LEFT JOIN iSchoolUser.dbo.unionid_weixin wx on wx.unionid = c.ParentUserid
                    LEFT JOIN LogCorrectPoint cp on cp.InviterUnionId = c.ParentUserid and cp.EndTime = (select Max(cp2.EndTime) from LogCorrectPoint cp2 where cp2.InviterUnionId = c.ParentUserid)
                    where c.IsLastActivity = 0 and c.IsJoinGroup = 1 and c.Gender = 2
                    GROUP BY c.ParentUserid,wx.userID,cp.EndTime
"
                        , new { startTime, endTime }
                    );

                return list.ToList();
            }
        }
        public async Task<InviteUserPointViewModel> GetInviteUserPoint(string unionId ,DateTime startTime, DateTime endTime)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<InviteUserPointViewModel>(
                    @"select @unionId as unionid,
                    (select count(1) from Contact cc where cc.ParentUserid = @unionId and cc.IsJoinGroup = 1 and cc.IsLastActivity = 0 and cc.Gender = 2 and cc.CreateTime > @startTime) as AddPoint,
                    (select count(1) from Contact cc where cc.UnionId in (SELECT gm.UnoinId from GroupMember gm where cc.unionId = gm.UnoinId and gm.IsExit = '1') 
                    and cc.ParentUserid = @unionId and cc.IsJoinGroup = 1 and cc.IsLastActivity = 0 and cc.Gender = 2 and cc.CreateTime > @startTime ) as InvalidPoint"
                        , new { unionId, startTime, endTime }
                    );

                return result.FirstOrDefault();
            }
        }

        public async Task<long> GetChatDataLastSeq()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<int>(
                   @"select Max(seq)
                        FROM [dbo].[ChatData];"
                        , new {  }
                    );

                return result;
            }
        }
        public async Task<bool> InsertChatData(List<ChatDataViewModel> chatDatas)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                var result = await connection.ExecuteAsync(
                    @"INSERT INTO [dbo].[ChatData] ([Msgid], [Action], [From], [Tolist], [Roomid], [Msgtime], [Msgtype], [Contents],[Seq]) VALUES 
                    (@Msgid, @Action, @From, @Tolist, @Roomid,@Msgtime, @Msgtype, @Contents,@Seq); "
                        , chatDatas, transaction: trans
                    );
                trans.Commit();
                return result>0;
            }
        }
    }
}

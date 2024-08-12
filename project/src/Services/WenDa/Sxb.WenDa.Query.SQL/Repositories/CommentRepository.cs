using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.DB;
using Sxb.WenDa.Query.SQL.QueryDto;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        readonly LocalQueryDB _queryDB;

        public CommentRepository(LocalQueryDB queryDB)
        {
            _queryDB = queryDB;
        }

        public async Task<QaComment> GetComment(Guid id)
        {
            var sql = $@"select * from QaComment where id=@id ";
            var v = await _queryDB.SlaveConnection.QueryFirstOrDefaultAsync<QaComment>(sql, new { id });
            return v;
        }

        public async Task AddComment(QaComment comment, QaComment commentFrom = null, QuestionAnswer answer = null)
        {
            using var tctx = new DbTranCtx(_queryDB.Connection);
            string sql = null;
            tctx.BeginTransaction();

            await _queryDB.Connection.CommandSet<QaComment>(tctx.CurrentTransaction).InsertAsync(comment);

            if (commentFrom != null)
            {
                sql = @"
                    update QaComment set ReplyCount=ReplyCount+1,ModifyDateTime=@ModifyDateTime,Modifier=@Modifier
                    where Id=@Id ;
                    
                    update QaComment set ReplyTotalCount=isnull(ReplyTotalCount,0)+1,ModifyDateTime=@ModifyDateTime,Modifier=@Modifier
                    where Id=@MainCommentId
                ";
                await tctx.Connection.ExecuteAsync(sql, commentFrom, tctx.CurrentTransaction);
            }

            if (answer != null)
            {
                sql = @"
                    update QuestionAnswer set ReplyCount=ReplyCount+1,ModifyDateTime=@ModifyDateTime,Modifier=@Modifier
                    where Id=@Id ;
                ";
                await tctx.Connection.ExecuteAsync(sql, answer, tctx.CurrentTransaction);
            }

            tctx.CommitTransaction();
        }

        public async Task<int> GetMyCommentLikeCount(Guid userId)
        {
            var sql = @"
                select sum(LikeCount) from QaComment where IsValid=1 and UserId=@userId
            ";
            var v = await _queryDB.SlaveConnection.QueryFirstOrDefaultAsync<int?>(sql, new { userId });
            return v ?? 0;
        }

        public async Task<Page<CommentItemDto>> GetCommentsPageList(GetCommentsPageListQuery query)
        {
            var sql = $@"
                SELECT count(1) 
                from QaComment c 
                where c.IsValid=1 {(query.AnswerId != null ? "and c.aid=@AnswerId and c.id=c.maincommentid" : "")}
                {(query.CommentId != null ? "and c.maincommentid=@CommentId and c.id<>c.maincommentid" : "")}
                {(query.Naf == null ? "" : "and c.CreateTime<=@Naf")}

                SELECT c.Id,c.UserId,c.FromUserId,c.Content,c.CreateTime,c.LastEditTime as EditTime,c.LikeCount,c.ReplyTotalCount as ReplyCount
                from QaComment c 
                where c.IsValid=1 {(query.AnswerId != null ? "and c.aid=@AnswerId and c.id=c.maincommentid" : "")}
                {(query.CommentId != null ? "and c.maincommentid=@CommentId and c.id<>c.maincommentid" : "")}
                {(query.Naf == null ? "" : "and c.CreateTime<=@Naf")}
                order by {(query.Orderby == (int)CommentsListOrderByEnum.LikeCountDesc ? "c.LikeCount desc," : "")} c.CreateTime desc
                OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY ;
            ";
            var gr = await _queryDB.SlaveConnection.QueryMultipleAsync(sql, query);
            var total = await gr.ReadFirstAsync<int>();
            var ls = await gr.ReadAsync<CommentItemDto>();
            return new(ls.AsList(), total);
        }

        public async Task<IEnumerable<(Guid, CommentItemDto[])>> GetTop2ChildrenCommentsByMainCommentIds(IEnumerable<Guid> mainCommentIds)
        {
            var sql = @"
                select * from(
                    select row_number()over(partition by maincommentid order by createtime desc)_i,c.* 
                    from QaComment c 
                    where 1=1 and c.maincommentid in @mainCommentIds and c.id<>c.maincommentid
                )c where _i<=@i
            ";
            var ls = await _queryDB.SlaveConnection.QueryAsync<QaComment>(sql, new { mainCommentIds = mainCommentIds.Select(_ => _.ToString()), i = 2 });
            return ls.GroupBy(_ => _.MainCommentId).Select(x => (x.Key, x.OrderByDescending(_ => _.CreateTime)
                .Select(_ => new CommentItemDto 
                {
                    Id = _.Id,
                    UserId = _.UserId,
                    FromUserId = _.FromUserId,
                    Content = _.Content,
                    CreateTime = _.CreateTime,
                    EditTime = _.LastEditTime,
                    LikeCount = _.LikeCount,                    
                }).ToArray()));
        }

        public async Task<IEnumerable<(Guid, int)>> GetCommentsLikeCounts(IEnumerable<Guid> ids)
        {
            var sql = $@"
                select c.Id,c.LikeCount from QaComment c 
                where c.IsValid=1 {(ids?.Any() == true ? "and c.id in @ids" : "and 1=0")}
            ";
            var ls = await _queryDB.SlaveConnection.QueryAsync<(Guid, int)>(sql, new { ids = ids.Select(_ => _.ToString()) });
            return ls;
        }


        /// <summary>
        /// 有新被评论的用户
        /// 定时（每天晚上8点检查前一天晚上8点到当晚8点是否有收到评论，若收到，则推送该消息）
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<NotifyUserQueryDto>> GetCommentFromUserAsync(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var sql = $@"
SELECT *
FROM
(
	SELECT
		C.FromUserId AS UserId, Q.No, A.No as Ano,
		row_number() OVER(
			partition BY C.FromUserId
			ORDER BY C.CreateTime
		)AS row
	FROM QaComment C
		INNER JOIN QuestionAnswer A ON A.Id = C.Aid AND A.IsValid = 1
		INNER JOIN Question Q ON Q.Id = A.QId AND Q.IsValid = 1
	WHERE 
		C.IsValid = 1
		AND C.FromUserId IS NOT NULL -- null是导入的数据
		AND C.CreateTime >= @startTime 
        AND C.CreateTime < @endTime
) AS T
WHERE T.row = 1
ORDER BY T.UserId
offset (@pageIndex-1)*@pageSize rows 
FETCH next @pageSize rows only
";
            var param = new { startTime, endTime, pageIndex, pageSize };
            return await _queryDB.SlaveConnection.QueryAsync<NotifyUserQueryDto>(sql, param);
        }
    }
}
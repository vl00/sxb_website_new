using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.DB;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public class AnswerRepository : IAnswerRepository
    {
        readonly LocalQueryDB _queryDB;

        public AnswerRepository(LocalQueryDB queryDB)
        {
            _queryDB = queryDB;
        }

        public async Task<QuestionAnswer> GetAnswer(Guid id = default, long no = default)
        {
            var sql = $@"
                select * from QuestionAnswer where 1=1 {(id != default ? "and id=@id" : "")} {(no != default ? "and no=@no" : "")}
            ";
            var v = await _queryDB.SlaveConnection.QueryFirstOrDefaultAsync<QuestionAnswer>(sql, new { id, no });
            return v;
        }

        public async Task<bool> DeleteAnswer(QuestionAnswer answer, Question question = null)
        {
            using var tctx = new DbTranCtx(_queryDB.Connection);
            tctx.BeginTransaction();

            var sql = @"
                update QuestionAnswer set Modifier=@Modifier,ModifyDateTime=@ModifyDateTime,IsValid=@IsValid                    
                where Id=@Id ;
            ";
            var i = await tctx.Connection.ExecuteAsync(sql, answer, tctx.CurrentTransaction);

            sql = @"
                update Question set ReplyCount=ReplyCount-1,ModifyDateTime=@ModifyDateTime,Modifier=@Modifier
                where Id=@Id ;
            ";
            await tctx.Connection.ExecuteAsync(sql, question ?? (object)(new { answer.Modifier, answer.ModifyDateTime, Id = answer.Qid }), tctx.CurrentTransaction);

            tctx.CommitTransaction();
            return i > 0;
        }

        public async Task<bool> EditAnswer(QuestionAnswer answer)
        {
            var sql = @"
                update QuestionAnswer set Content=@Content,Imgs=@Imgs,Imgs_s=@Imgs_s,IsAnony=@IsAnony, --AnonyUserName=@AnonyUserName
                    LastEditTime=@LastEditTime,EditCount=@EditCount,Modifier=@Modifier,ModifyDateTime=@ModifyDateTime 
                where Id=@Id
            ";
            var i = await _queryDB.Connection.ExecuteAsync(sql, answer);
            return i > 0;
        }

        public async Task AddAnswer(QuestionAnswer answer, Question question = null, Invitation invitation = null)
        {
            using var tctx = new DbTranCtx(_queryDB.Connection);
            string sql = null;
            tctx.BeginTransaction();

            await _queryDB.Connection.CommandSet<QuestionAnswer>(tctx.CurrentTransaction).InsertAsync(answer, new[] { "No" });

            sql = @"
                update Question set ReplyCount=ReplyCount+1,ModifyDateTime=@ModifyDateTime,Modifier=@Modifier
                where Id=@Id ;
            ";
            await tctx.Connection.ExecuteAsync(sql, question ?? (object)(new { answer.Modifier, answer.ModifyDateTime, Id = answer.Qid }), tctx.CurrentTransaction);

            if (invitation != null)
            {
                sql = @"
                    update Invitation set AnswerTime=@AnswerTime where Id=@Id
                ";
                await tctx.Connection.ExecuteAsync(sql, invitation, tctx.CurrentTransaction);
            }

            tctx.CommitTransaction();

            sql = "select no from QuestionAnswer where id=@Id";
            answer.No = await _queryDB.Connection.QueryFirstOrDefaultAsync<long?>(sql, new { answer.Id }) ?? 0;
        }

        public async IAsyncEnumerable<(Guid UserId, bool IsInvited)> GetUsersIsInvitedToQuesion(Guid qid, IEnumerable<Guid> userIds)
        {
            var ls = await _queryDB.SlaveConnection.QuerySet<Invitation>()
                .Where("IsValid=1 and qid=@qid and ToUserId in @userIds and AnswerTime is null ", new { qid, userIds = userIds.Select(_ => _.ToString()) })
                .ToIEnumerableAsync();

            foreach (var userId in userIds)
            {
                var b = ls.Any(_ => _.ToUserId == userId);
                yield return (userId, b);
            }
        }

        public async Task<Invitation> GetInvitation(Guid qid, Guid toUserId)
        {
            var v = await _queryDB.SlaveConnection.QuerySet<Invitation>()
                .Where("IsValid=1 and qid=@qid and ToUserId=@toUserId and AnswerTime is null ", new { qid, toUserId })
                .GetAsync();

            return v;
        }

        public async Task AddInvitation(Invitation invitation)
        {
            await _queryDB.Connection.CommandSet<Invitation>().InsertAsync(invitation);
        }

        public async Task<int> GetMyAnswerCount(Guid userId)
        {
            var sql = @"
                select count(1) from QuestionAnswer where IsValid=1 and UserId=@userId
            ";
            var v = await _queryDB.SlaveConnection.QueryFirstOrDefaultAsync<int?>(sql, new { userId });
            return v ?? 0;
        }

        public async Task<int> GetMyAnswerGetLikeCount(Guid userId)
        {
            var sql = @"
                select sum(LikeCount) from QuestionAnswer where IsValid=1 and UserId=@userId
            ";
            var v = await _queryDB.SlaveConnection.QueryFirstOrDefaultAsync<int?>(sql, new { userId });
            return v ?? 0;
        }


        public async Task<IEnumerable<QuestionAnswer>> LoadQaAnswers(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default)
        {
            var sql = $@"
                select * from QuestionAnswer where IsValid=1 
                {(ids?.Any() == true && nos?.Any() == true ? "and ((id in @ids) or (no in @nos) ) "
                    : ids?.Any() == true ? "and id in @ids"
                    : nos?.Any() == true ? "and no in @nos" : "and 1=0")}
            ";
            var ls = await _queryDB.SlaveConnection.QueryAsync<QuestionAnswer>(sql, new { ids = ids.Select(_ => _.ToString()), nos });
            return ls;            
        }

        public async Task<IEnumerable<(Guid, int)>> GetAnswersReplyCounts(IEnumerable<Guid> ids)
        {
            //var sql = $@"
            //    select Id,ReplyCount from QuestionAnswer where IsValid=1 {(ids?.Any() == true ? "and id in @ids" : "and 1=0")}
            //";
            var sql = $@"
                select aid,count(1) as ReplyCount from QaComment where IsValid=1 {(ids?.Any() == true ? "and aid in @ids" : "and 1=0")} group by aid
            ";
            var ls = await _queryDB.SlaveConnection.QueryAsync<(Guid, int)>(sql, new { ids = ids.Select(_ => _.ToString()) });
            return ls;
        }

        public async Task<IEnumerable<(Guid, int)>> GetAnswersLikeCounts(IEnumerable<Guid> ids)
        {
            var sql = $@"
                select a.Id,a.LikeCount from QuestionAnswer a 
                where a.IsValid=1 {(ids?.Any() == true ? "and a.id in @ids" : "and 1=0")}
            ";
            var ls = await _queryDB.SlaveConnection.QueryAsync<(Guid, int)>(sql, new { ids = ids.Select(_ => _.ToString()) });
            return ls;
        }

        public async Task<IEnumerable<(Guid Aid, string Content)>> GetQaAnswersContent(IEnumerable<Guid> ids)
        {
            var sql = $@"
                select id,content from QuestionAnswer where IsValid=1 and id in @ids
            ";
            var ls = await _queryDB.SlaveConnection.QueryAsync<(Guid, string)>(sql, new { ids = ids.Select(_ => _.ToString()) });
            return ls;
        }


        public async Task<Page<Guid>> GetAnswersPageList(Guid questionId, int pageIndex, int pageSize, AnswersListOrderByEnum orderby)
        {
            var sql = $@"
                SELECT count(1) from QuestionAnswer a where a.IsValid=1 and a.qid=@questionId ;

                SELECT a.id 
                from QuestionAnswer a
                where a.IsValid=1 and a.qid=@questionId
                order by {(orderby == AnswersListOrderByEnum.LikeCountDesc ? "a.LikeCount desc," : "")} a.CreateTime desc
                OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY ;
            ";
            var gr = await _queryDB.SlaveConnection.QueryMultipleAsync(sql, new { questionId, pageIndex, pageSize });
            var total = await gr.ReadFirstAsync<int>();
            var ls = await gr.ReadAsync<Guid>();
            return new(ls.ToArray(), total);
        }

        public async Task<Page<(Guid Qid, Guid Aid)>> GetIdsByMyAnswers(Guid userId, int pageIndex, int pageSize)
        {
            // 同一问题，如果用户回答了多次，是显示1条
            //var sql = $@"
            //    select count(1) 
            //    from (
            //     select qid,max(createtime) as createtime from QuestionAnswer a
            //     where a.IsValid=1 and a.userid=@userId
            //     group by a.qid
            //    ) a0 ;
            //
            //    select a.qid,a.id as aid 
            //    from QuestionAnswer a join (
            //     select qid,max(createtime) as createtime from QuestionAnswer a
            //     where a.IsValid=1 and a.userid=@userId
            //     group by a.qid
            //    ) a0 on a.qid=a0.qid and a.createtime=a0.createtime
            //    where a.IsValid=1 and a.userid=@userId
            //    order by a.createtime desc
            //    OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY ;
            //";

            // 同一问题，如果用户回答了多次，是显示多条
            var sql = $@"
                select count(1) from QuestionAnswer a 
                where a.IsValid=1 and a.userid=@userId ;
            
                select a.qid,a.id as aid from QuestionAnswer a
                where a.IsValid=1 and a.userid=@userId
                order by a.createtime desc
                OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY ;
            ";

            var gr = await _queryDB.SlaveConnection.QueryMultipleAsync(sql, new { userId, pageIndex, pageSize });
            var total = await gr.ReadFirstAsync<int>();
            var ls = await gr.ReadAsync<(Guid, Guid)>();
            return new(ls.ToArray(), total);
        }

        public async Task<IEnumerable<(Guid Qid, Guid Aid)>> GetAnswersIdsWithMaxLikeCountInQuestion(IEnumerable<Guid> qids)
        {
            if (qids?.Any() != true) return Enumerable.Empty<(Guid, Guid)>();
            var sql = $@"
                select qid,aid from( 
                    select a.qid,a.id as aid, row_number()over(partition by a.qid order by a.LikeCount desc,a.createtime desc)as _i
                    from QuestionAnswer a 
                    where a.IsValid=1 and a.qid in @qids
                )T where _i=1
            ";
            var ls = await _queryDB.SlaveConnection.QueryAsync<(Guid, Guid)>(sql, new { qids = qids.Select(_ => _.ToString()) });
            return ls;
        }

        public async Task<IEnumerable<(Guid Qid, bool HasMyAnswers)>> GetQuestionsIfHasMyAnswers(Guid userId, IEnumerable<Guid> qids)
        {
            if (qids?.Any() != true) return Enumerable.Empty<(Guid, bool)>();

            var sql = $@"
                select a.qid,count(1) from QuestionAnswer a 
                where a.IsValid=1 and a.userid=@userId and a.qid in @qids
                group by a.qid
            ";
            var ls = await _queryDB.SlaveConnection.QueryAsync<(Guid, int)>(sql, new { userId, qids = qids.Select(_ => _.ToString()) });
            return ls.Select(_ => (_.Item1, _.Item2 > 0)).ToArray();
        }

    }
}
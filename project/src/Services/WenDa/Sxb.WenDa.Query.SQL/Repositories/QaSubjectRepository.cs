using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public class QaSubjectRepository : IQaSubjectRepository
    {
        readonly LocalQueryDB _queryDB;

        public QaSubjectRepository(LocalQueryDB queryDB)
        {
            _queryDB = queryDB;
        }

        public async Task<SubjectDbDto> GetSubject(Guid id = default, long no = default)
        {
            var ls = await GetSubjects(new[] { id }, new[] { no });
            return ls.FirstOrDefault();
        }

        public async Task<IEnumerable<SubjectDbDto>> GetSubjects(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default)
        {
            var sql = $@"
                select * from QaSubject where 1=1 
                {(ids?.Any() == true && nos?.Any() == true ? "and ((id in @ids) or (no in @nos) ) "
                    : ids?.Any() == true ? "and id in @ids"
                    : nos?.Any() == true ? "and no in @nos" : "and 1=0")}
            ";
            var subjects = await _queryDB.SlaveConnection.QueryAsync<SubjectDbDto>(sql, new { ids = ids.Select(_ => _.ToString()), nos });
            if (!subjects.Any()) return default;

            sql = @"
                select t.SubjectId,t.CategoryId,c.name as CategoryName from SubjectCategory t 
                left join Category c on c.id=t.CategoryId and c.IsValid=1 and c.type=1
                where t.SubjectId in @ids 
            ";
            var categorys = await _queryDB.SlaveConnection.QueryAsync<(Guid, long, string)>(sql, new { ids = subjects.Select(_ => _.Id.ToString()) });

            sql = @"
                select t.SubjectId,t.TagId,c.name as TagName from SubjectTag t 
                left join Category c on c.id=t.TagId and c.IsValid=1 and c.type=1
                where t.SubjectId in @ids 
            ";
            var tags = await _queryDB.SlaveConnection.QueryAsync<(Guid, long, string)>(sql, new { ids = subjects.Select(_ => _.Id.ToString()) });

            return subjects.Select(q =>
            {
                var categorys1 = categorys.Where(_ => _.Item1 == q.Id);
                q.CategoryIds = categorys1.Select(_ => _.Item2).ToArray();
                q.CategoryNames = categorys1.Select(_ => _.Item3).ToArray();
                var tags1 = tags.Where(_ => _.Item1 == q.Id);
                q.TagIds = tags1.Select(_ => _.Item2).ToArray();
                q.TagNames = tags1.Select(_ => _.Item3).ToArray();
                return q;
            }).ToArray();
        }

        public async Task<bool> IsCollectedByUser(Guid subjectId, Guid userId)
        {
            var sql = $@"
                select top 1 1 from UserCollectInfo where IsValid=1 and Type=@ty and UserId=@userId and DataId=@subjectId
            ";
            var b = await _queryDB.SlaveConnection.QueryFirstOrDefaultAsync<bool?>(sql, new { subjectId, userId, ty = (byte)UserCollectType.Subject });
            return b == true;
        }

        public async Task<Page<Guid>> GetUserCollectSubjectIds(Guid userId, int pageIndex, int pageSize)
        {
            var sql = $@"
                select count(1) from UserCollectInfo where IsValid=1 and Type=@ty and UserId=@userId 

                select DataId from UserCollectInfo where IsValid=1 and Type=@ty and UserId=@userId 
                order by CreateTime desc
                OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY
            ";
            var gr = await _queryDB.SlaveConnection.QueryMultipleAsync(sql, new { userId, pageIndex, pageSize, ty = (byte)UserCollectType.Subject });
            var total = await gr.ReadFirstAsync<int>();
            var ls = await gr.ReadAsync<Guid>();
            return new(ls.ToArray(), total);
        }

        public async Task<Page<Guid>> GetQuestionsIdsPageListBySubject(Guid subjectId, int pageIndex, int pageSize, SubjectQuestionListOrderByEnum orderBy)
        {
            string GetSql(bool isCount)
            {
                return $@"
                    select {(isCount ? "count(1)" : "q.id")} 
                    from Question q
                    left join SubjectQuestionTop s on s.IsValid=1 and q.Id=s.QuestionId and s.SubjectId=@subjectId 
                    where q.IsValid=1 and q.SubjectId=@subjectId
                    {(isCount ? ""
                        : orderBy == SubjectQuestionListOrderByEnum.Hot ? "order by (case when s.id is null then 0 else 1 end) desc, q.ReplyCount desc, q.LikeTotalCount desc, q.createtime desc"
                        : orderBy == SubjectQuestionListOrderByEnum.CreateTimeDesc ? "order by (case when s.id is null then 0 else 1 end) desc, q.createtime desc"
                        : "")}
                    {(isCount ? "" : "OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY")}
                ";
            }
            var sql = $"{GetSql(true)} ;\n{(GetSql(false))} ;";
            var gr = await _queryDB.SlaveConnection.QueryMultipleAsync(sql, new { subjectId, pageIndex, pageSize });
            var total = await gr.ReadFirstAsync<int>();
            var ls = await gr.ReadAsync<Guid>();
            return new(ls.ToArray(), total);
        }

        public async Task<IEnumerable<(Guid, int)>> GetSubjectsViewCounts(IEnumerable<Guid> subjectIds)
        {
            if (subjectIds?.Any() != true) return Enumerable.Empty<(Guid, int)>();
            var sql = "select id,viewcount from QaSubject where IsValid=1 and id in @subjectIds";
            var ls = await _queryDB.Connection.QueryAsync<(Guid, int)>(sql, new { subjectIds = subjectIds.Select(_ => _.ToString()) });
            return ls;
        }

        public async Task<(int, int)> UpSubjectViewCount(Guid subjectId, int incr = 1)
        {
            var sql = @"
                declare @c0 int;
                select @c0=ViewCount from QaSubject where id=@subjectId 

                update QaSubject set ViewCount=@c0+@incr where id=@subjectId

                select @c0
            ";
            var c0 = await _queryDB.Connection.ExecuteScalarAsync<int?>(sql, new { subjectId, incr });
            var c = c0 ?? 0;
            return (c, c + incr);
        }
    }
}
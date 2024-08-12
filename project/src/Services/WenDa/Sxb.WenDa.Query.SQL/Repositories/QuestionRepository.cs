using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.Enum;
using Sxb.Framework.Foundation;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.DB;
using Sxb.WenDa.Query.SQL.QueryDto;
using System.Text;
using Sxb.WenDa.Query.SQL.Helper;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        readonly LocalQueryDB _queryDB;
        readonly ILogger<QuestionRepository> _logger;

        public QuestionRepository(LocalQueryDB queryDB, ILogger<QuestionRepository> logger)
        {
            _queryDB = queryDB;
            _logger = logger;
        }

        public async Task<Common.Entity.Question> GetQuestion(Guid id = default, long no = default)
        {
            var sql = $@"
                select top 1 * from Question where IsValid=1 {(id != default ? "and id=@id" : "")} {(no != default ? "and no=@no" : "")}
            ";
            var itm = await _queryDB.SlaveConnection.QueryFirstOrDefaultAsync<Common.Entity.Question>(sql, new { id, no });
            return itm;
        }

        public async Task<List<GetQuestionByKeywordItemDto>> GetTopNQuestionByKeywords(string keyword, int count = 8)
        {
            var sql = $@"
                select top {count} q.id as QuestionId,q.no as QuestionNo,q.Title,q.ReplyCount 
                from Question q
                where q.IsValid=1 and (q.Title=@keyword or (q.Title like @keyword2 ))
                order by (case when q.Title=@keyword then 0 else 1 end), q.CreateTime desc
            ";
            var ls = (await _queryDB.SlaveConnection.QueryAsync<GetQuestionByKeywordItemDto>(sql, new { keyword, keyword2 = $"%{keyword}%" })).AsList();
            foreach (var item in ls)
            {
                item.QuestionNo = long.TryParse(item.QuestionNo, out var _no) ? UrlShortIdUtil.Long2Base32(_no) : "";
            }
            return ls;
        }

        public async Task<(Common.Entity.Question Question, Guid[] Eids, long[] TagIds)> LoadQuestion(Guid id = default, long no = default)
        {
            var sql = $@"
                select * from Question where IsValid=1 {(id != default ? "and id=@id" : "")} {(no != default ? "and no=@no" : "")}
            ";
            var question = await _queryDB.SlaveConnection.QueryFirstOrDefaultAsync<Common.Entity.Question>(sql, new { id, no });
            if (question == null) return default;

            sql = "select * from QuestionEids where qid=@id ";
            var eids = await _queryDB.SlaveConnection.QueryAsync<Common.Entity.QuestionEids>(sql, new { id });

            sql = "select * from QuestionTag where qid=@id ";
            var tags = await _queryDB.SlaveConnection.QueryAsync<Common.Entity.QuestionTag>(sql, new { id });

            var r_eids = eids.Any() ? eids.Select(_ => _.Eid).ToArray() : null;
            var r_tagids = tags.Any() ? tags.Select(_ => _.TagId).ToArray() : null;
            return (question, r_eids, r_tagids);
        }

        public async Task<IEnumerable<QuestionDbDto>> LoadQuestions(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default)
        {
            var sql = $@"
                select * from Question where IsValid=1
                {(ids?.Any() == true && nos?.Any() == true ? "and ((id in @ids) or (no in @nos) ) "
                    : ids?.Any() == true ? "and id in @ids"
                    : nos?.Any() == true ? "and no in @nos" : "and 1=0")}
            ";
            var questions = await _queryDB.SlaveConnection.QueryAsync<QuestionDbDto>(sql, new { ids = ids.Select(_ => _.ToString()), nos });
            if (!questions.Any()) return default;

            sql = "select * from QuestionEids where qid in @ids ";
            var eids = await _queryDB.SlaveConnection.QueryAsync<Common.Entity.QuestionEids>(sql, new { ids = questions.Select(_ => _.Id.ToString()) });

            sql = @"
                select t.qid,t.tagId,c.name as tagName from QuestionTag t 
                left join Category c on c.id=t.tagId and c.IsValid=1 and c.type=2
                where t.qid in @ids 
            ";
            var tags = await _queryDB.SlaveConnection.QueryAsync<(Guid, long, string)>(sql, new { ids = questions.Select(_ => _.Id.ToString()) });

            sql = "select c.id,c.name from Category c where c.IsValid=1 and c.type=1 and c.id in @cids ";
            var categorys = await _queryDB.SlaveConnection.QueryAsync<(long, string)>(sql, new { cids = questions.Select(_ => _.CategoryId).Where(_ => _ != null) });

            sql = "select city,cityName from CityInfo where IsValid=1 and city in @citys";
            var citys = await _queryDB.SlaveConnection.QueryAsync<(long, string)>(sql, new { citys = questions.Select(_ => _.City).Distinct() });

            return questions.Select(q =>
            {
                q.Eids = eids.Where(_ => _.Qid == q.Id).Select(_ => _.Eid).ToArray();
                q.CityName = citys.FirstOrDefault(_ => _.Item1 == q.City).Item2;
                var tags1 = tags.Where(_ => _.Item1 == q.Id);
                q.TagIds = tags1.Select(_ => _.Item2).ToArray();
                q.TagNames = tags1.Select(_ => _.Item3).ToArray();
                var categorys1 = categorys.FirstOrDefault(_ => _.Item1 == q.CategoryId);
                q.CategoryName = categorys1.Item2;
                return q;
            }).ToArray();
        }

        public async Task<bool> IsTitleExists(string title)
        {
            var sql = "select 1 from Question where IsValid=1 and title=@title";
            var b = await _queryDB.Connection.QueryFirstOrDefaultAsync<bool>(sql, new { title });
            return b;
        }

        public async Task BatchAddQuestions(List<Question> questions, List<QuestionAnswer> answers, List<QuestionTag> questionTags)
        {
            using var tctx = new DbTranCtx(_queryDB.Connection);
            tctx.BeginTransaction();

            await _queryDB.Connection.CommandSet<Question>(tctx.CurrentTransaction).InsertAsync(questions, new[] { "No" });
            if (answers?.Count > 0)
            {
                await _queryDB.Connection.CommandSet<QuestionAnswer>(tctx.CurrentTransaction).InsertAsync(answers, new[] { "No" });
            }
            if (questionTags?.Count > 0)
            {
                await _queryDB.Connection.CommandSet<QuestionTag>(tctx.CurrentTransaction).InsertAsync(questionTags);
            }
            tctx.CommitTransaction();
        }

        public async Task AddQuestion(Common.Entity.Question question, List<Common.Entity.QuestionEids> quesionEids, List<Common.Entity.QuestionTag> questionTags)
        {
            using var tctx = new DbTranCtx(_queryDB.Connection);
            tctx.BeginTransaction();

            await _queryDB.Connection.CommandSet<Common.Entity.Question>(tctx.CurrentTransaction).InsertAsync(question, new[] { "No" });
            if (quesionEids?.Count > 0)
            {
                await _queryDB.Connection.CommandSet<Common.Entity.QuestionEids>(tctx.CurrentTransaction).InsertAsync(quesionEids);
            }
            if (questionTags?.Count > 0)
            {
                await _queryDB.Connection.CommandSet<Common.Entity.QuestionTag>(tctx.CurrentTransaction).InsertAsync(questionTags);
            }

            tctx.CommitTransaction();

            var sql = "select no from Question where id=@Id";
            question.No = await _queryDB.Connection.QueryFirstOrDefaultAsync<long?>(sql, new { question.Id }) ?? 0;
        }

        public async Task EditQuestion(Common.Entity.Question question, List<Common.Entity.QuestionEids> quesionEids, List<Common.Entity.QuestionTag> questionTags)
        {
            using var tctx = new DbTranCtx(_queryDB.Connection);
            tctx.BeginTransaction();

            var sql = @"
                update Question set Platform=@Platform,City=@City,CategoryId=@CategoryId,Content=@Content,Imgs=@Imgs,Imgs_s=@Imgs_s,IsAnony=@IsAnony, --AnonyUserName=@AnonyUserName,
                    LastEditTime=@LastEditTime,EditCount=@EditCount,Modifier=@Modifier,ModifyDateTime=@ModifyDateTime
                where Id=@Id
            ";
            await _queryDB.Connection.ExecuteAsync(sql, question, tctx.CurrentTransaction);

            sql = "delete from QuestionEids where qid=@Id";
            await _queryDB.Connection.ExecuteAsync(sql, new { question.Id }, tctx.CurrentTransaction);
            if (quesionEids?.Count > 0)
            {
                await _queryDB.Connection.CommandSet<Common.Entity.QuestionEids>(tctx.CurrentTransaction).InsertAsync(quesionEids);
            }

            sql = "delete from QuestionTag where qid=@Id";
            await _queryDB.Connection.ExecuteAsync(sql, new { question.Id }, tctx.CurrentTransaction);
            if (questionTags?.Count > 0)
            {
                await _queryDB.Connection.CommandSet<Common.Entity.QuestionTag>(tctx.CurrentTransaction).InsertAsync(questionTags);
            }

            tctx.CommitTransaction();
        }

        public async Task<int> GetMyQuestionCount(Guid userId)
        {
            var sql = @"
                select count(1) from Question where IsValid=1 and UserId=@userId
            ";
            var v = await _queryDB.SlaveConnection.QueryFirstOrDefaultAsync<int?>(sql, new { userId });
            return v ?? 0;
        }

        public async Task<IEnumerable<(Guid Qid, bool IsCollectedByMe)>> IsCollectedByMe(Guid userId, IEnumerable<Guid> quesionIds)
        {
            var sql = @"
                select DataId,IsValid 
                from UserCollectInfo
                where Type=@ty and UserId=@userId and DataId in @quesionIds
            ";
            var ls = await _queryDB.SlaveConnection.QueryAsync<(Guid, bool)>(sql, new { userId, quesionIds = quesionIds.Select(_ => _.ToString()), ty = (int)UserCollectType.Question });
            return ls;
        }

        public async Task<Page<Guid>> GetMyQuestionsIds(Guid userId, int pageIndex, int pageSize)
        {
            var sql = $@"
                select count(1) from Question q where q.IsValid=1 and q.userid=@userId ;

                select q.id from Question q where q.IsValid=1 and q.userid=@userId
                order by q.CreateTime desc
                OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY ;
            ";
            var gr = await _queryDB.SlaveConnection.QueryMultipleAsync(sql, new { userId, pageIndex, pageSize });
            var total = await gr.ReadFirstAsync<int>();
            var ls = await gr.ReadAsync<Guid>();
            return new(ls.ToArray(), total);
        }

        public async Task<Page<Guid>> GetUserCollectQuestions(Guid userId, int pageIndex, int pageSize)
        {
            var sql = $@"
                select count(1) from UserCollectInfo where IsValid=1 and Type=@ty and UserId=@userId 

                select DataId from UserCollectInfo where IsValid=1 and Type=@ty and UserId=@userId 
                order by CreateTime desc
                OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY
            ";
            var gr = await _queryDB.SlaveConnection.QueryMultipleAsync(sql, new { userId, pageIndex, pageSize, ty = (byte)UserCollectType.Question });
            var total = await gr.ReadFirstAsync<int>();
            var ls = await gr.ReadAsync<Guid>();
            return new(ls.ToArray(), total);
        }

        public async Task<Page<Guid>> GetUserQuestionsToBeAnsweredByInvited(Guid userId, int pageIndex, int pageSize)
        {
            string GetSql(bool isCount)
            {
                return $@"
                    select {(isCount ? "count(1)" : "iv.qid")} 
                    from Invitation iv where iv.IsValid=1 and iv.AnswerTime is null and iv.ToUserId=@userId
                    {(isCount ? "" : "order by iv.InviteTime desc")}
                    {(isCount ? "" : "OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY")}
               ";
            }
            var sql = $"{GetSql(true)} ;\n{(GetSql(false))} ;";
            var gr = await _queryDB.SlaveConnection.QueryMultipleAsync(sql, new { userId, pageIndex, pageSize });
            var total = await gr.ReadFirstAsync<int>();
            var ls = await gr.ReadAsync<Guid>();
            return new(ls.ToArray(), total);
        }

        public async Task<Page<Guid>> GetUserQuestionsToBeAnswered(Guid userId, IEnumerable<long> categoryIds, IEnumerable<long> platforms, int pageIndex, int pageSize)
        {
            categoryIds = categoryIds?.Any() == true ? categoryIds : null;
            platforms = platforms?.Any() == true ? platforms : null;
            if (!((categoryIds == null && platforms == null) || (categoryIds != null && platforms != null))) return new(Enumerable.Empty<Guid>(), 0);
            Debug.Assert((categoryIds == null && platforms == null) || (categoryIds != null && platforms != null));
            //
            string GetSql()
            {
                //--and(q.ReplyCount=0 or not exists(select 1 from QuestionAnswer where IsValid=1 and qid=q.id and userid=@userId) )
                //--and not exists(select 1 from Invitation iv where iv.IsValid=1 and iv.AnswerTime is null and iv.ToUserId=@userId and iv.Qid=q.id)
                //              --{(categoryIds?.Any() == true ? "and q.CategoryId in @categoryIds" : "")}
                //
                //!! categoryIds is lv2 or lv3
                return $@"                
select count(1) from Question q
left join QuestionAnswer a on a.IsValid=1 and a.qid=q.id and a.userid=@userId
left join Invitation iv on iv.IsValid=1 and iv.ToUserId=@userId and iv.Qid=q.id and iv.AnswerTime is null
where q.IsValid=1 and q.userid<>@userId
    and (case when iv.id is not null then 0 when a.id is null then 1 else 0 end)=1       
{(categoryIds == null ? "" : "and (q.CategoryId in @categoryIds or q.Platform in @platforms)")}
--;
select id from (
select q.id, {(categoryIds == null ? "-1" : "(case when c.id is not null then 0 when c1.id is not null then 1 else 2 end)")} as _domain,
    isnull(q.IsRealUser,0) as IsRealUser, (case when q.IsRealUser=1 then -1 else q.ReplyCount end)as ReplyCount,
    q.createtime
from Question q
left join QuestionAnswer a on a.IsValid=1 and a.qid=q.id and a.userid=@userId
left join Invitation iv on iv.IsValid=1 and iv.ToUserId=@userId and iv.Qid=q.id and iv.AnswerTime is null
{(categoryIds == null ? "" : "left join Category c on q.CategoryId=c.id and c.IsValid=1 and c.id in @categoryIds")}
{(categoryIds == null ? "" : "left join Category c1 on q.Platform=c1.id and c1.IsValid=1 and c1.id in @platforms")}
where q.IsValid=1 and q.userid<>@userId
    and (case when iv.id is not null then 0 when a.id is null then 1 else 0 end)=1       
{(categoryIds == null ? "" : "and (q.CategoryId in @categoryIds or q.Platform in @platforms)")}
)T
order by _domain asc, IsRealUser desc, ReplyCount asc, createtime desc
OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY
--;                    
               ";
            }
            var sql = GetSql();
            var gr = await _queryDB.SlaveConnection.QueryMultipleAsync(sql, new { userId, categoryIds, platforms, pageIndex, pageSize });
            var total = await gr.ReadFirstAsync<int>();
            var ls = await gr.ReadAsync<Guid>();
            return new(ls.ToArray(), total);
        }

        public async Task<IEnumerable<RelevantQuestionDto>> GetTopNRelevantQuestions(int top = 6, long? city = null, IEnumerable<long> tagIds = null, IEnumerable<long> categoryIds = null, IEnumerable<Guid> qidsNotIn = null)
        {
            tagIds = tagIds?.Any() == true ? tagIds : null;
            categoryIds = tagIds == null ? categoryIds : null;

            var sql = $@"
                    select top {top} q.Id as QuestionId,q.No as _No,q.Title
                    from Question q 
                    where q.IsValid=1 {(qidsNotIn?.Any() == true ? "and q.id not in @qidsNotIn" : "")}
                        {(tagIds == null ? "" : "and exists(select 1 from QuestionTag qt where qt.Qid=q.Id and qt.TagId in @tagIds)")}
                        {(categoryIds?.Any() != true ? "" : "and q.CategoryId in @categoryIds")}
                    order by (case when q.city=@city then 0 else 1 end), q.ReplyCount desc
                ";
            var ls = (await _queryDB.SlaveConnection.QueryAsync<RelevantQuestionDto>(sql, new
            {
                city = city ?? -1,
                tagIds,
                categoryIds,
                qidsNotIn = qidsNotIn?.Select(_ => _.ToString()),
            })).AsList();

            foreach (var dto in ls)
            {
                dto.QuestionNo = dto._No == null ? null : UrlShortIdUtil.Long2Base32(dto._No.Value);
                dto._No = null;
            }

            return ls;
        }

        public async Task<Page<Guid>> GetEveryoneTalkingAboutPageList(int pageIndex, int pageSize)
        {
            var sql = $@"
                select count(1) from Question q where q.IsValid=1 and q.ReplyCount>0 ;

                select q.id, (case when q.CreateTime>=@time then 0 else 1 end)as t1,(case when q.CreateTime>=@time then q.LikeTotalCount else -1 end)as t2
                from Question q where q.IsValid=1 and q.ReplyCount>0
                order by q.IsTop desc, (case when q.CreateTime>=@time then 0 else 1 end), q.ReplyCount desc, (case when q.CreateTime>=@time then q.LikeTotalCount else -1 end) desc, q.CreateTime desc
                OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY ;
            ";
            var gr = await _queryDB.SlaveConnection.QueryMultipleAsync(sql, new { pageIndex, pageSize, time = DateTime.Today.AddDays(-30) });
            var total = await gr.ReadFirstAsync<int>();
            var ls = await gr.ReadAsync<(Guid, int, int)>();
            return new(ls.Select(_ => _.Item1).ToArray(), total);
        }


        /// <summary>
        /// 获取文章标题和短链接
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="platform"></param>
        /// <param name="city"></param>
        /// <param name="subjectId"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public async Task<IEnumerable<QuestionLinkDto>> GetLinkListAsync(QuestionOrderBy orderBy, ArticlePlatform? platform, long? city, Guid? subjectId, int top)
        {
            //new List<QuestionLinkDto>().OrderBy(s=>s.QuestionTitle).ThenBy(s => s.QuestionTitle).ToList();
            var platformValue = platform.GetDefaultValue<long>();
            var platformSql = platform == null || platform == ArticlePlatform.Master ? "" : " AND Platform = @platformValue ";
            var citySql = city == null ? "" : " AND City = @city ";
            var subjectSql = subjectId == null ? "" : " AND SubjectId = @subjectId ";

            string orderBySql = LocalSqlHelper.GetQuestionOrderBySql(orderBy);

            var sql = $@"
SELECT
	TOP {top}
    Id,
	No,
	Title
FROM
   Question
WHERE
	IsValid = 1
	{platformSql}
	{citySql}
	{subjectSql}
ORDER BY
   {orderBySql}
";
            var questions = await _queryDB.SlaveConnection.QueryAsync<Question>(sql, new { platformValue, city, subjectId, top });
            return questions.Select(s => new QuestionLinkDto(s));
        }


        /// <summary>
        /// 获取问题 标题和短链接, 最高赞回答内容和短链接
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="platform"></param>
        /// <param name="subjectId"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public async Task<IEnumerable<QuestionAndAnswerLinkDto>> GetQuestionAndAnswerLinkListAsync(QuestionOrderBy orderBy, ArticlePlatform? platform, long? city, Guid? subjectId, int top)
        {

            var sql = $@"
SELECT
	*
FROM
(
	SELECT 
        Qid, No, Content,
        row_number() OVER ( 
		    partition BY QId
		    ORDER BY LikeCount DESC
	    ) AS row
	FROM QuestionAnswer
	WHERE IsValid = 1 AND Qid in @ids
) AS T
WHERE
	row = 1
";
            var questions = await GetLinkListAsync(orderBy, platform, city, subjectId, top);

            var data = questions.Select(s => new QuestionAndAnswerLinkDto(s));
            var ids = questions.Select(s => s.Id.ToString());
            var answers = await _queryDB.SlaveConnection.QueryAsync<QuestionAnswer>(sql, new { ids });

            var dto =  data.Select(s =>
            {
                var answer = answers.FirstOrDefault(a => a.Qid == s.Id);
                return s.SetAnswer(answer);
            }).ToList();

            if (dto.Any(s => string.IsNullOrWhiteSpace(s.AnswerContent)))
            {
                _logger.LogInformation("GetQuestionAndAnswerLinkListAsync");
                _logger.LogInformation(ids.ToJson());
                _logger.LogInformation(questions.ToJson());
                _logger.LogInformation(answers.ToJson());

                var tests = await _queryDB.SlaveConnection.QueryAsync<dynamic>(sql, new { ids });
                _logger.LogInformation(tests.ToJson());
            }
            return dto;
        }

        /// <summary>
        /// 获取学校的热门问题
        /// </summary>
        /// <param name="extIds"></param>
        /// <param name="orderBy"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SchoolQuestionLinkDto>> GetSchoolQuestionLinksAsync(IEnumerable<Guid> extIds, QuestionOrderBy orderBy, int top)
        {
            string orderBySql = LocalSqlHelper.GetQuestionOrderBySql(orderBy);

            var sql = $@"
SELECT
	*
FROM
(
	SELECT 
		QE.EId as ExtId, Q.Id, Q.No, Q.Title, 
		row_number() OVER ( 
			partition BY QE.EId
			ORDER BY {orderBySql}
		) AS row
	FROM Question Q
	INNER JOIN dbo.QuestionEids QE ON QE.QId = Q.Id
	WHERE IsValid = 1 AND QE.EId in @extIds
) AS T
WHERE
	row <= @top
";
            return await _queryDB.SlaveConnection.QueryAsync<SchoolQuestionLinkDto>(sql, new { extIds = extIds.Select(_ => _.ToString()), top });
        }


        /// <summary>
        /// 获取大家热议的问题
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public async Task<IEnumerable<QuestionLinkDto>> GetHotsAsync(int top)
        {
            var startTime = DateTime.Today.AddDays(-30);
            var sql = $@"
SELECT TOP {top} Id,No, Title
FROM Question 
WHERE IsValid = 1 AND CreateTime >= @startTime
ORDER BY ReplyCount DESC, LikeTotalCount DESC
";
            var questions = await _queryDB.SlaveConnection.QueryAsync<Question>(sql, new { startTime });
            return questions.Select(s => new QuestionLinkDto(s));
        }


        /// <summary>
        /// 获取待您回答
        /// </summary>
        /// 若用户已设置擅长领域。
        ///  1、优先显示学段分类重合度高的问题。
        ///  2、在1的前提下，优先显示真实用户发起的提问。
        ///  3、在1和2的前提下，优先显示回答数少的问题。
        ///  若用户未设置擅长领域。
        ///  1、优先显示真实用户发起的提问。
        ///  2、在1的前提下优先显示回答数少的问题。
        /// <param name="top"></param>
        /// <returns></returns>
        public async Task<IEnumerable<WaitQuestionItemDto>> GetWaitsAsync(IEnumerable<long> categoryIds, Guid exclundUserId, int top)
        {
            //分类及其所有子分类
            var categorySql = LocalSqlHelper.GetCategoryExistsSql(categoryIds);

            var sql = $@"
SELECT TOP {top} Id, No, Title, UserId, CreateTime
FROM Question 
WHERE IsValid = 1 AND IsRealUser = 1 AND UserId != @exclundUserId {categorySql}
ORDER BY ReplyCount ASC
";
            var questions = await _queryDB.SlaveConnection.QueryAsync<Question>(sql, new { exclundUserId });
            return questions.Select(s => new WaitQuestionItemDto(s));
        }


        public async Task<IEnumerable<QuestionCategoryItemDto>> GetRandomRecommendAsync(int top)
        {
            var sql = $@"
SELECT
	 TOP {top} Q.Id, Q.No, Q.Title, C.Name AS CategoryName
FROM 
	Question Q
	INNER JOIN Category C ON C.Id = Q.CategoryId
WHERE Q.IsValid = 1
ORDER BY Q.CreateTime DESC
";
            var questions = await _queryDB.SlaveConnection.QueryAsync<QuestionCategoryQueryDto>(sql, new { });
            return questions.Select(s => s.Convert());
        }

        public async Task<IEnumerable<Question>> GetQuestionsAsync(IEnumerable<Guid> ids)
        {
            return await _queryDB.SlaveConnection
                .QuerySet<Question>()
                .Where(s => s.IsValid == true)
                .Where(q => ids.Contains(q.Id))
                .ToIEnumerableAsync();
        }

        /// <summary>
        /// 优先显示当前城市问答。
        /// 如果设有置顶项，则优先显示置顶项。（置顶数量与子站首页显示数一致）
        /// 排序优先显示30天内全站回答数最多的问题。如果回答数一样多，则按问答总点赞数从多到少排序。
        /// 30天外的优先显示有回答数（从多到少）的问题，如果回答数一样，则按日期从近到远进行排序。
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="city"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Page<Question>> GetQuestionPageAsync(ArticlePlatform platform, int city, IEnumerable<long> categoryIds, int pageIndex, int pageSize)
        {
            var platformValue = platform.GetDefaultValue<long>();
            var categorySql = LocalSqlHelper.GetCategoryExistsSql(categoryIds);
            //30天内的数据
            var sql = $@"
DECLARE @time DATETIME
SET @time = DATEADD(DAY, -30, GETDATE())

SELECT
    Q.*, 
    CASE WHEN Q.City = @city THEN 1 ELSE 0 END AS CityTop,
    CASE WHEN Q.CreateTime > @time THEN 1 ELSE 0 END AS TimeTop
FROM Question Q
WHERE
    Q.IsValid = 1 
    AND Q.Platform = @platformValue 
    {categorySql}
ORDER BY
	CityTop DESC, TimeTop DESC, Q.ReplyCount DESC, Q.LikeTotalCount DESC, Q.Id ASC
offset (@pageIndex-1)*@pageSize rows 
FETCH next @pageSize rows only
";

            var sqlTotal = $@"
SELECT
    count(1) as total
FROM Question Q
WHERE 
    Q.IsValid = 1 
    AND Q.Platform = @platformValue 
    {categorySql}
";

            var param = new { platformValue, city, pageIndex, pageSize };
            return await _queryDB.QueryPageAsync<Question>(sql, sqlTotal, param);
        }


        /// <summary>
        /// 优先显示当前城市问答。
        /// 如果设有置顶项，则优先显示置顶项。（置顶数量与子站首页显示数一致）
        /// 排序优先显示30天内全站回答数最多的问题。如果回答数一样多，则按问答总点赞数从多到少排序。
        /// 30天外的优先显示有回答数（从多到少）的问题，如果回答数一样，则按日期从近到远进行排序。
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="city"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Question>> GetQuestionsAsync(ArticlePlatform platform, int city, IEnumerable<long> categoryIds, int pageIndex, int pageSize)
        {
            var platformValue = platform.GetDefaultValue<long>();
            var categorySql = LocalSqlHelper.GetCategoryExistsSql(categoryIds);
            //30天内的数据
            var sql = $@"
DECLARE @time DATETIME
SET @time = DATEADD(DAY, -30, GETDATE())

SELECT
    Q.*, 
	 CASE WHEN Q.CreateTime > @time THEN 1 ELSE 0 END AS Level
FROM Question Q
WHERE 
    Q.IsValid = 1 
    AND Q.Platform = @platformValue 
    {categorySql}
ORDER BY 
	Level DESC, Q.ReplyCount DESC, Q.LikeTotalCount DESC, Q.Id ASC
offset (@pageIndex-1)*@pageSize rows 
FETCH next @pageSize rows only
";
            var param = new { platformValue, city, pageIndex, pageSize };
            return await _queryDB.SlaveConnection.QueryAsync<Question>(sql, param);
        }


        /// <summary>
        /// 有新回答的用户
        /// 定时（每天晚上8点检查前一天晚上8点到当晚8点是否有收到新回答，若收到，则推送该消息）
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<NotifyUserQueryDto>> GetNewAnswerUserAsync(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var sql = $@"
SELECT
    Q.UserId, MIN(Q.No) as No
FROM Question Q
WHERE 
    Q.IsValid = 1
    AND EXISTS (
        SELECT 1 FROM QuestionAnswer A 
        WHERE A.IsValid = 1 AND A.Qid = Q.Id 
            AND A.CreateTime >= @startTime AND A.CreateTime < @endTime
	 )
GROUP BY Q.UserId
ORDER BY Q.UserId
offset (@pageIndex-1)*@pageSize rows 
FETCH next @pageSize rows only
";
            var param = new { startTime, endTime, pageIndex, pageSize };
            return await _queryDB.SlaveConnection.QueryAsync<NotifyUserQueryDto>(sql, param);
        }
    }
}

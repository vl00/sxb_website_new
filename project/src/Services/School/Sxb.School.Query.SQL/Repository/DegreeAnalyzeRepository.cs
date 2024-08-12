using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.Foundation;
using Sxb.School.Common;
using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Query.SQL.DB;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.Repository
{
    public class DegreeAnalyzeRepository : IDegreeAnalyzeRepository
    {
        readonly SchoolDataDB _schoolDataDB;

        public DegreeAnalyzeRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<IEnumerable<DgAyQuestionDto>> FindQuestions(bool useReadConn = true)
        {
            var sql = @"
select n.nextqid,q.*
from DgAyQuestion q
left join DgAyToNextQuestion n on q.id=n.scid and n.sctype=2 and n.IsValid=1
where q.IsValid=1
order by q.id
";
            var conn = useReadConn ? _schoolDataDB.SlaveConnection : _schoolDataDB.Connection;
            var ls = await conn.QueryAsync<DgAyQuestionDto>(sql);
            return ls;
        }

        public async Task<IEnumerable<DgAyQuestionOptionDto>> FindQuestionOptions(bool useReadConn = true)
        {
            var sql = @"
select opt.*,n.nextqid
from DgAyQuestionOption opt
left join DgAyToNextQuestion n on opt.id=n.scid and n.sctype=1 and n.IsValid=1
where opt.IsValid=1
order by opt.qid,opt.sort
";
            var conn = useReadConn ? _schoolDataDB.SlaveConnection : _schoolDataDB.Connection;
            var ls = await conn.QueryAsync<DgAyQuestionOptionDto>(sql);
            return ls;
        }

        public async Task<IEnumerable<(long City, string CityName, long Area, string AreaName)>> FindAddrAreas(long city)
        {
            var sql = $@"
select d.city,c.name as cityname,d.area,a.name as areaname
from DgAyAddressAndPrimarySchool d 
left join keyvalue c on c.id=d.city and c.type=1 and c.IsValid=1
left join keyvalue a on a.id=d.area and a.type=1 and d.IsValid=1
where d.IsValid=1 and d.eid is not null
group by d.city,c.name,d.area,a.name
";
            var ls = await _schoolDataDB.SlaveConnection.QueryAsync<(long City, string CityName, long Area, string AreaName)>(sql, new { city });
            return ls;
        }

        public async Task<IEnumerable<(long City, string CityName, long Area, string AreaName, string Address, int Sort)>> FindAddresses(long? area = null)
        {
            var sql = $@"
select d.city,c.name as cityname,d.area,a.name as areaname,d.address,d.sort
from DgAyAddressAndPrimarySchool d 
left join keyvalue c on c.id=d.city and c.type=1 and c.IsValid=1
left join keyvalue a on a.id=d.area and a.type=1 and d.IsValid=1
where d.IsValid=1 {(area != null ? "and (d.area=@area or d.city=@area)" : "")} and d.eid is not null
order by d.sort
";
            var ls = await _schoolDataDB.SlaveConnection.QueryAsync<(long City, string CityName, long Area, string AreaName, string Address, int Sort)>(sql, new { area });
            return ls;
        }

        public async Task<PagedList<(long City, string CityName, long Area, string AreaName, string Address, int Sort)>> FindAddresses(long area, string kw, int pageIndex, int pageSize)
        {
            var sql = $@"
select count(1)
from DgAyAddressAndPrimarySchool d 
where d.IsValid=1 and (d.area=@area or d.city=@area) {(!string.IsNullOrEmpty(kw) ? "d.address like @kw" : "")} and d.eid is not null

select d.city,c.name as cityname,d.area,a.name as areaname,d.address,d.sort
from DgAyAddressAndPrimarySchool d 
left join keyvalue c on c.id=d.city and c.type=1 and c.IsValid=1
left join keyvalue a on a.id=d.area and a.type=1 and d.IsValid=1
where d.IsValid=1 and (d.area=@area or d.city=@area) {(!string.IsNullOrEmpty(kw) ? "d.address like @kw" : "")} and d.eid is not null
order by d.sort
OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY
";
            var gr = await _schoolDataDB.SlaveConnection.QueryMultipleAsync(sql, new { area, kw = $"%{kw}%", pageIndex, pageSize });
            var total = await gr.ReadFirstAsync<int>();
            var ls = await gr.ReadAsync<(long City, string CityName, long Area, string AreaName, string Address, int Sort)>();
            return ls.ToArray().ToPagedList(pageSize, pageIndex, total);
        }

        public async Task<bool> IsExistsAddresse(long area, string address)
        {
            var sql = @"select top 1 1 from DgAyAddressAndPrimarySchool where IsValid=1 and area=@area and address=@address";
            var itm = await _schoolDataDB.SlaveConnection.QueryFirstOrDefaultAsync<int?>(sql, new { area, address });
            return itm != null;
        }

        public async Task<PagedList<(Guid Id, string Title, DateTime Time, int i)>> GetMyQaResultList(Guid userid, int pageIndex, int pageSize)
        {
            var sql = $@"
select count(1) from DgAyUserQaPaper q where q.IsValid=1 and q.status=@status and q.userid=@userid ;

select q.id,q.title,q.UnlockedTime as [Time],
row_number()over(partition by userid,DATEADD(MS,0,DATEADD(DD, DATEDIFF(DD,0,q.UnlockedTime), 0)) order by UnlockedTime asc)as i
from DgAyUserQaPaper q
where q.IsValid=1 and q.status=@status and q.userid=@userid
order by q.UnlockedTime desc
OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY
";
            var gr = await _schoolDataDB.SlaveConnection.QueryMultipleAsync(sql, new
            {
                userid, pageIndex, pageSize,
                status = (int)DgAyStatusEnum.Unlocked,
            });
            var cc = await gr.ReadFirstAsync<int>();
            var items = await gr.ReadAsync<(Guid Id, string Title, DateTime Time, int i)>();
            return items.ToPagedList(pageSize, pageIndex, cc);
        }

        public async Task<DgAyUserQaPaperDto> GetQaPaperAndResult(Guid id)
        {
            var sql = $@"
select qa.*,r.ctn from DgAyUserQaPaper qa
left join DgAyUserQaResultContent r on qa.id=r.qaid and r.IsValid=1
where qa.IsValid=1 and qa.id=@id
";
            var itm = await _schoolDataDB.SlaveConnection.QueryFirstOrDefaultAsync<DgAyUserQaPaperDto>(sql, new { id });
            return itm;
        }

        public async Task<DgAyUserQaContent[]> GetQaContents(Guid id)
        {
            var sql = "select * from DgAyUserQaContent where qaid=@id order by qaid,num";
            var ls = await _schoolDataDB.SlaveConnection.QueryAsync<DgAyUserQaContent>(sql, new { id });
            return ls.ToArray();
        }

        public async Task<DgAyUserQaPaperIsUnlockedDto> GetQaIsUnlocked(Guid id)
        {
            var sql = $@"
select qa.Id,qa.UserId,qa.Status,qa.UnlockedType,qa.UnlockedTime from DgAyUserQaPaper qa
where qa.IsValid=1 and qa.id=@id
";
            var itm = await _schoolDataDB.SlaveConnection.QueryFirstOrDefaultAsync<DgAyUserQaPaperIsUnlockedDto>(sql, new { id });
            return itm;
        }

        public async Task<IEnumerable<DgAySchoolItemDto>> GetSchoolItems(IEnumerable<Guid> eids)
        {
            var sql = $@"
select e.id as eid,e.no,s.name as schname,e.name as extname,e2.Address,e.schftype,s.EduSysType,e2.authentication,v.RecruitWay,
(case when s.IsValid=1 and e.IsValid=1 then 1 else 0 end)as IsValid
from OnlineSchool s left join OnlineSchoolExtension e on s.id=e.sid 
left join OnlineSchoolExtContent e2 on e2.eid=e.id and e2.IsValid=1
left join SchoolOverViewInfo v on v.eid=e.id 
where 1=1 --and s.status=3 
and e.id in @eids
";
            var ls = await _schoolDataDB.SlaveConnection.QueryAsync<DgAySchoolItemDto>(sql, new { eids });
            return ls;
        }

        public async Task<IEnumerable<(Guid Eid, double Score)>> GetSchoolJfScoreLine(IEnumerable<Guid> eids, int year)
        {
            var sql = $@"
select e3.eid,e3.IntegralAdmitLevel as JfScoreLine
from  OnlineSchoolRecruitInfo e3 
where 1=1 and e3.year=@year and e3.eid in @eids and e3.type=3
";
            var ls = await _schoolDataDB.SlaveConnection.QueryAsync<(Guid, double)>(sql, new { eids, year });
            return ls;
        }

        public async Task<(long Code, string Name)> GetCityAreaByCodeOrName(string v, long? parentCode = null)
        {
            var code = long.TryParse(v, out var _code) ? _code : -1;
            var name = code > -1 ? null : v;

            var sql = $@"
select top 1 id,name from [keyvalue] where isvalid=1 and type=1 
{(code > -1 ? "and id=@code" : "")} {(!string.IsNullOrEmpty(name) ? "and left(name,2)=left(@name,2)" : "")} {(parentCode == null ? "" : "and parentid=@parentCode")}
";
            var itm = await _schoolDataDB.SlaveConnection.QueryFirstOrDefaultAsync<(long, string)>(sql, new { code, name, parentCode });
            return itm;
        }

        public async Task<IEnumerable<(long Code, string Name)>> GetCityAreas(long code)
        {
            var sql = $@"
select id,name from [keyvalue] where isvalid=1 and type=1 and parentid=@code
";
            var itms = await _schoolDataDB.SlaveConnection.QueryAsync<(long, string)>(sql, new { code });
            return itms;
        }

        public async Task SaveQpaper(DgAyUserQaPaper q, List<DgAyUserQaContent> qaContents)
        {
            using var ctx = new DbTranCtx(_schoolDataDB.Connection);
            try
            {
                ctx.BeginTransaction();

                await ctx.Connection.CommandSet<DgAyUserQaPaper>(ctx.CurrentTransaction).InsertAsync(q);
                await ctx.Connection.CommandSet<DgAyUserQaContent>(ctx.CurrentTransaction).InsertAsync(qaContents);

                ctx.CommitTransaction();
            }
            catch
            {
                ctx.RollBackTransaction();
                throw;
            }
        }

        public async Task UpQpaperAndAnalyzedResult(DgAyUserQaPaper qa, DgAyUserQaResultContent resultContent)
        {
            // 无结果变解锁 fix Title
            if (qa.UnlockedType == (byte)DgAyUnlockedTypeEnum.NoResult)
            {
                var sql = $@"
select top 1 row_number()over(partition by userid,DATEADD(MS,0,DATEADD(DD, DATEDIFF(DD,0,q.UnlockedTime), 0)) order by UnlockedTime asc)as i
from DgAyUserQaPaper q
where 1=1 and q.status=@status and q.IsValid=1
and q.userid=@UserId and DATEDIFF(dd,q.UnlockedTime,@UnlockedTime)=0
order by q.UnlockedTime desc
";
                var i = await _schoolDataDB.Connection.QueryFirstOrDefaultAsync<int?>(sql, new
                {
                    qa.UserId, qa.Status, qa.UnlockedTime,
                }) ?? 0;
                i += 1;

                qa.Title = $"{qa.UnlockedTime:yyyy年MM月dd日}{(i <= 1 ? "" : $"第{i}次")}学位分析结果";
            }

            using var ctx = new DbTranCtx(_schoolDataDB.Connection);
            try
            {
                ctx.BeginTransaction();

                var sql = $@"
update DgAyUserQaPaper set [Title]=@Title,[Status]=@Status,[AnalyzedTime]=@AnalyzedTime,[UnlockedType]=@UnlockedType,[UnlockedTime]=@UnlockedTime 
where [Id]=@Id 
";
                await ctx.Connection.ExecuteAsync(sql, qa, ctx.CurrentTransaction);

                if (resultContent != null)
                {
                    sql = $@"update DgAyUserQaResultContent set IsValid=0 where qaid=@Qaid and IsValid=1 ;";
                    await ctx.Connection.ExecuteAsync(sql, new { resultContent.Qaid }, ctx.CurrentTransaction);

                    await ctx.Connection.CommandSet<DgAyUserQaResultContent>(ctx.CurrentTransaction).InsertAsync(resultContent);
                }

                ctx.CommitTransaction();
            }
            catch
            {
                ctx.RollBackTransaction();
                throw;
            }
        }

        public async Task<Guid> GetDgAyPrimarySchoolByAreaAddress(long area, string address)
        {
            var sql = @"
select top 1 * from DgAyAddressAndPrimarySchool where IsValid=1 and area=@area and address=@address
";
            var itm = await _schoolDataDB.SlaveConnection.QueryFirstOrDefaultAsync<DgAyAddressAndPrimarySchool>(sql, new { area, address });
            return itm?.Eid ?? default;
        }

        public async Task<DgAySchPcyFileDto> GetDgAySchPcyFile(long? area, DgAySchPcyFileTypeEnum type)
        {
            var sql = $@"
select top 1 * from DgAySchPcyFile where IsValid=1 {(area != null ? "and area=@area" : "")} and type=@type
order by [year] desc,CreateTime desc
";
            var itm = await _schoolDataDB.SlaveConnection.QueryFirstOrDefaultAsync<DgAySchPcyFileDto>(sql, new { area, type = (int)type });
            return itm;
        }

        public async Task<Guid[]> FindTop5JfPriSchoolEids(int year, long area, double totalPoint)
        {
            var sql = $@"
select top 5 e3.eid --,e3.*
from OnlineSchoolRecruitInfo e3
left join OnlineSchoolExtension e on e3.eid=e.id and e.isvalid=1 
left join OnlineSchool s on s.id=e.sid and s.isvalid=1 and s.status=3
left join OnlineSchoolExtContent e2 on e3.eid=e2.eid and e2.isvalid=1 
where 1=1 and e.grade=@grade and e3.type=3 and area=@area
and e3.year=@year and (@totalPoint-30)<=e3.IntegralAdmitLevel and e3.IntegralAdmitLevel<=@totalPoint 
order by e3.IntegralAdmitLevel desc
";
            // e3.type=3 非户籍生积分入学
            var ls = await _schoolDataDB.SlaveConnection.QueryAsync<Guid>(sql, new { year, totalPoint, area, grade = (int)SchoolGradeType.PrimarySchool });
            return ls.ToArray();
        }

        public async Task<Guid[]> Find3kmOvPriSchoolEids(double lng, double lat, long? area)
        {
            var sql = $@"
select e2.eid
from OnlineSchoolExtContent e2
join OnlineSchoolExtension e on e2.eid=e.id 
join OnlineSchool s on s.id=e.sid
where e.isvalid=1 and s.isvalid=1 and e2.IsValid=1 and s.status=3 and e.schftype='lx210' {(area == null ? "" : "and e2.area=@area")}
and geography::Point({lat},{lng},4326).STDistance(e2.LatLong)<=3000 
and geography::Point({lat},{lng},4326).STDistance(e2.LatLong)>=-3000 
";
            var ls = await _schoolDataDB.SlaveConnection.QueryAsync<Guid>(sql, new { area, grade = (int)SchoolGradeType.PrimarySchool });
            return ls.ToArray();
        }

        public async Task<(Guid[] Counterpart, Guid[] Allocation)> FindCpPcAssignAndHeliSchoolEids(Guid eid, int? year)
        {
            // 查年份数据
            if (year != null)
            {
                try 
                {
                    var sql = $@"
select field,content from OnlineSchoolYearFieldContent_{year} where IsValid=1 and eid=@eid and field in('Allocation', 'Counterpart')
";
                    var ls = await _schoolDataDB.SlaveConnection.QueryAsync<(string F, string Ctn)>(sql, new { eid });
                    return (
                        (ls.FirstOrDefault(_ => _.F == "Counterpart").Ctn?.FromJsonSafe<KeyValuePair<string, Guid>[]>() ?? new KeyValuePair<string, Guid>[0]).Select(_ => _.Value).ToArray(),
                        (ls.FirstOrDefault(_ => _.F == "Allocation").Ctn?.FromJsonSafe<KeyValuePair<string, Guid>[]>() ?? new KeyValuePair<string, Guid>[0]).Select(_ => _.Value).ToArray()
                    );
                }
                catch
                {
                    year = null;
                }
            }
            // 查最新数据
            if (year == null)
            {
                var sql = $@"select top 1 Counterpart,Allocation from OnlineSchoolExtContent where isvalid=1 and eid=@eid ";
                var itm = await _schoolDataDB.SlaveConnection.QueryFirstOrDefaultAsync<(string C, string A)>(sql, new { eid });
                if (itm == default) return default;
                return (
                    (itm.C.FromJsonSafe<KeyValuePair<string, Guid>[]>() ?? new KeyValuePair<string, Guid>[0]).Select(_ => _.Value).ToArray(),
                    (itm.A.FromJsonSafe<KeyValuePair<string, Guid>[]>() ?? new KeyValuePair<string, Guid>[0]).Select(_ => _.Value).ToArray()
                );
            }
            return default;
        }

        public async Task<Guid[]> FindMbPriSchoolEids(IEnumerable<(string FindField, string FindFieldFw, string FindFieldFwJx)> conditions)
        {
            var sql = $@"
select top 5 e.id as eid
from OnlineSchoolExtension e 
left join OnlineSchool s on e.sid=s.id and s.Status=3
left join SchoolViewALevel a on e.id=a.id
{{leftjoin}}
where e.IsValid=1 and s.IsValid=1 and e.grade={(int)SchoolGradeType.PrimarySchool} and e.type={(int)SchoolType.Private}
{{where}}
order by (case when a.id is not null then 1 else 0 end) desc
";
            var s_leftjoin = new HashSet<string>();
            var where = new Dictionary<string, List<string>>();
            foreach (var condition in conditions)
            {
                var cdd = "";
                switch (condition.FindField)
                {
                    case "区":
                        {
                            s_leftjoin.Add("left join OnlineSchoolExtContent e2 on e2.IsValid=1 and e2.eid=e.id");
                            cdd = condition.FindFieldFwJx.Replace("{tb}", "e2");
                        }
                        break;
                    case "走读寄宿":
                        {
                            s_leftjoin.Add("left join OnlineSchoolExtContent e2 on e2.IsValid=1 and e2.eid=e.id");
                            cdd = condition.FindFieldFwJx.Replace("{tb}", "e2");
                        }
                        break;
                    case "学费":
                        {
                            s_leftjoin.Add("left join OnlineSchoolExtCharge e5 on e5.IsValid=1 and e5.eid=e.id");
                            cdd = condition.FindFieldFwJx.Replace("{tb}", "e5");
                        }
                        break;
                    default:
                        continue;
                }
                if (!string.IsNullOrEmpty(cdd))
                {
                    var s_where = where.TryGetValue(condition.FindField, out var _where) ? _where : new List<string>();
                    s_where.Add($"({cdd})");
                    where[condition.FindField] = s_where;
                }
            }

            sql = sql.Replace("{leftjoin}", string.Join("\n", s_leftjoin));
            sql = sql.Replace("{where}", string.Join("\n", where.Select(x => 
                "and ( " + string.Join(" or ", x.Value) + " )"
            )));
            
            var ls = await _schoolDataDB.SlaveConnection.QueryAsync<Guid>(sql);
            return ls.ToArray();
        }
    }
}
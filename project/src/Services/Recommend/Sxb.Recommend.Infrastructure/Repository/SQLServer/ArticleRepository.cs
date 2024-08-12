using Dapper;
using Newtonsoft.Json.Linq;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Value;
using Sxb.Recommend.Infrastructure.IRepository;
using Sxb.Recommend.Infrastructure.Repository.SQLServer.DataBases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.Repository.SQLServer
{
    public class ArticleRepository : IArticleRepository
    {
        DataBase _db;
        public ArticleRepository(ISchoolArticleDB iSchoolArticleDB)
        {
            _db = iSchoolArticleDB;
        }

        public async Task<IEnumerable<Article>> GetArticles(DateTime afterTime, int offset = 0, int limit = 100)
        {
            string sql = @"SELECT id,[type],[time],[IsDeleted],[show] INTO #TEMP FROM article
WHERE  updateTime > @afterTime
order by createTime, id
offset @offset rows 
fetch next @limit rows only
SELECT  article.id
,article.[type]
,(SELECT tagID FROM tag_bind WHERE dataID = article.id AND dataType=1 FOR JSON PATH) tags
,(SELECT SchoolId FROM Article_SchoolBind WHERE ArticleId=article.id  FOR JSON PATH) schools
,(SELECT ProvinceId,CityId,AreaId FROM Article_Areas WHERE ArticleId = article.id  FOR JSON PATH) deployAreaInfos
,(SELECT SchoolTypeId FROM Article_SchoolTypeBinds  WHERE ArticleId = article.id GROUP BY SchoolTypeId FOR JSON PATH) schoolTypes
,article.[time]
,article.[IsDeleted]
,article.[show]
FROM #TEMP article
";
            var datas = await _db.Connection.QueryAsync(sql, new { afterTime, offset, limit });
            return datas.Select(s =>
             {
                 List<Guid> tags = new List<Guid>();
                 if (!string.IsNullOrEmpty(s.tags))
                 {
                     tags.AddRange(JArray.Parse((string)s.tags).Select(t => Guid.Parse(t["tagID"].Value<string>())));
                 }
                 List<Guid> schools = new List<Guid>();
                 if (!string.IsNullOrEmpty(s.schools))
                 {
                     schools.AddRange(JArray.Parse((string)s.schools).Select(t => Guid.Parse(t["SchoolId"].Value<string>())));
                 }
                 List<DeployAreaInfo> deployAreaInfos = new List<DeployAreaInfo>();
                 if (!string.IsNullOrEmpty(s.deployAreaInfos))
                 {
                     deployAreaInfos.AddRange(JArray.Parse((string)s.deployAreaInfos).Select(t =>
                     {
                         var provinceId = t["ProvinceId"]?.Value<int>();
                         var cityId = t["CityId"]?.Value<int?>();
                         var areaId = t["AreaId"]?.Value<int?>();
                         return new DeployAreaInfo() { Province = provinceId.GetValueOrDefault(), City = cityId.GetValueOrDefault(), Area = areaId.GetValueOrDefault() };
                     }));
                 }
                 List<int> schoolTypes = new List<int>();
                 if (!string.IsNullOrEmpty(s.schoolTypes))
                 {
                     schoolTypes.AddRange(JArray.Parse((string)s.schoolTypes).Select(t => t["SchoolTypeId"].Value<int>()));
                 }
                 return new Article(
                         s.id,
                         s.type ?? 0,
                         s.time,
                         tags,
                         schools,
                         deployAreaInfos,
                         schoolTypes,
                         s.IsDeleted,
                         s.show
                     );

             });
        }


        public async Task<(Guid id1, Guid id2)> GetIdByNo(long no1, long no2)
        {
            var sql = $@" 
select Id from article where no = @no1
select Id from article where no = @no2
";
            using var reader = await _db.Connection.QueryMultipleAsync(sql, new { no1, no2 });
            var id1 = await reader.ReadFirstOrDefaultAsync<Guid>();
            var id2 = await reader.ReadFirstOrDefaultAsync<Guid>();
            return (id1, id2);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using Polly;
using Newtonsoft.Json.Linq;

namespace Sxb.Static.BackgroundTask.Application.Queries
{
    public class ArticleQueries : IArticleQueries
    {
        private readonly string _connectionString;
        public ArticleQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString)); ;
        }

        public async Task<Article> GetArticleFromNoAsync(long no)
        {
            using (IDbConnection connection = new SqlConnection(this._connectionString))
            {
                Article article = await Policy.Handle<Exception>()
                .WaitAndRetryAsync(new[] {
                 TimeSpan.FromSeconds(1),
                 TimeSpan.FromSeconds(5)
                }, (exception, ts) =>
                {
                    Console.WriteLine("处理错误重试:" + exception.Message);
                })
                .ExecuteAsync(async () =>
                {
                    var articleDynamic = await connection.QueryFirstOrDefaultAsync<dynamic>(@"
SELECT
article.id 
,(SELECT ProvinceId,CityId,AreaId FROM Article_Areas WHERE ArticleId = article.id  FOR JSON PATH) deployAreaInfos
FROM article
WHERE No = @no
", new { no = no });
                    if (articleDynamic != null)
                    {
                        List<DeployAreaInfo> deployAreaInfos = new List<DeployAreaInfo>();
                        if (!string.IsNullOrEmpty(articleDynamic.deployAreaInfos))
                        {
                            deployAreaInfos.AddRange(JArray.Parse((string)articleDynamic.deployAreaInfos).Select(t =>
                            {
                                var provinceId = t["ProvinceId"]?.Value<int>();
                                var cityId = t["CityId"]?.Value<int?>();
                                var areaId = t["AreaId"]?.Value<int?>();
                                return new DeployAreaInfo() { Province = provinceId.GetValueOrDefault(), City = cityId.GetValueOrDefault(), Area = areaId.GetValueOrDefault() };
                            }));
                        }
                        return new Article()
                        {
                            Id = articleDynamic.id,
                            DeployAreaInfo = deployAreaInfos
                        };
                    }
                    else {
                        return null;
                    }

                });

                if (article == null)
                    throw new KeyNotFoundException();
                return article;
            }

        }
    }
}

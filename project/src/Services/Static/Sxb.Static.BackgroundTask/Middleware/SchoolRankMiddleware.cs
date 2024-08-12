using MongoDB.Bson;
using MongoDB.Driver;
using Sxb.Framework.Foundation;
using Sxb.Static.BackgroundTask.Application.Queries;
using Sxb.Static.BackgroundTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Middleware
{
    public class SchoolRankMiddleware : IMiddleware
    {
        public async Task InvokeAsync(KafkaContext context, Func<KafkaContext, Task> next)
        {
            var kafkalog = context.AsKafkaLog();
            Regex schoolRankRegex = new Regex("(?<=sxkid.com/schoolrank/detail/)[a-zA-Z0-9]+(?=.html)", RegexOptions.IgnoreCase);
            if (kafkalog.Url != null && schoolRankRegex.IsMatch(kafkalog.Url))
            {
                ISchoolRankQueries schoolRankQueries = context.Services.GetService(typeof(ISchoolRankQueries)) as ISchoolRankQueries;
                IMongoClient mongoClient = context.Services.GetService(typeof(IMongoClient)) as IMongoClient;
                string shortId = schoolRankRegex.Match(kafkalog.Url).Value;
                var schoolRank = await schoolRankQueries.GetSchoolRankFromNoAsync(UrlShortIdUtil.Base322Long(shortId));
                var collection = mongoClient.GetDatabase("SxbStatic").GetCollection<SchoolRankLog>("SchoolRankLogs");
                await collection.InsertOneAsync(new SchoolRankLog
                {
                    Id = ObjectId.GenerateNewId(),
                    DataId = schoolRank.Id,
                    Citys = schoolRank.Citys,
                    CreateTime = DateTime.Now,
                    DeviceId = kafkalog.DeviceId,
                    UserId = kafkalog.UserId

                });
                Console.WriteLine("学校榜单：{0}", schoolRank.Id);

            }
            else
            {
                await next(context);
            }
        }
    }
}

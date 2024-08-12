using Confluent.Kafka;
using MongoDB.Bson;
using MongoDB.Driver;
using Sxb.Framework.Foundation;
using Sxb.Static.BackgroundTask.Application.Queries;
using Sxb.Static.BackgroundTask.Infrastucture;
using Sxb.Static.BackgroundTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace Sxb.Static.BackgroundTask.Middleware
{
    public class SchoolMiddleware : IMiddleware
    {
        static readonly object _lock = new object();
        static List<SchoolLog> schoolLogs = new List<SchoolLog>();
        int cacheCount = 20;
        public async Task InvokeAsync(KafkaContext context, Func<KafkaContext, Task> next)
        {
            var kafkalog = context.AsKafkaLog();
            Regex schoolRegex = new Regex("(?<=sxkid.com/school-)[a-zA-Z0-9]+", RegexOptions.IgnoreCase);
            if (kafkalog.Url != null && schoolRegex.IsMatch(kafkalog.Url))
            {
                ISchoolQueries schoolQueries = context.Services.GetService(typeof(ISchoolQueries)) as ISchoolQueries;
                StaticDbContext dbContext = context.Services.GetService<StaticDbContext>();
                string shortId = schoolRegex.Match(kafkalog.Url).Value;
                var school = await schoolQueries.GetSchoolFromNoAsync(UrlShortIdUtil.Base322Long(shortId));
                lock(_lock){
                    schoolLogs.Add(new SchoolLog
                    {
                        Id = ObjectId.GenerateNewId(),
                        DataId = school.ExtId,
                        Province = school.Province,
                        City = school.City,
                        Area = school.Area,
                        CreateTime = DateTime.Now,
                        DeviceId = kafkalog.DeviceId,
                        UserId = kafkalog.UserId

                    });
                    if (schoolLogs.Count >= cacheCount)
                    {
                        dbContext.SchoolLogs.AddRange(schoolLogs);
                        var res = dbContext.SaveChangesAsync().Result;
                        if (res > 0) {
                            schoolLogs.Clear();
                        }
                        Console.WriteLine("学校已满{0}条，插入。", cacheCount);
                    }
                }

            }
            else
            {
                await next(context);
            }
        }
    }
}

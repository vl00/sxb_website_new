using Confluent.Kafka;
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
    public class CourseMiddleware : IMiddleware
    {


        public async Task InvokeAsync(KafkaContext context, Func<KafkaContext, Task> next)
        {
            var kafkalog = context.AsKafkaLog();
            List<Regex> regices = new List<Regex>() {
              new Regex("(?<=/pagesA/pages/course_detail/course_detail\\?id=)[a-zA-Z0-9-]+", RegexOptions.IgnoreCase),
              new Regex("(?<=sxkid.com/org/course/detail/)[a-zA-Z0-9]+", RegexOptions.IgnoreCase),
             };
            foreach (var regex in regices)
            {
                if (kafkalog.Url != null && regex.IsMatch(kafkalog.Url))
                {
                    string shortId = regex.Match(kafkalog.Url).Value;
                    ICourseQueries courseQueries = context.Services.GetService(typeof(ICourseQueries)) as ICourseQueries;
                    IMongoClient mongoClient = context.Services.GetService(typeof(IMongoClient)) as IMongoClient;
                    var course = await courseQueries.GetCourseFromNoAsync(UrlShortIdUtil.Base322Long(shortId));
                    var collection = mongoClient.GetDatabase("SxbStatic").GetCollection<CourseLog>("CourseLogs");
                    await collection.InsertOneAsync(new CourseLog
                    {
                        Id = ObjectId.GenerateNewId(),
                        DataId = course.Id,
                        Type = course.Type,
                        CreateTime = DateTime.Now,
                        DeviceId = kafkalog.DeviceId,
                        UserId = kafkalog.UserId

                    });
                    Console.WriteLine("商品：{0}", course.Id);
                    return;
                }

            }
            await next(context);
        }
    }
}

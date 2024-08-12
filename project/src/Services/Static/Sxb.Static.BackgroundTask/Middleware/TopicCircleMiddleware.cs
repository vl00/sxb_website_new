using MongoDB.Bson;
using MongoDB.Driver;
using Sxb.Static.BackgroundTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Middleware
{
    public class TopicCircleMiddleware : IMiddleware
    {
        public async Task InvokeAsync(KafkaContext context, Func<KafkaContext, Task> next)
        {
            var kafkalog = context.AsKafkaLog();
            List<Regex> regices = new List<Regex>()
            {
                new Regex("(?<=sxkid.com/topic/topicNew/circleId=)[a-zA-Z0-9-]+",RegexOptions.IgnoreCase),
                new Regex("(?<=sxkid.com/topic/circle-default.html\\?circleId=)[a-zA-Z0-9-]+",RegexOptions.IgnoreCase),
                new Regex("(?<=sxkid.com/topic/subject-details.html\\?circleId=)[a-zA-Z0-9-]+",RegexOptions.IgnoreCase),
                new Regex("(?<=sxkid.com/topic/subject-quit.html\\?circleId=)[a-zA-Z0-9-]+",RegexOptions.IgnoreCase),
             };
            foreach (var regex in regices)
            {
                if (kafkalog.Url != null && regex.IsMatch(kafkalog.Url))
                {
                    //IArticleQueries articleQueries = context.Services.GetService(typeof(IArticleQueries)) as IArticleQueries;
                    IMongoClient mongoClient = context.Services.GetService(typeof(IMongoClient)) as IMongoClient;
                    string id = regex.Match(kafkalog.Url).Value;
                    if (Guid.TryParse(id, out Guid gid))
                    {
                        var collection = mongoClient.GetDatabase("SxbStatic").GetCollection<TopicLog>("TopicLogs");
                        await collection.InsertOneAsync(new TopicLog
                        {
                            Id = ObjectId.GenerateNewId(),
                            DataId = gid,
                            CreateTime = DateTime.Now,
                            DeviceId = kafkalog.DeviceId,
                            UserId = kafkalog.UserId

                        });
                        Console.WriteLine("话题圈：{0}", id);
                    }
                    return;

                }
            }
            await next(context);

        }
    }
}

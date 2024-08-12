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
    public class LiveMiddleware : IMiddleware
    {


        public async Task InvokeAsync(KafkaContext context, Func<KafkaContext, Task> next)
        {
            var kafkalog = context.AsKafkaLog();
            List<Regex> regices = new List<Regex>()
            {
               new Regex("(?<=sxkid.com/live/client/liveroom.html.*lectureid=)[a-zA-Z0-9-]+", RegexOptions.IgnoreCase),
               new Regex("(?<=sxkid.com/live/client/livedetail.html.*contentid=)[a-zA-Z0-9-]+", RegexOptions.IgnoreCase)
            };
            foreach (var regex in regices)
            {
                if (kafkalog.Url != null && regex.IsMatch(kafkalog.Url))
                {
                    ILiveQueries liveQueries = context.Services.GetService(typeof(ILiveQueries)) as ILiveQueries;
                    IMongoClient mongoClient = context.Services.GetService(typeof(IMongoClient)) as IMongoClient;
                    string id = regex.Match(kafkalog.Url).Value;
                    var live = await liveQueries.GetLiveFromIdAsync(Guid.Parse(id));
                    var collection = mongoClient.GetDatabase("SxbStatic").GetCollection<LiveLog>("LiveLogs");
                    await collection.InsertOneAsync(new LiveLog
                    {
                        Id = ObjectId.GenerateNewId(),
                        DataId = live.Id,
                        City = live.City,
                        CreateTime = DateTime.Now,
                        DeviceId = kafkalog.DeviceId,
                        UserId = kafkalog.UserId

                    });
                    Console.WriteLine("直播：{0}", id);
                    return;

                }
            }

            await next(context);
        }
    }
}

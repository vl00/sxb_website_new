using MongoDB.Bson;
using MongoDB.Driver;
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
    public class TalentMiddleware : IMiddleware
    {
        public async Task InvokeAsync(KafkaContext context, Func<KafkaContext, Task> next)
        {
            var kafkalog = context.AsKafkaLog();
            Regex regex = new Regex("(?<=sxkid.com/mine/mine-msg/\\?id=)[a-zA-Z0-9-]+", RegexOptions.IgnoreCase);
            if (kafkalog.Url != null && regex.IsMatch(kafkalog.Url))
            {
                string id = regex.Match(kafkalog.Url).Value;
                ITalentQueries talentQueries = context.Services.GetService(typeof(ITalentQueries)) as ITalentQueries;
                IMongoClient mongoClient = context.Services.GetService(typeof(IMongoClient)) as IMongoClient;
                var talent = await talentQueries.GetTalentFromUserIdAsync(Guid.Parse(id));
                var collection = mongoClient.GetDatabase("SxbStatic").GetCollection<TalentLog>("TalentLogs");
                await collection.InsertOneAsync(new TalentLog
                {
                    Id = ObjectId.GenerateNewId(),
                    DataId = talent.Id,
                    City = talent.City,
                    CreateTime = DateTime.Now,
                    DeviceId = kafkalog.DeviceId,
                    UserId = kafkalog.UserId

                });
                Console.WriteLine("达人：{0}", id);

            }
            else
            {
                await next(context);
            }
        }
    }
}

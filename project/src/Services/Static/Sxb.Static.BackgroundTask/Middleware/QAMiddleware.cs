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
    public class QAMiddleware : IMiddleware
    {
        public async Task InvokeAsync(KafkaContext context, Func<KafkaContext, Task> next)
        {
            var kafkalog = context.AsKafkaLog();
            Regex regex = new Regex("(?<=sxkid.com/question/)[a-zA-Z0-9]+(?=.html)", RegexOptions.IgnoreCase);
            if (kafkalog.Url != null && regex.IsMatch(kafkalog.Url))
            {
                IQuestionQueries questionQueries = context.Services.GetService(typeof(IQuestionQueries)) as IQuestionQueries;
                IMongoClient mongoClient = context.Services.GetService(typeof(IMongoClient)) as IMongoClient;
                string shortId = regex.Match(kafkalog.Url).Value;
                var question = await questionQueries.GetQuestionFromNoAsync(UrlShortIdUtil.Base322Long(shortId));
                var collection = mongoClient.GetDatabase("SxbStatic").GetCollection<QuestionLog>("QuestionLogs");
                await collection.InsertOneAsync(new QuestionLog
                {
                    Id = ObjectId.GenerateNewId(),
                    DataId = question.Id,
                    Province = question.Province,
                    City = question.City,
                    Area = question.Area,
                    CreateTime = DateTime.Now,
                    DeviceId = kafkalog.DeviceId,
                    UserId = kafkalog.UserId

                });
                Console.WriteLine("问答：{0}", question.Id);

            }
            else
            {
                await next(context);
            }
        }
    }
}

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
    public class EvaluationMiddleware : IMiddleware
    {


        public async Task InvokeAsync(KafkaContext context, Func<KafkaContext, Task> next)
        {
            var kafkalog = context.AsKafkaLog();
            Regex mpRegex = new Regex("(?<=/pagesPlant/pages/plant-detail/index\\?id=)[a-zA-Z0-9-]+", RegexOptions.IgnoreCase);
            Regex h5Regex = new Regex("(?<=sxkid.com/org/evaluation/detail/)[a-zA-Z0-9-]+", RegexOptions.IgnoreCase);
            if (kafkalog.Url != null)
            {
                Guid? evaluationId = null;
                if (mpRegex.IsMatch(kafkalog.Url))
                {
                    string id = mpRegex.Match(kafkalog.Url).Value;
                    evaluationId = Guid.Parse(id);
                }
                else if (h5Regex.IsMatch(kafkalog.Url))
                {
                    string shortId = h5Regex.Match(kafkalog.Url).Value;
                    IEvaluationQueries evaluationQueries = context.Services.GetService(typeof(IEvaluationQueries)) as IEvaluationQueries;
                    var evaluation = await evaluationQueries.GetEvaluationFromNoAsync(UrlShortIdUtil.Base322Long(shortId));
                    evaluationId = evaluation.Id;
                }
                if (evaluationId != null)
                {
                    IMongoClient mongoClient = context.Services.GetService(typeof(IMongoClient)) as IMongoClient;
                    var collection = mongoClient.GetDatabase("SxbStatic").GetCollection<EvaluationLog>("EvaluationLogs");
                    await collection.InsertOneAsync(new EvaluationLog
                    {
                        Id = ObjectId.GenerateNewId(),
                        DataId = evaluationId.Value,
                        CreateTime = DateTime.Now,
                        DeviceId = kafkalog.DeviceId,
                        UserId = kafkalog.UserId

                    });


                    Console.WriteLine("评测：{0}", evaluationId);
                    return;
                }
            }
            await next(context);
        }
    }
}

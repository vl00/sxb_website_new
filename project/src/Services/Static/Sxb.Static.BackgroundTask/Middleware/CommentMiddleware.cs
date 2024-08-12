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
    public class CommentMiddleware : IMiddleware
    {
        public async Task InvokeAsync(KafkaContext context, Func<KafkaContext, Task> next)
        {

            var kafkalog = context.AsKafkaLog();
            Regex regex = new Regex("(?<=sxkid.com/comment/)[a-zA-Z0-9]+(?=.html)", RegexOptions.IgnoreCase);
            if (kafkalog.Url != null && regex.IsMatch(kafkalog.Url))
            {
                ICommentQueries  commentQueries = context.Services.GetService(typeof(ICommentQueries)) as ICommentQueries;
                IMongoClient mongoClient = context.Services.GetService(typeof(IMongoClient)) as IMongoClient;
                string shortId = regex.Match(kafkalog.Url).Value;
                var comment = await commentQueries.GetCommentFromNoAsync(UrlShortIdUtil.Base322Long(shortId));
                var collection = mongoClient.GetDatabase("SxbStatic").GetCollection<CommentLog>("CommentLogs");
                await collection.InsertOneAsync(new CommentLog
                {

                    Id = ObjectId.GenerateNewId(),
                    DataId = comment.Id,
                    Province = comment.Province,
                    City = comment.City,
                    Area = comment.Area,
                    CreateTime = DateTime.Now,
                    DeviceId = kafkalog.DeviceId,
                    UserId = kafkalog.UserId
                });
                Console.WriteLine("点评：{0}", comment.Id);

            }
            else
            {
                await next(context);
            }
        }
    }
}

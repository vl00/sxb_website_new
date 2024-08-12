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
    public class ArticleMiddleware : IMiddleware
    {
        static readonly object _lock = new object();
        static List<ArticleLog> ArticleLogs = new List<ArticleLog>();
        int cacheCount = 20;
        public async Task InvokeAsync(KafkaContext context, Func<KafkaContext, Task> next)
        {
            var kafkalog = context.AsKafkaLog();
            Regex articleRegex = new Regex("(?<=sxkid.com/article/)[a-zA-Z0-9]+(?=.html)", RegexOptions.IgnoreCase);
            if (kafkalog.Url != null && articleRegex.IsMatch(kafkalog.Url))
            {
                IArticleQueries articleQueries = context.Services.GetService<IArticleQueries>();
                StaticDbContext dbContext = context.Services.GetService<StaticDbContext>();
                string shortId = articleRegex.Match(kafkalog.Url).Value;
                var article = await articleQueries.GetArticleFromNoAsync(UrlShortIdUtil.Base322Long(shortId));
                lock (_lock)
                {
                    ArticleLogs.Add(new ArticleLog
                    {
                        Id = ObjectId.GenerateNewId(),
                        DataId = article.Id,
                        DeployAreaInfo = article.DeployAreaInfo,
                        CreateTime = DateTime.Now,
                        DeviceId = kafkalog.DeviceId,
                        UserId = kafkalog.UserId

                    });
                    if (ArticleLogs.Count > cacheCount)
                    {
                        dbContext.ArticleLogs.AddRange(ArticleLogs);
                        var res = dbContext.SaveChangesAsync().Result;
                        if (res > 0)
                        {
                            ArticleLogs.Clear();
                        }
                        Console.WriteLine("文章已满{0}条，插入。", cacheCount);
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

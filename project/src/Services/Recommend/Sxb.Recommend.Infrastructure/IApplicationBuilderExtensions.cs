using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Sxb.Recommend.Infrastructure.IRepository;
using Sxb.Recommend.Infrastructure.Repository.MongoDB;
using Sxb.Recommend.Domain.Entity;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSxbRecommendMongoDB(this IApplicationBuilder app)
        {

            using (var scope = app.ApplicationServices.CreateScope())
            {
                //初始化部分mongodb集合，因为这些表使用了事务实现upsert功能。事务的前提必须有个集合。
                (scope.ServiceProvider.GetService<ISchoolMapRepository>() as IMongoRepository<SchoolMap>)?.InitCollection();
                (scope.ServiceProvider.GetService<IArticleMapRepository>() as IMongoRepository<ArticleMap>)?.InitCollection();
                (scope.ServiceProvider.GetService<IArticleRedirectFrequencyRepository>() as IMongoRepository<ArticleRedirectFrequency>)?.InitCollection();
                (scope.ServiceProvider.GetService<ISchoolRedirectFrequencyRepository>() as IMongoRepository<SchoolRedirectFrequency>)?.InitCollection();

            }


            return app;
        }
    }
}

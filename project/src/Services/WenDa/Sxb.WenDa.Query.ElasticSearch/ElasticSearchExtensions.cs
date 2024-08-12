using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Framework.SearchAccessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Query.ElasticSearch
{
    public static class ElasticSearchExtensions
    {
        //ES
        public static IServiceCollection AddElasticSearch(this IServiceCollection services, 
            IConfiguration configuration,  
            string section = "SearchConfig")
        {
            services.Configure<EsIndexConfig>(configuration.GetSection(section));

            services.AddScopedSearchAccessor(config =>
            {
                var searchConfig = configuration.GetSection(section).Get<SearchConfig>();
                config.ServerUrl = searchConfig.ServerUrl;
                config.DefultIndexName = searchConfig.DefultIndexName;
            });

            services.RegisterRepository();
            return services;
        }

        private static void RegisterRepository(this IServiceCollection services)
        {
            services.AddScoped<IQuestionEsRepository, QuestionEsRepository>();
            services.AddScoped<IAnswerEsRepository, AnswerEsRepository>();
            services.AddScoped<ISubjectEsRepository, SubjectEsRepository>();
        }
    }
}

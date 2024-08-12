using Newtonsoft.Json;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.ArticleMajor.Common.QueryDto;
using Sxb.ArticleMajor.Query.Mongodb;
using Sxb.Framework.Foundation.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Runner.DynamicImage
{
    internal partial class RemoveDuplicationRunner : BaseRunner<RemoveDuplicationRunner>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IArticleContentRepository _articleContentRepository;

        public RemoveDuplicationRunner(IArticleRepository articleRepository, IArticleContentRepository articleContentRepository)
        {
            _articleRepository = articleRepository;
            _articleContentRepository = articleContentRepository;
        }

        protected override void Running()
        {
            var duplications = _articleRepository.GetArticleDuplications(0);
            WriteToFile($"./duplicate-fromurl-article-{Guid.NewGuid():N}.txt", duplications);

            Remove(duplications);
        }


        public void Remove(IEnumerable<ArticleDuplication> duplications)
        {
            var filename = $"./duplicate-fromurl-article-delete-{Guid.NewGuid():N}.txt";
            using StreamWriter inputStream = File.CreateText(filename);

            new BatchHelper.BatchBuilder<ArticleDuplication>(10000000, 10000)
                .From(duplications)
                .Handle(async items =>
                {
                    //重复的数据保留第一个
                    var ids = items.SelectMany(s => s.Ids.Skip(1)).ToArray();
                    inputStream.WriteLine(JsonConvert.SerializeObject(ids));

                    await _articleRepository.DeleteAsync(ids);
                    await _articleContentRepository.DeleteByArticleIds(ids);
                })
                .Build()
                .Run();


            inputStream.Flush();
            inputStream.Close();
        }
    }
}

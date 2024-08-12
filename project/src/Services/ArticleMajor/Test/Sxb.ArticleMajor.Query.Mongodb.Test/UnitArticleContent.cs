using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using NUnit.Framework;
using ProductManagement.Framework.MongoDb;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.Framework.Foundation.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb.Test
{
    public class UnitArticleContent : UnitBase
    {
        IArticleContentRepository _articleContentRepository;
        IArticleRepository _articleRepository;

        string TestId = ObjectId.GenerateNewId().ToString();
        //Guid TestId = Guid.NewGuid();
        [SetUp]
        public void Setup()
        {
            _articleContentRepository = ServiceProvider.GetService<IArticleContentRepository>();
            _articleRepository = ServiceProvider.GetService<IArticleRepository>();
        }


        [Test]
        public void TestAddManySuccess()
        {
            var entities = new List<ArticleContent>() {
                new ArticleContent()
                {
                    ArticleId = ObjectId.GenerateNewId().ToString(),
                    Content = "测试文章第一页",
                    Sort = 0
                },
                new ArticleContent()
                {
                    ArticleId = ObjectId.GenerateNewId().ToString(),
                    Content = "测试文章第二页",
                    Sort = 1
                }
            };
            _articleContentRepository.AddAsync(entities).GetAwaiter().GetResult();
            Assert.Pass();
        }


        [Test]
        public void TestGetSuccess()
        {
            var articleId = "62218a3fa9e7007c9260c460";
            var data = _articleContentRepository.GetAsync(articleId, 0).GetAwaiter().GetResult();
            WriteLine(data);
            Assert.IsNotNull(data);
        }



        [Test]
        public void TestGetArticleIdsSuccess()
        {
            var data = _articleContentRepository.GetArticleIds(1, 10).GetAwaiter().GetResult();
            WriteLine(data);
            Assert.IsNotNull(data);
        }

        [Test]
        public void GetNoArticleIdData()
        {
            var filename = $"./files/non-article-id-{Guid.NewGuid():N}.txt";
            var dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            using StreamWriter inputStream = File.CreateText(filename);
            //var bom = Encoding.Default.GetBytes($"\uFEFF");
            //inputStream.Write(bom);


            int pageSize = 10000;
            string lastArticleId = "";
            //lastArticleId = "62303f16bdc17a52f0b51a63";
            new BatchHelper.BatchBuilder<string>(10000000, 10000)
                .From(async pageIndex =>
                {
                    //if (pageIndex > 3) return default;

                    var entities = await _articleContentRepository.GetArticleIds(lastArticleId, pageSize);
                    lastArticleId = entities.Count != 0 ? entities[^1] : string.Empty;

                    return entities.Distinct();
                })
                .Handle(async articleIds =>
                {
                    var existIds = await _articleRepository.ExistsIdsAsync(articleIds.ToArray());
                    var nonExistIds = articleIds.Where(s => !existIds.Contains(s));

                    //await _articleContentRepository.DeleteByArticleIds(nonExistIds.ToArray());
                    foreach (var id in nonExistIds)
                    {
                        inputStream.WriteLine(id);
                    }
                })
                .Build()
                .Run();


            inputStream.Flush();
            inputStream.Close();

            WriteLine(filename);
        }

        //[Test]
        public async Task DeleteByFileAsync()
        {
            var filename = $"./files/non-article-id-b01523cd4e434eaca367fba950b46146.txt";

            int i = 0;
            int batchSize = 2000;
            List<string> articleIds = new(batchSize);

            using StreamReader reader = File.OpenText(filename);
            string line;
            while ((line = reader.ReadLine()) != null && line != string.Empty && i++ < 100000000)
            {
                articleIds.Add(line);
                if (articleIds.Count >= batchSize)
                {
                    await _articleContentRepository.DeleteByArticleIds(articleIds.ToArray());
                    articleIds.Clear();
                }
            }

            //尾数据
            await _articleContentRepository.DeleteByArticleIds(articleIds.ToArray());
            WriteLine($"删除{i}个数据");
        }
    }
}
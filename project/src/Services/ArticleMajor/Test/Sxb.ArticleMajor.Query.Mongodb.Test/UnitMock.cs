using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using ProductManagement.Framework.MongoDb;
using Sxb.ArticleMajor.Common.MongoEntity;
using System;
using System.Collections.Generic;

namespace Sxb.ArticleMajor.Query.Mongodb.Test
{
    public class UnitMock : UnitBase
    {
        IArticleContentRepository _articleContentRepository;

        string TestId = ObjectId.GenerateNewId().ToString();
        [SetUp]
        public void Setup()
        {
            var config = new Mock<MongoDbConfig>();
            var options = new Mock<MongoConfigOptions>(config);
            var mongoService = new Mock<IMongoService>();
            _articleContentRepository = new Mock<IArticleContentRepository>().Object;
        }


        [Test]
        public void TestAddManySuccess()
        {
            var entities = new List<ArticleContent>() {
                new ArticleContent()
                {
                    ArticleId = TestId,
                    Content = "测试文章第一页",
                    Sort = 0
                },
                new ArticleContent()
                {
                    ArticleId = TestId,
                    Content = "测试文章第二页",
                    Sort = 1
                }
            };
            _articleContentRepository.AddAsync(entities);
            Assert.Pass();
        }


        [Test]
        public void TestGetSuccess()
        {
            var articleId = TestId;
            var data = _articleContentRepository.GetAsync(articleId, 0).GetAwaiter().GetResult();
            WriteLine(data);
            Assert.IsNotNull(data);
        }
    }
}
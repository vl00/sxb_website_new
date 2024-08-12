using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using ProductManagement.Framework.MongoDb;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.GenerateNo;
using System;
using System.Collections.Generic;

namespace Sxb.ArticleMajor.Query.Mongodb.Test
{
    public class UnitArticle : UnitBase
    {
        ISxbGenerateNo _sxbGenerateNo;
        IArticleRepository _articleRepository;

        Guid TestId = Guid.Parse("{FCBB302A-097D-4664-89E3-B659F8D62B92}");
        //Guid TestId = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            _sxbGenerateNo = ServiceProvider.GetService<ISxbGenerateNo>();
            _articleRepository = ServiceProvider.GetService<IArticleRepository>();
        }


        [Test]
        [TestCase("1")]
        public void GetHaveDataCategoryIds(ArticlePlatform platform)
        {
            var data = _articleRepository.GetHaveDataCategoryIds(platform).GetAwaiter().GetResult();
            WriteLine(data);
            Assert.IsNotNull(data);
        }

        [Test]
        public void TestAddManySuccess()
        {
            var entities = new List<Article>() {
                new Article()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Code = _sxbGenerateNo.GetNumber(),
                    Title = "2019年北京新中考改革方案",
                    Author = "Labbor",
                    PublishTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    CreatorName = "system-unit",
                    Abstract = "北京市教委日前发布《关于进一步推进高中阶段学校考试招生制度改革的实施意见》",
                    FromWhere = "nunit",
                    Platform =  Common.Enum.ArticlePlatform.YouEr,
                    CategoryId = 30,
                    TagIds = new List<int>(){ 1 },
                    IsValid = true
                },
                new Article()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Code = _sxbGenerateNo.GetNumber(),
                    Title = "2019年北京市中考政策9个关键信息",
                    Author = "Labbor",
                    PublishTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    CreatorName = "system-unit",
                    Abstract = "中考网整理了关于2019年北京市中考9个关键信息，希望对考生们有所帮助",
                    FromWhere = "nunit",
                    Platform =  Common.Enum.ArticlePlatform.YouEr,
                    CategoryId = 30,
                    TagIds = new List<int>(){ 1 },
                    IsValid = true
                }
            };
            _articleRepository.AddAsync(entities).GetAwaiter().GetResult();
            Assert.Pass();
        }

        /// <summary>
        /// 主键重复
        /// </summary>
        [Test]
        public void TestAddFail()
        {
            var entities = new List<Article>() {
                new Article()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    //Id = Guid.NewGuid(),
                    Code = _sxbGenerateNo.GetNumber(),
                    Title = "2019年北京新中考改革方案2",
                    Author = "Labbor",
                    PublishTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    CreatorName = "system-unit",
                    Abstract = "北京市教委日前发布《关于进一步推进高中阶段学校考试招生制度改革的实施意见》",
                    FromWhere = "nunit",
                    Platform =  Common.Enum.ArticlePlatform.ZhongXue,
                    CategoryId= 291,
                    TagIds = new List<int>(){ 1 },
                    IsValid = true
                }

            };

            var ex = Assert.Throws<MongoBulkWriteException<Article>>(() =>
            {
                _articleRepository.AddAsync(entities).GetAwaiter().GetResult();
            }, "主键重复异常");
            WriteLine(ex.Message);
        }

        [Test]
        [TestCase("220120184253668517021811")]
        public void TestGetSuccess(string code)
        {
            var data = _articleRepository.GetAsync(code).GetAwaiter().GetResult();
            WriteLine(data);
            Assert.IsNotNull(data);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("xxx")]
        public void TestGetFail(string code)
        {
            var data = _articleRepository.GetAsync(code).GetAwaiter().GetResult();
            Assert.IsNull(data);
        }
    }
}
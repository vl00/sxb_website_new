using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using ProductManagement.Framework.MongoDb;
using ProductManagement.Framework.MongoDb.UoW;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.ArticleMajor.Query.Mongodb.Test.Helper;
using Sxb.GenerateNo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb.Test
{
    public class UnitUow : UnitBase
    {
        ISxbGenerateNo _sxbGenerateNo;
        IMongoDbUnitOfWork _unitOfWork;

        IArticleRepository _articleRepository;
        IArticleContentRepository _articleContentRepository;
        ICategoryRepository _categoryRepository;

        [SetUp]
        public void Setup()
        {
            _sxbGenerateNo = ServiceProvider.GetService<ISxbGenerateNo>();
            _unitOfWork = ServiceProvider.GetService<IMongoDbUnitOfWork>();

            _articleRepository = ServiceProvider.GetService<IArticleRepository>();
            _articleContentRepository = ServiceProvider.GetService<IArticleContentRepository>();
            _categoryRepository = ServiceProvider.GetService<ICategoryRepository>();
        }


        [Test]
        [TestCase(null)]
        //[TestCase("7a6229f4-34c4-4b59-a9bd-6d7004ed32c9")]
        public async Task TestAddSuccessUowAsync(string _id)
        {
            var id = _id ?? ObjectId.GenerateNewId().ToString();
            GetArticle(id, out List<ArticleContent> contents, out Article entity);

            await _unitOfWork.BeginTransactionAsync();

            await _articleRepository.AddAsync(entity);
            await _articleContentRepository.AddAsync(contents);

            await _unitOfWork.CommitTransactionAsync();
            Assert.Pass();
        }

        [Test]
        [TestCase(null)]
        //[TestCase("7a6229f4-34c4-4b59-a9bd-6d7004ed32c9")]
        public async Task TestAddSuccessAsync(string? _id)
        {
            var id = _id ?? ObjectId.GenerateNewId().ToString();
            GetArticle(id, out List<ArticleContent> contents, out Article entity);

            await _articleRepository.AddAsync(entity);
            await _articleContentRepository.AddAsync(contents);

            Assert.Pass();
        }


        [Test]
        public async Task TestAddSuccessAllAsync()
        {
            //return;

            var platforms = Enum.GetValues<ArticlePlatform>();

            foreach (var platform in platforms)
            {
                if (platform == ArticlePlatform.Master)
                {
                    continue;
                }
                if (platform == ArticlePlatform.YouEr)
                {
                    continue;
                }

                var categories = (await _categoryRepository.GetChildrenFlatAsync(platform, 0)).ToList();

                int count = 0;
                foreach (var category in categories)
                {
                    if (category.IsLeaf)// && category.Id == 113
                    {
                        WriteLine(string.Format("{0}-{1}", category.Name, count++));
                        for (int i = 0; i < 3; i++)
                        {

                            GetArticle(category, i, out List<ArticleContent> contents, out Article entity);
                            await _articleRepository.AddAsync(entity);
                            await _articleContentRepository.AddAsync(contents);
                        }
                    }
                }
            }

            Assert.Pass();
        }
        private void GetArticle(Category category, int number, out List<ArticleContent> contents, out Article entity)
        {
            var cityId = 440100;
            var categoryName = category.Name;
            var id = ObjectId.GenerateNewId().ToString();
            contents = new List<ArticleContent>() { };
            var pageCount = number % 5 + 1;

            var cover = ImgTest.GetTestRandImg();
            for (int page = 0; page < pageCount; page++)
            {
                var contentArray = Enumerable.Range(0, 100).Select(s => string.Format("{0}第{1}篇第{2}页", categoryName, number + 1, page + 1));
                var content = string.Join('-', contentArray) + $" <img src='{cover}' /> ";
                contents.Add(new ArticleContent()
                {
                    ArticleId = id,
                    Content = content,
                    Sort = page
                });
            }

            var abs = contents.FirstOrDefault().Content;
            abs = abs.Length > 100 ? abs.Substring(0, 100) : abs;

            var t = new Random().Next(100000);
            entity = new Article()
            {
                Id = id,
                Code = _sxbGenerateNo.GetNumber(),
                Title = string.Format("{0}第{1}篇", categoryName, number + 1),
                Author = category.ShortName,
                PublishTime = DateTime.Now.AddMinutes(-t).AddMilliseconds(-t),
                CreateTime = DateTime.Now,
                CreatorName = "system-unit",
                Abstract = abs,
                //FromWhere = "nunit",
                Platform = category.Platform,
                CategoryId = category.Id,
                CityId = cityId,
                IsValid = true,
                Covers = new List<string>() { cover },
                PageCount = contents.Count
            };
        }
        private void GetArticle(string id, out List<ArticleContent> contents, out Article entity)
        {
            contents = new List<ArticleContent>() {
                new ArticleContent()
                {
                    ArticleId = id,
                    Content = "测试文章第一页",
                    Sort = 0
                },
                new ArticleContent()
                {
                    ArticleId = id,
                    Content = "测试文章第二页",
                    Sort = 1
                },
                new ArticleContent()
                {
                    ArticleId = id,
                    Content = "测试文章第三页",
                    Sort = 2
                }
            };
            entity = new Article()
            {
                Id = id,
                Code = _sxbGenerateNo.GetNumber(),
                Title = "2019年北京新中考改革方案3",
                Author = "Labbor",
                PublishTime = DateTime.Now,
                CreateTime = DateTime.Now,
                CreatorName = "system-unit",
                Abstract = "北京市教委日前发布《关于进一步推进高中阶段学校考试招生制度改革的实施意见》",
                //FromWhere = "nunit",
                Platform = Common.Enum.ArticlePlatform.YouEr,
                CategoryId = 19,
                TagIds = new List<int>() { 1 },
                IsValid = true,
                PageCount = contents.Count
            };
        }

        [Test]
        public async Task TestAddRollbackAsync()
        {
            var id = ObjectId.GenerateNewId().ToString();
            var entity = new Article()
            {
                Id = id,
            };

            var contents = new List<ArticleContent>() {
                new ArticleContent()
                {
                    ArticleId = id,
                    Content = "测试文章第一页",
                    Sort = 0
                }
            };

            await _unitOfWork.BeginTransactionAsync();

            await _articleRepository.AddAsync(entity);
            await _articleContentRepository.AddAsync(contents);

            await _unitOfWork.RollbackAsync();
            Assert.Pass();
        }


        [Test]
        public async Task TestAddRepeatFailAsync()
        {
            var id = ObjectId.GenerateNewId().ToString();
            //先成功
            await TestAddSuccessAsync(id);


            var entity = new Article()
            {
                Id = id,
            };

            var contents = new List<ArticleContent>() {
                new ArticleContent()
                {
                    ArticleId = id,
                    Content = "测试文章第一页",
                    Sort = 0
                }
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                await _articleRepository.AddAsync(entity);
                await _articleContentRepository.AddAsync(contents);

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (MongoWriteException ex)
            {
                await _unitOfWork.RollbackAsync();
                Assert.Throws<MongoWriteException>(() => throw ex);
                return;
            }
            Assert.Fail();
        }
    }
}
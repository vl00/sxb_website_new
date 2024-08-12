using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProductManagement.Framework.MongoDb;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.GenerateNo;
using System;
using System.Collections.Generic;
using System.Linq;
using Sxb.Framework.Foundation;
using static Sxb.ArticleMajor.Query.Mongodb.Test.XmlHelper;
using TinyPinyin;

namespace Sxb.ArticleMajor.Query.Mongodb.Test
{
    public class UnitCategory : UnitBase
    {
        ICategoryRepository _categoryRepository;

        [SetUp]
        public void Setup()
        {
            _categoryRepository = ServiceProvider.GetService<ICategoryRepository>();
        }

        [Test]
        public void TestAddManySuccess()
        {
            var entities = new List<Category>() {
                new Category()
                {
                    Id = 1,
                    Name = "�п�",
                    ShortName = "zhongkao",
                    Depth = 0,
                    IsLeaf = false,
                    ParentId = 0,
                    Platform = Common.Enum.ArticlePlatform.ZhongXue,
                    IsValid = true
                },
                new Category()
                {
                    Id = 2,
                    Name = "�п�����",
                    ShortName = "baokao",
                    Depth = 1,
                    IsLeaf = false,
                    ParentId = 1,
                    Platform = Common.Enum.ArticlePlatform.ZhongXue,
                    IsValid = true
                },
                new Category()
                {
                    Id = 3,
                    Name = "�п�����",
                    ShortName = "beikao",
                    Depth = 1,
                    IsLeaf = false,
                    ParentId = 1,
                    Platform = Common.Enum.ArticlePlatform.ZhongXue,
                    IsValid = true
                },
                new Category()
                {
                    Id = 4,
                    Name = "�п�ʱ��",
                    ShortName = "shijian",
                    Depth = 2,
                    IsLeaf = false,
                    ParentId = 2,
                    Platform = Common.Enum.ArticlePlatform.ZhongXue,
                    IsValid = true
                },
                new Category()
                {
                    Id = 5,
                    Name = "�п�����",
                    ShortName = "zuowen",
                    Depth = 2,
                    IsLeaf = false,
                    ParentId = 3,
                    Platform = Common.Enum.ArticlePlatform.ZhongXue,
                    IsValid = true
                },
                new Category()
                {
                    Id = 6,
                    Name = "�п���������",
                    ShortName = "manfen",
                    Depth = 3,
                    IsLeaf = false,
                    ParentId = 5,
                    Platform = Common.Enum.ArticlePlatform.ZhongXue,
                    IsValid = true
                },
                new Category()
                {
                    Id = 7,
                    Name = "����2016",
                    ShortName = "gz2016",
                    Depth = 4,
                    IsLeaf = true,
                    ParentId = 6,
                    Platform = Common.Enum.ArticlePlatform.ZhongXue,
                    IsValid = true
                }
            };
            _categoryRepository.AddAsync(entities).GetAwaiter().GetResult();
            Assert.Pass();
        }

        [Test]
        public void TestGetSuccess()
        {
            var data = _categoryRepository.GetListAsync().GetAwaiter().GetResult();
            WriteLine(data);
            Assert.IsNotNull(data);
        }

        [Test]
        public void TestGetChildrenSuccess()
        {
            var data = _categoryRepository.GetChildrenAsync(Common.Enum.ArticlePlatform.ZhongXue, 1).GetAwaiter().GetResult();
            WriteLine(data);
            Assert.IsNotNull(data);
        }
    }
}
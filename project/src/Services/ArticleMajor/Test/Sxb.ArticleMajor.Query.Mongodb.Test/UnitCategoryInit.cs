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
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb.Test
{
    public class UnitCategoryInit : UnitBase
    {
        ICategoryRepository _categoryRepository;

        [SetUp]
        public void Setup()
        {
            _categoryRepository = ServiceProvider.GetService<ICategoryRepository>();
        }


        [Test]
        public async Task FillShortNameAsync()
        {
            var all = (await _categoryRepository.GetListAsync()).ToList();
            var updates = new List<Category>();
            foreach (var item in all)
            {
                //未设置短名称
                if (string.IsNullOrWhiteSpace(item.ShortName))
                {
                    var shortName = PinyinHelper.GetPinyinInitials(item.Name).ToLower();
                    while (all.Where(s => s.Platform == item.Platform && s.ShortName == shortName).Any())
                    {
                        shortName += "s";
                    }

                    item.ShortName = shortName;
                    updates.Add(item);
                }
            }

            if (updates.Count != 0)
            {
                await _categoryRepository.UpdateShortNameAsync(updates);
            }

            Assert.Pass();
        }

        //全局唯一
        //static List<string> shortNames = new List<string>();
        //子站内唯一
        static Dictionary<ArticlePlatform, List<string>> shortNames = new Dictionary<ArticlePlatform, List<string>>();
        static int increaseId = 0;

        [Test]
        public void InitAll()
        {
            var enums = Enum.GetValues<ArticlePlatform>()
                .Where(s => s != ArticlePlatform.Master);
            foreach (var item in enums)
            {
                shortNames.Add(item, new List<string>());
            }

            var sites = new XmlHelper(trim: true).TransferXml();
            List<Category> entities = new List<Category>();

            var children = sites.SelectMany(site =>
            {
                var item = site;
                var platformExpr = enums.Where(s => s.GetDefaultValue<string>() == item.Title);
                if (!platformExpr.Any())
                    return Enumerable.Empty<Category>();
                var platform = platformExpr.First();

                var id = ++increaseId;
                var category = new Category()
                {
                    Id = id,
                    Name = item.Title,
                    ShortName = GetShortName(platform, item.Title),
                    Depth = 0,
                    IsLeaf = false,
                    ParentId = 0,
                    Platform = platform,
                    IsValid = true
                };
                entities.Add(category);

                var childrenCategories = GetChildrenCategories(category.Id, category.Depth + 1, category.Platform, site.Children).ToList();
                return childrenCategories;
            }).ToList();

            var all = entities.Concat(children).ToList();
            //_categoryRepository.AddAsync(all).GetAwaiter().GetResult();
            Assert.Pass();
        }

        public IEnumerable<Category> GetChildrenCategories(int parentId, int depth, ArticlePlatform platform, List<Topic> newChildren)
        {
            if (newChildren != null && newChildren.Any())
            {
                var result = newChildren.SelectMany(s =>
                {
                    //预占id和shortName
                    var id = ++increaseId;
                    var shortName = GetShortName(platform, s.Title);

                    IEnumerable<Category> children = Enumerable.Empty<Category>();
                    if (s.Children != null)
                    {
                        children = GetChildrenCategories(id, depth + 1, platform, s.Children);
                    }

                    //一级分类, 且无子级, 添加自己
                    if (depth == 1 && !children.Any())
                    {
                        var subId = ++increaseId;
                        children = new List<Category>(){
                                new Category()
                                {
                                    Id = subId,
                                    Name = s.Title,
                                    ShortName = GetShortName(platform, s.Title),
                                    Depth = depth + 1,
                                    IsLeaf = true,
                                    ParentId = id,
                                    Platform = platform,
                                    IsValid = true
                                }
                            };
                    }

                    var isLeaf = !children.Any();
                    var list = new List<Category>(){
                        new Category()
                        {
                            Id = id,
                            Name = s.Title,
                            ShortName = shortName,
                            Depth = depth,
                            IsLeaf = isLeaf,
                            ParentId = parentId,
                            Platform = platform,
                            IsValid = true
                        }
                    };
                    return list.Concat(children).ToList();
                }).ToList();
                return result;
            }
            return Enumerable.Empty<Category>();
        }

        /// <summary>
        /// 首字符
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private static string GetShortName(ArticlePlatform platform, string title)
        {

            //var shortName = PinyinHelper.GetPinyin(s.Title, separator: "").ToLower();
            //首字符
            var shortName = PinyinHelper.GetPinyinInitials(title).ToLower();
            while (shortNames[platform].Contains(shortName))
            {
                shortName += "s";
            }
            shortNames[platform].Add(shortName);
            return shortName;
        }


    }
}
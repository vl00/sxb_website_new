using Sxb.ArticleMajor.API.Utils;
using Sxb.ArticleMajor.Common.Enum;
using System;
using System.Collections.Generic;

namespace Sxb.ArticleMajor.API.Application.Query.ViewModel
{
    public class NavSubjectVM
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string[] ShortNames { get; set; }

        public List<Item> Items { get; set; }

        public class Item
        {
            public Item()
            {
            }

            public Item(ArticlePlatform platform, string name)
            {
                Platform = platform;
                Name = name;
            }

            public bool Show => Articles?.Count > 0;

            public ArticlePlatform Platform { get; set; }

            public string Name { get; set; }

            public string Url { get; set; }

            public List<int> CategoryIds { get; set; }

            public List<ArticleItemVM> Articles { get; set; }
        }

        public static List<Item> DefaultItems()
        {
            return new List<Item>()
            {
                //new Item(ArticlePlatform.GuoJi, "国际"),
                new Item(ArticlePlatform.GaoZhong, "高中"),
                new Item(ArticlePlatform.ZhongXue, "初中"),
                //new Item(ArticlePlatform.ZhongZhi, "中职"),
                new Item(ArticlePlatform.XiaoXue, "小学"),
                new Item(ArticlePlatform.YouEr, "幼儿园"),
                //new Item(ArticlePlatform.SuZhi, "素质教育"),
            };
        }
    }

    public class SubNavSubjectVM
    {
        public string Name { get; set; }

        public string ShortName { get; set; }

        public List<Item> Items { get; set; }

        public class Item
        {
            public Item()
            {
            }

            public string Name { get; set; }

            public string ShortName { get; set; }

            public string Text => Name;

            public string ShortId => ShortName;
        }
    }
}

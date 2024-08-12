using Sxb.ArticleMajor.API.Utils;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.MongoEntity;
using System;
using System.Linq;

namespace Sxb.ArticleMajor.API.Application.Query.ViewModel
{
    public class ArticleItemVM
    {
        /// <summary> 
        /// </summary> 
        public string Code { get; set; }

        /// <summary> 
        /// </summary> 
        public string Title { get; set; }

        /// <summary> 
        /// 文章作者 
        /// </summary> 
        public string Author { get; set; }

        /// <summary> 
        /// </summary> 
        public string PublishTime { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Abstract { get; set; }

        /// <summary>
        /// 直属分类
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 直属分类短名称
        /// </summary>
        public string CategoryShortName { get; set; }

        /// <summary>
        /// 封面图
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// 归属平台
        /// </summary>
        public ArticlePlatform Platform { get; set; }

        /// <summary>
        /// 上级链路
        /// </summary>
        public string LinkedParents { get; set; }

        /// <summary>
        /// 文章链接
        /// </summary>
        public string Url => ArticleUtils.GetArticleUrl(Code, Platform);

        public static ArticleItemVM Convert(Article s, Category category)
        {
            return new ArticleItemVM()
            {
                Code = s.Code,
                Title = s.Title,
                Author = s.Author,
                Abstract = s.Abstract,
                PublishTime = s.PublishTime.ToString("yyyy-MM-dd"),
                CategoryName = category?.Name,
                CategoryShortName = category?.ShortName,
                //Cover = s.Covers?.FirstOrDefault(), //不再显示图片
                Cover = GetRandomImages(s.Id, s.CategoryId, category?.ParentId), //使用指定分类的随机图片
                Platform = s.Platform // category?.Platform ?? ArticlePlatform.Master
            };
        }

        public static string GetRandomImages(string id, int categoryId, int? parentId)
        {
            var idImage = Constant.IdImages.FirstOrDefault(s => s.Id == categoryId);

            //尝试自己父级有无配置图片
            if (idImage == null && parentId != null)
            {
                idImage = Constant.IdImages.FirstOrDefault(s => s.Id == parentId);

                //尝试直接匹配双方父级
                if (idImage == null)
                {
                    idImage = Constant.IdImages.FirstOrDefault(s => s.ParentId == parentId);
                }
            }

            var images = idImage?.Images;
            if (images == null || images.Length == 0)
                return string.Empty;

            //var hash = Math.Abs(id.GetHashCode());
            int hash = GetBaseInt(id[^1]) + GetBaseInt(id[^2]);

            var length = images.Length;
            var pos = hash % length;
            return images[pos];
        }

        public static int GetBaseInt(char c)
        {
            int i = c;
            if (i is >= 48 and <= 57)
            {
                return i - 48;
            }
            else if (i is >= 65 and <= 90)
            {
                //A = 10  B = 11
                return i - 65 + 10;
            }
            else if (i is >= 97 and <= 122)
            {
                //a = 10  b = 11
                return i - 97 + 10;
            }
            return c;
        }
    }
}

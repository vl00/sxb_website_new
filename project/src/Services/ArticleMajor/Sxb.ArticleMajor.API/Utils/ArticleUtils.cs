using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.QueryDto;
using System.Collections.Generic;
using Sxb.Framework.Foundation;

namespace Sxb.ArticleMajor.API.Utils
{
    public class ArticleUtils
    {

        public static string GetArticleUrl(string code, ArticlePlatform platform)
        {
            var site = Constant.ArticleSites.TryGetValue(platform);

            //https://gaozhong.sxkid.com/detail/18bce19b07a64c798797e0c992a8c820
            return UriHelper.Combine(site, "detail", code);
        }
    }
}

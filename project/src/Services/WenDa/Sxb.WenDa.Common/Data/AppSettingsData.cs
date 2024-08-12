using Sxb.WenDa.Common.Enum;
using System.Collections.Generic;
using System.Linq;
using Sxb.Framework.Foundation;

namespace Sxb.WenDa.Common.Data
{
    public class AppSettingsData
    {
        public Dictionary<ArticlePlatform, string> ArticleSites { get; set; } = new Dictionary<ArticlePlatform, string>();


        public WeChatMsg WeChatMsg { get; set; }

        public string GetSite(long categoryId)
        {
            return GetSite(GetArticlePlatform(categoryId));
        }

        public string GetSite(ArticlePlatform platform)
        {
            ArticleSites.TryGetValue(platform, out var site);
            return site;
        }

        public static ArticlePlatform GetArticlePlatform(long categoryId)
        {
            var enums = System.Enum.GetValues(typeof(ArticlePlatform)).Cast<ArticlePlatform>();
            foreach (var item in enums)
            {
                if (categoryId == item.GetDefaultValue<long>())
                {
                    return item;
                }
            }
            return ArticlePlatform.Master;
        }
    }
}

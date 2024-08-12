using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Common
{
    public class RedisKeys
    {
        //平台-站点-分类
        //中考-广州站-中考真题
        //articlemajor:hot:zhongkao:gz:1
        public static string HotArticlesKey = "articlemajor:hot:{0}:{1}:{2}";

        //articlemajor:top:zhongkao:gz:1
        public static string LastestArticlesKey = "articlemajor:lastest:{0}:{1}:{2}:{3}:{4}";

        //platform:city:categoryName
        public static string RecommendArticlesKey = "articlemajor:recommend:{0}:{1}:{2}";

        public static string NavsKey = "articlemajor:navs";
        public static string SubNavsKey = "articlemajor:subnavs:{0}";

        //主站专栏配置
        public static string NavsSubjectsConfig = "articlemajor:navs:subjects:config";
        //主站专栏缓存
        public static string NavsSubjects = "articlemajor:navs:subjects";

        //有文章的分类
        public static string HaveDataCityCategories = "articlemajor:categories:city-{0}";

        //子站专栏配置  platform
        public static string SubNavSubjectsConfigKey = "articlemajor:subnavs:subjects:config:{0}";

    }
}

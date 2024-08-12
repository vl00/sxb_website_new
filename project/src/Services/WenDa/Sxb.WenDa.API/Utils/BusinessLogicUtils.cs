
namespace Sxb.WenDa.API.Utils
{
    /// <summary>
    /// 业务utils
    /// </summary>
    internal static class BusinessLogicUtils
    {
        /// <summary>
        /// 默认头像
        /// </summary>
        public const string AnonyUserHeadImg = "https://cos.sxkid.com/images/head.png";

        /// <summary>
        /// 匿名规则：匿名用户+匿名编码（两个英文字母+5个数字）。
        /// </summary>
        /// <returns></returns>
        public static string RandomAnonyUserName()
        {
            var c0 = (int)'a';
            var rnd = new Random(unchecked((int)DateTime.Now.Ticks));

            var w2 = $"{((char)(c0 + rnd.Next(0, 26)))}{((char)(c0 + rnd.Next(0, 26)))}";
            var d5 = string.Join("", Enumerable.Range(1, 5).Select(_ => $"{(0 + rnd.Next(0, 10))}"));

            return $"匿名用户{w2}{d5}";
        }

        /// <summary>
        /// 一级分类id to 年级
        /// </summary>
        /// <param name="lv1CategoryId">一级分类id</param>
        /// <returns></returns>
        public static int Lv1CategoryIdToGrade(long lv1CategoryId)
        {
            return lv1CategoryId switch
            {
                1 => 1, // 幼儿
                2 => 2, // 小学
                3 => 3, // 初中
                5 => 4, // 高中
                _ => 0,
            };
        }

        /// <summary>
        /// 根据分类路径获取站点（一级）
        /// </summary>
        public static long GetPlatformFromCategoryPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return 0;
            var ps = path.AsSpan()[1..];
            var s = ps[..ps.IndexOf('/')];
            return long.TryParse(s, out var v) ? v : 0;
        }
    }
}

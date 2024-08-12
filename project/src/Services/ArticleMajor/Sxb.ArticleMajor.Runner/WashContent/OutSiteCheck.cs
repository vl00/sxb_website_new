using System.Collections.Concurrent;

namespace Sxb.ArticleMajor.Runner.WashContent
{
    public class OutSiteCheck
    {
        public List<string> OutSites { get; }
        private ConcurrentDictionary<string, bool> OutUrlDic = new ConcurrentDictionary<string, bool>();
        private ConcurrentDictionary<string, string> ArticleCodeDic = new ConcurrentDictionary<string, string>();

        public OutSiteCheck()
        {
            OutSites = GetOutSites();
        }

        public bool IsOutSite(string url)
        {
            if (OutUrlDic.TryGetValue(url, out bool isOutSite))
            {
                return isOutSite;
            }

            isOutSite = IsOutSiteMain(url);
            OutUrlDic.TryAdd(url, isOutSite);
            return isOutSite;
        }

        private bool IsOutSiteMain(string url)
        {
            url = url.Trim();
            if (!url.StartsWith("https://") && !url.StartsWith("http://"))
            {
                return false;
            }

            string host;
            try
            {
                var uri = new Uri(url);
                host = uri.Host;
            }
            catch (Exception)
            {
                return false;
            }


            foreach (var item in OutSites)
            {
                if (host.Contains(item, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }


        public string GetArticleCode(string url)
        {
            if (ArticleCodeDic.TryGetValue(url, out string code))
            {
                return code;
            }
            return null;
        }

        public void AddArticleCode((string url, string code)[] data)
        {
            foreach (var (url, code) in data)
            {
                ArticleCodeDic.TryAdd(url, code);
            }
        }

        private static List<string> GetOutSites()
        {
            var sites = new List<string>()
            {
                //https://shop99905021.m.youzan.com/wscgoods/detail/2xmyyiwjkxkh1l2?step=1
                //http://m.aoshu.com/e/20210508/6095caae97be3.shtml

                "youjiao.com",
                "aoshu.com",
                "zhongkao.com",
                "gaokao.com",
                "zuowen.com",
            };
            //var mSites = sites.Select(s => s.Replace("www", "m")).ToList();
            //sites.AddRange(mSites);

            return sites;
        }
    }
}

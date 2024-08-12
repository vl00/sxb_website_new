using Newtonsoft.Json;
using SharpCompress.Common.Zip;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.QueryDto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Sxb.ArticleMajor.API.Utils
{
    public class Constant
    {
        public static void Init(AppSettingsData appSettingsData)
        {
            ArticleSites = appSettingsData.ArticleSites;
        }

        //public static Dictionary<ArticlePlatform, string> ArticleSites = new Dictionary<ArticlePlatform, string>()
        //{
        //    {ArticlePlatform.Master, "https://master.sxkid.com"},
        //    {ArticlePlatform.YouEr, "https://youer.sxkid.com"},
        //    {ArticlePlatform.XiaoXue, "https://xiaoxue.sxkid.com"},
        //    {ArticlePlatform.ZhongXue, "https://zhongxue.sxkid.com"},
        //    {ArticlePlatform.ZhongZhi, "https://zhongzhi.sxkid.com"},
        //    {ArticlePlatform.GaoZhong, "https://gaozhong.sxkid.com"},
        //    {ArticlePlatform.SuZhi, "https://suzhi.sxkid.com"},
        //    {ArticlePlatform.GuoJi, "https://guoji.sxkid.com"},
        //};
        public static Dictionary<ArticlePlatform, string> ArticleSites { get; set; } = new Dictionary<ArticlePlatform, string>();

        public static List<IdImage> IdImages => GetImages();

        private static List<IdImage> idImages = idImages ?? GetImages();

        private static List<IdImage> GetImages()
        {
            try
            {
                // docker /app/bin/Debug/net6.0/
                var domain = AppDomain.CurrentDomain.BaseDirectory;
                // docker /app
                //var domain = Environment.CurrentDirectory;

                var separator = Path.DirectorySeparatorChar;
                //linux不支持\\文件路径
                //var filename = Path.Combine(domain, "wwwroot\\assets\\category-images.zip");
                var filename = Path.Combine(domain, "wwwroot/assets/category-images.zip");

                var zipArchive = SharpCompress.Archives.Zip.ZipArchive.Open(filename);

                var reader = zipArchive.Entries.First();
                var stream = reader.OpenEntryStream();

                var sr = new StreamReader(stream);
                return JsonConvert.DeserializeObject<List<IdImage>>(sr.ReadToEnd()); ;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<IdImage>();
            }
        }
    }
}

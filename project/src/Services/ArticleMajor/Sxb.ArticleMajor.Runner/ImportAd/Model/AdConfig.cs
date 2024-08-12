using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Runner.ImportAd.Model
{
    internal class AdConfig
    {
        [NPOIHelper.TitleName("位置合集")]
        public string LocationIdStrs { get; set; }

        [NPOIHelper.TitleName("配置的广告图片名称")]
        public string ImageName { get; set; }

        //图片的路径
        public string ImageFileName { get; set; }


        public int[] LocationIds => GetLocationIds();

        private int[] GetLocationIds()
        {
            try
            {
                return LocationIdStrs.Split(',').Select(s => int.Parse(s.Trim())).ToArray();
            }
            catch (Exception ex)
            {
                return new int[] { };
            }
        }
    }
}

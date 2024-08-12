using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Common.Enum
{
    /// <summary>
    /// 子站  1幼儿教育 2小学教育 3中学教育 4中职网 5高中教育 6素质教育 7国际教育
    /// </summary>
    public enum ArticlePlatform
    {
        [DefaultValue("主站")]
        Master = 0,
        [DefaultValue("幼儿教育")]
        YouEr = 1,
        [DefaultValue("小学教育")]
        XiaoXue = 2,
        [DefaultValue("中学教育")]
        ZhongXue = 3,
        [DefaultValue("中职网")]
        ZhongZhi = 4,
        [DefaultValue("高中教育")]
        GaoZhong = 5,
        [DefaultValue("素质教育")]
        SuZhi = 6,
        [DefaultValue("国际教育")]
        GuoJi = 7
    }
}

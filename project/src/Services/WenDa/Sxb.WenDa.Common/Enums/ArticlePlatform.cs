using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.Enum
{
    /// <summary>
    /// 子站  1幼儿教育 2小学教育 3中学教育 4中职网 5高中教育 6素质教育 7国际教育
    /// </summary>
    public enum ArticlePlatform
    {
        /// <summary>
        /// 数据库是long,这里一致
        /// </summary>
        [DefaultValue(0L)]
        [Description("主站")]
        Master = 0,

        [DefaultValue(1L)]
        [Description("幼儿教育")]
        YouEr = 1,

        [DefaultValue(2L)]
        [Description("小学教育")]
        XiaoXue = 2,

        [DefaultValue(3L)]
        [Description("中学教育")]
        ZhongXue = 3,

        [DefaultValue(5L)]
        [Description("高中教育")]
        GaoZhong = 5
    }
}

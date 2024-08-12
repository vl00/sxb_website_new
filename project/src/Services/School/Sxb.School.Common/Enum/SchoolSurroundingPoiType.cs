using System.ComponentModel;

namespace Sxb.School.Common.Enum
{
    /// <summary>
    /// 周边信息类型
    /// </summary>
    public enum SchoolSurroundingPoiType
    {
        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 0,
        /// <summary>
        /// 商场
        /// </summary>
        [Description("商场")]
        Mall = 1,
        /// <summary>
        /// 书店
        /// </summary>
        [Description("书店")]
        BookShop = 2,
        /// <summary>
        /// 医院
        /// </summary>
        [Description("医院")]
        Hospital = 3,
        /// <summary>
        /// 警察局
        /// </summary>
        [Description("警察局")]
        PoliceStation = 4,
        /// <summary>
        /// 地铁站
        /// </summary>
        [Description("地铁站")]
        SubwayStation = 5,
        /// <summary>
        /// 公交站
        /// </summary>
        [Description("公交站")]
        BusStation = 6,
        /// <summary>
        /// 房产相关
        /// </summary>
        [Description("房产相关")]
        Estate = 7
    }
}

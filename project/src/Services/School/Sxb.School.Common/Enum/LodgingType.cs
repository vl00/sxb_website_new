using System.ComponentModel;

namespace Sxb.School.Common.Enum
{
    /// <summary>
    /// 寄宿类型 0 暂未收录 1 走读 2 寄宿 3 寄宿&走读
    /// </summary>
    /// <modify time="2021-08-31 16:32:19" author="Lonlykids"></modify>
    public enum LodgingType
    {
        /// <summary>
        /// 未收录
        /// </summary>
        [Description("暂未收录")]
        Unkown = 0,
        /// <summary>
        /// 走读
        /// </summary>
        [Description("走读")]
        Walking = 1,
        /// <summary>
        /// 寄宿
        /// </summary>
        [Description("寄宿")]
        Lodging = 2,
        /// <summary>
        /// 可走读、寄宿 
        /// </summary>
        [Description("寄宿&走读")]
        LodgingOrGo = 3
    }

    public static class LodgingUtil
    {
        public static LodgingType Reason(bool? lodging, bool? sdextern)
        {
            if (lodging == true && sdextern == true)
            {
                return LodgingType.LodgingOrGo;
            }
            if (lodging == true)
            {
                return LodgingType.Lodging;
            }
            if (sdextern == true)
            {
                return LodgingType.Walking;
            }
            return LodgingType.Unkown;
        }
    }
}

using System;

namespace Sxb.School.API
{

    public class Configs
    {
        public static string AppID => "sxbschoolapi483092490";

        public const string PaySystem = "OrgSystem";
        public const string PayKey = "orgwoaishangxuebang2021";
        public const decimal ViewSchoolUnitPrice = 1;

        /// <summary>
        /// 学位分析结果解锁单价
        /// </summary>
        public const decimal DgAyResultUnitPrice = 1;


        /// <summary>
        /// 学位分析器限时免费期限
        /// </summary>
        public static DateTime DgAyFreeSTime = new DateTime(2022, 5, 16, 00, 00, 00);
        public static DateTime DgAyFreeETime = new DateTime(2022, 6, 30, 00, 00, 00);

    }


    public class RedisCacheKeys
    {

        public static string AppIDKey => $"InsideApps:{Configs.AppID}";


        /// <summary>
        /// 学位分析订单微信回调DataKey
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static string WXCB_DHAY_Order(Guid orderId) => $"wxcallback_dgay_order:{orderId}";

        /// <summary>
        /// 签名重放nonce
        /// </summary>
        public const string PayCenterVisitNonce = "PayCenterVisitNonce_nonce{0}";


        public const string CallSchoolApi_School_idandname = "splext:school_idandname:eid_{0}";
        public const string CallSchoolApi_School_noandname = "splext:school_idandname:no_{0}";
    
    }
}

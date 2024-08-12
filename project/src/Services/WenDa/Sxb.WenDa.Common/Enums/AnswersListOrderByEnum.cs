using System.ComponentModel;

namespace Sxb.WenDa.Common.Enums
{
    /// <summary>
    /// 回答列表 排序
    /// </summary>
    public enum AnswersListOrderByEnum
    {
        /// <summary>点赞多到少</summary>
        [Description("点赞多到少")]
        LikeCountDesc = 1,
        /// <summary>时间近到远</summary>
        [Description("时间近到远")]
        CreateTimeDesc = 2,
    }


}

using System.ComponentModel;

namespace Sxb.WenDa.Common.Enums
{
    /// <summary>
    /// 专题-问题列表 排序
    /// </summary>
    public enum SubjectQuestionListOrderByEnum
    {
        /// <summary>
        /// 热门问题 <br/>
        /// 优先显示专题问答中回答数最多的问题。如果回答数一样多，则按问答总点赞数从多到少排序。由回答数从多到少进行排序
        /// </summary>
        [Description("热门问题")]
        Hot = 1,
        /// <summary>
        /// 最新提问 <br/>
        /// 时间近到远
        /// </summary>
        [Description("最新提问")]
        CreateTimeDesc = 2,
    }


}

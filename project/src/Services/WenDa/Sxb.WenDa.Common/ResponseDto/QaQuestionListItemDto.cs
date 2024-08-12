using Sxb.Framework.Foundation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    /// <summary>
    /// 问答item-公共头部
    /// </summary>
    public class QaItemDto
    {
        /// <summary>问题id</summary>
        public Guid QuestionId { get; set; }
        /// <summary>问题短id</summary>
        public string QuestionNo { get; set; }

        /// <summary>问题标题</summary>
        public string Title { get; set; }

        /// <summary>问题标签s</summary>
        public List<string> Tags { get; set; }
        /// <summary>城市编号</summary>
        public long City { get; set; }
        /// <summary>城市名</summary>
        public string CityName { get; set; }

        public List<Guid> _SchoolEids { get; set; } = null;
        /// <summary>
        /// 关联的学校短id <br/>
        /// 列表的学校暂不可点,不需要此字段 <br/>
        /// 问答详情里的学校需要此字段 打开新窗口
        /// </summary>
        public List<string> SchoolIds { get; set; } = null;
        /// <summary>关联的学校名</summary>
        public List<string> SchoolNames { get; set; }

        /// <summary>回答数</summary>
        public int ReplyCount { get; set; }

        /// <summary>收藏数</summary>
        public int CollectCount { get; set; }
        /// <summary>true=我有收藏</summary>
        public bool? IsCollectedByMe { get; set; }

        /// <summary>
        /// 问题的 创建时间=发布于
        /// </summary>
        [JsonConverter(typeof(DateTimeToTimestampJsonConverter))]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 问题的 最后一次编辑时间=编辑于
        /// </summary>
        [JsonConverter(typeof(DateTimeToTimestampJsonConverter))]
        public DateTime? EditTime { get; set; }

        /// <summary>提问者id</summary>
        public Guid UserId { get; set; }
        /// <summary>提问者-用户名</summary>
        public string UserName { get; set; }
        /// <summary>提问者-用户头像</summary>
        public string UserHeadImg { get; set; }
        /// <summary>提问者-用户描述</summary>
        public string UserDesc { get; set; } = null;

        /// <summary>true=是我发起的问题; false=不是</summary>
        public bool IsMyQuestion { get; set; }

        /// <summary>
		/// true=匿名 false=非匿名
		/// </summary> 
		public bool? IsAnony { get; set; }
    }

    /// <summary>
    /// 问答item-回答部分 / 问答详情-回答列表item
    /// </summary>
    public class QaAnswerItemDto
    {
        /// <summary>回答id</summary>
        public Guid AnswerId { get; set; }
        /// <summary>回答短id</summary>
        public string AnswerNo { get; set; }

        /// <summary>用户id</summary>
        public Guid UserId { get; set; }
        /// <summary>用户名</summary>
        public string UserName { get; set; }
        /// <summary>用户头像</summary>
        public string UserHeadImg { get; set; }
        /// <summary>用户描述</summary>
        public string UserDesc { get; set; }

        /// <summary>点赞数</summary>
        public int LikeCount { get; set; }
        /// <summary>true=是我点赞的</summary>
        public bool? IsLikeByMe { get; set; }

        /// <summary>评论总数(含子评论)</summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 创建时间=发布于
        /// </summary>
        [JsonConverter(typeof(DateTimeToTimestampJsonConverter))]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 最后一次编辑时间=编辑于
        /// </summary>
        [JsonConverter(typeof(DateTimeToTimestampJsonConverter))]
        public DateTime? EditTime { get; set; }

        /// <summary>回答的内容</summary>
        public string Content { get; set; }
        /// <summary>回答的第一张图.如没则为null</summary>
        public string Img { get; set; } = null;

        /// <summary>true=是我发起的回答; false=不是</summary>
        public bool IsMyAnswer { get; set; }

        /// <summary>
		/// true=匿名 false=非匿名
		/// </summary> 
		public bool? IsAnony { get; set; }
    }

    /// <summary>
    /// 问题item.用于列表
    /// </summary>
    public class QaQuestionListItemDto : QaItemDto
    {
        /// <summary>回答dto</summary>
        public QaAnswerItemDto Answer { get; set; }
    }

    /// <summary>
    /// 问答详情-问题部分
    /// </summary>
    public class QaQuestionItemDto : QaItemDto
    {
        /// <summary>问题补充描述</summary>
        public string Content { get; set; }
        /// <summary>原图s</summary>
        public string[] Imgs { get; set; }
        /// <summary>缩略图s</summary>
        public string[] Imgs_s { get; set; }

        /// <summary>分类id</summary>
        public long CategoryId { get; set; }
        /// <summary>分类名称</summary>
        public string CategoryName { get; set; }
        /// <summary>问题标签ids</summary>
        public long[] TagIds { get; set; }
        /// <summary>站点/一级标签</summary> 
        public long Platform { get; set; }
    }

    public class MyCollectQuestionListItemDto : QaQuestionListItemDto
    {
        /// <summary>
        /// true=我回答过此问题; <br/>
        /// false=我未回答过此问题 <br/>
        /// 若用户本人未回答过，则显示写回答按钮。若已回答过则不显示。
        /// </summary>
        public bool? HasMyAnswers { get; set; } = default!;
    }
}

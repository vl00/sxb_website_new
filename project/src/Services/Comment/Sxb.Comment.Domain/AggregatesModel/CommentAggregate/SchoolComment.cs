using Sxb.Comment.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Sxb.Comment.Domain.AggregatesModel.CommentAggregate
{
    public class SchoolComment
    {
        /// <summary>
         /// 学校ID
         /// </summary>
        public Guid SchoolId { get; private set; }
        /// <summary>
        /// 学校学部ID
        /// </summary>
        public Guid SchoolSectionId { get; private set; }
        /// <summary>
        /// 点评者ID
        /// </summary>
        public Guid CommentUserId { get; private set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; private set; }
        /// <summary>
        /// 点评审核状态（0：未阅，1：已阅，2：已加精，4：已屏蔽） 默认值为：0（可以在前端显示）
        /// </summary>
        public ExamineStatus State { get; private set; }
        /// <summary>
        /// 用户角色
        /// </summary>
        public UserRoleType PostUserRole { get; private set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; private set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; private set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; private set; }
        /// <summary>
        /// 是否已结算
        /// </summary>
        public bool IsSettlement { get; private set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; private set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; private set; }
        /// <summary>
        /// 是否有上传图片
        /// </summary>
        public bool IsHaveImagers { get; private set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long No { get; private set; }

        /// <summary>
        /// 浏览量
        /// </summary>
        public int ViewCount { get; private set; }

        /// <summary>
        /// 写入日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime AddTime { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public void Comment()
        {

        }
    }
}

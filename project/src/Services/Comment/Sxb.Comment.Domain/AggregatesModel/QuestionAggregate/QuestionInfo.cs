using Sxb.Comment.Common.Enum;
using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Sxb.Comment.Domain.AggregatesModel
{
    [Table("QuestionInfos")]
    public class QuestionInfo : Entity<Guid>, IAggregateRoot
    {
        /// <summary>
        /// 问题审核状态（0：未阅，1：已阅，2：已加精，4：已屏蔽） 默认值为：0（可以在前端显示）
        /// </summary>
        [Required]
        public ExamineStatus State { get; private set; }
        /// <summary>
        /// 学校Id
        /// </summary>
        public Guid SchoolId { get; private set; }

        /// <summary>
        /// 学校学部ID
        /// </summary>
        public Guid SchoolSectionId { get; private set; }
        /// <summary>
        /// 问题写入者
        /// </summary>
        public Guid UserId { get; private set; }
        /// <summary>
        /// 用户角色
        /// </summary>
        public UserRoleType PostUserRole { get; private set; }

        /// <summary>
        /// 问题内容
        /// </summary>
        [Required]
        public string Content { get; private set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; private set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; private set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; private set; }
        /// <summary>
        /// 是否有上传图片
        /// </summary>
        public bool IsHaveImagers { get; private set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; private set; }

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
        /// 创建日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// 新增问题
        /// </summary>
        public void AddQuestion()
        {

        }
    }
}

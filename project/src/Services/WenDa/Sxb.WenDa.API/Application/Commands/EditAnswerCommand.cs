using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Sxb.WenDa.API.Application.Commands
{
    /// <summary>
    /// 编辑回答
    /// </summary>
    public class EditAnswerCommand : IRequest<EditAnswerCommandResult>
    {
        /// <summary>用户id(后端字段,前端不管)</summary>
        [IgnoreDataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 回答id
        /// </summary>
        [Required]
        public Guid AnswerId { get; set; }

        /// <summary>图片s</summary>
        public string[] Imgs { get; set; } 
        /// <summary>图片缩略图s</summary>
        public string[] Imgs_s { get; set; }
        /// <summary>
        /// 富文本内容(含图片等)
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// true=匿名; false=非匿名
        /// </summary>
        public bool IsAnony { get; set; }
    }

    public class EditAnswerCommandResult
    {
        /// <summary>回答id</summary>
        public Guid AnswerId { get; set; }
        /// <summary>回答短id</summary>
        public string AnswerNo { get; set; }

        ///// <summary>是否关注公众号</summary>
        //public bool HasGzWxGzh { get; set; }
        ///// <summary>是否已加企业微信客服</summary>
        //public bool HasJoinWxEnt { get; set; }
    }
}

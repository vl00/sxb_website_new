using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Sxb.WenDa.API.Application.Commands
{
    /// <summary>
    /// 发评论
    /// </summary>
    public class AddQaCommentCommand : IRequest<AddQaCommentCommandResult>
    {
        /// <summary>用户id(后端字段,前端不管)</summary>
        [IgnoreDataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 回复评论时, 传评论id <br/>
        /// answerId 与 commentId 只能传其中一个
        /// </summary>
        public Guid? CommentId { get; set; }
        /// <summary>
        /// 回复回答时, 传回答id <br/>
        /// answerId 与 commentId 只能传其中一个
        /// </summary>
        public Guid? AnswerId { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }
    }

    public class AddQaCommentCommandResult
    {
        /// <summary>评论id</summary>
        public Guid CommentId { get; set; }

        ///// <summary>是否关注公众号</summary>
        //public bool HasGzWxGzh { get; set; }
        ///// <summary>是否已加企业微信客服</summary>
        //public bool HasJoinWxEnt { get; set; }
    }
}

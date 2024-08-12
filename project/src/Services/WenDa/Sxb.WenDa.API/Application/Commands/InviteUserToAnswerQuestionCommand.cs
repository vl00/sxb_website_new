using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Sxb.WenDa.API.Application.Commands
{
    /// <summary>
    /// 邀请别人来回答问题
    /// </summary>
    public class InviteUserToAnswerQuestionCommand : IRequest<InviteUserToAnswerQuestionCommandResult>
    {
        /// <summary>邀请用户id(后端字段,前端不管)</summary>
        [IgnoreDataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 问题id
        /// </summary>
        [Required]
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 被邀请用户id
        /// </summary>
        public Guid ToUserId { get; set; }
    }

    public class InviteUserToAnswerQuestionCommandResult
    {
        /// <summary>
        /// true=邀请成功; false=已邀请
        /// </summary>
        public bool Success { get; set; }
    }
}

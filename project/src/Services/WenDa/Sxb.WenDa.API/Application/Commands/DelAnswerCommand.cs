using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Sxb.WenDa.API.Application.Commands
{
    /// <summary>
    /// 删除回答
    /// </summary>
    public class DelAnswerCommand : IRequest<DelAnswerCommandResult>
    {
        /// <summary>用户id(后端字段,前端不管)</summary>
        [IgnoreDataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 回答id
        /// </summary>
        [Required]
        public Guid AnswerId { get; set; }        
    }

    public class DelAnswerCommandResult
    {
        /// <summary>
        /// true=删除成功; false=已删除
        /// </summary>
        public bool Success { get; set; }
    }
}

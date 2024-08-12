using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Sxb.WenDa.API.Application.Commands
{
    /// <summary>
    /// up专栏viewcount
    /// </summary>
    public class UpSubjectViewCountCommand : IRequest<UpSubjectViewCountCommandResult>
    {
        /// <summary>用户id(后端字段,前端不管)</summary>
        [IgnoreDataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 专栏id
        /// </summary>
        [Required]
        public Guid SubjectId { get; set; }

        public int IncrCount { get; set; } = 1;

        public bool UpToDb { get; set; } = false;
    }

    public class UpSubjectViewCountCommandResult
    {       
    }
}

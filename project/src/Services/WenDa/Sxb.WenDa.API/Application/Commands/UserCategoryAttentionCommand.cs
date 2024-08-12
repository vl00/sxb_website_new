using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Sxb.WenDa.API.Application.Commands
{
    public class UserCategoryAttentionCommand : IRequest<ResponseResult>
    {
        /// <summary>
        /// 收藏人
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 分类id
        /// </summary>
        public List<long> CategoryIds { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

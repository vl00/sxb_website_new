using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Sxb.WenDa.API.Application.Commands
{
    public class CollectCommand : IRequest<ResponseResult>
    {
        /// <summary>
        /// 收藏类型
        /// </summary>
        public UserCollectType Type { get; set; }

        /// <summary>
        /// 收藏人
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 收藏对象id
        /// </summary>
        public Guid DataId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public bool IsValid { get; set; }
    }
}

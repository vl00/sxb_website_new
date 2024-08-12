using MediatR;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.Enums;

namespace Sxb.WenDa.API.Application.Commands
{
    public class LikeCommand : IRequest<ResponseResult>
    {
        /// <summary>
        /// 点赞类型  1=回答 2=评论
        /// </summary>
        public UserLikeType Type { get; set; }

        /// <summary>
        /// 人
        /// </summary>
        [JsonIgnore]
        public Guid UserId { get; set; }

        /// <summary>
        /// 对象id
        /// </summary>
        public Guid DataId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonIgnore]
        public DateTime CreateTime { get; set; }

        public bool IsValid { get; set; }
    }
}

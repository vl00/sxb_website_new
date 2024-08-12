using Sxb.Domain;
using Sxb.WenDa.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Domain.AggregateModel.LikeAggregate
{
    public class UserLikeInfo : Entity<Guid>, IAggregateRoot
    {
        public UserLikeInfo(UserLikeType type, Guid userId, Guid dataId)
        {
            Type = type;
            UserId = userId;
            DataId = dataId;

            CreateTime = DateTime.Now;
            IsValid = true;
        }

        /// <summary>
        /// 类型
        /// </summary>
        public UserLikeType Type { get; set; }

        /// <summary>
        /// 人
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 对象id
        /// </summary>
        public Guid DataId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public bool IsValid { get; set; }

        public int SetValid(bool v)
        {
            IsValid = v;
            CreateTime = DateTime.Now;
            return v ? 1 : -1;
        }
    }

}

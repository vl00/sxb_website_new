using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Domain.AggregateModel.UserCategoryAttentionAggregate
{
    public partial class UserCategoryAttention : Entity<Guid>, IAggregateRoot
    {

        public UserCategoryAttention() { }
        public UserCategoryAttention(Guid userId, long categoryId, DateTime createTime)
        {
            UserId = userId;
            CategoryId = categoryId;
            CreateTime = createTime;
            IsValid = true;
        }

        /// <summary>
        /// 用户id
        /// </summary> 
        public Guid UserId { get; set; }

        /// <summary>
        /// 分类id
        /// </summary> 
        public long CategoryId { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 1=正常 0=删除
        /// </summary> 
        public bool IsValid { get; set; } = true;

        public void SetIsValid(bool v)
        {
            IsValid = v;
		}
    }
}

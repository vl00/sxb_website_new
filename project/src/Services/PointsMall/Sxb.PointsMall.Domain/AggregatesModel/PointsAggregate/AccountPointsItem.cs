using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate
{


    /// <summary>
    /// 账户积分项
    /// </summary>
    public class AccountPointsItem : Entity<Guid>, IAggregateRoot
    {

        public Guid UserId { get; private set; }

        public long Points { get; private set; }

        /// <summary>
        /// 来源类型
        /// </summary>
        public AccountPointsOriginType OriginType { get; private set; }

        /// <summary>
        /// 来源ID
        /// </summary>
        public string OriginId { get; private set; }

        /// <summary>
        /// 是否冻结
        /// </summary>
        public bool IsFreeze { get; private set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; private set; }

        public DateTime ModifyTime { get; private set; }

        /// <summary>
        /// 状态
        /// </summary>
        public AccountPointsItemState State { get; private set; }


        /// <summary>
        /// 父ID
        /// </summary>
        public Guid? ParentId { get; private set; }

        public void SetParent(AccountPointsItem parent)
        {
            this.ParentId = parent.Id;
        }

        /// <summary>
        /// 条目解冻
        /// </summary>
        public void DeFreeze()
        {
            if (!this.IsFreeze || this.Points <= 0 || this.State != AccountPointsItemState.Freeze)
            {
                throw new ArgumentException("当前条目不满足解冻条件");
            }
            this.State = AccountPointsItemState.Normal;
        }


        /// <summary>
        /// 构建一个冻结的项
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="points"></param>
        /// <param name="originType"></param>
        /// <param name="originId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static AccountPointsItem CreateFreezeItem(Guid userId
            , long points
            , AccountPointsOriginType originType
            , string originId
            , string remark)
        {
            var state = points > 0 ? AccountPointsItemState.Freeze : AccountPointsItemState.Normal;
            var freezeItem = new AccountPointsItem(userId, points, originType, originId, true, remark, DateTime.Now, state);
            return freezeItem;
        }

        /// <summary>
        /// 构建一个正常的项
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="points"></param>
        /// <param name="originType"></param>
        /// <param name="originId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static AccountPointsItem CreateNormalItem(Guid userId
    , long points
    , AccountPointsOriginType originType
    , string originId
    , string remark)
        {
            var freezeItem = new AccountPointsItem(userId, points, originType, originId, false, remark, DateTime.Now, AccountPointsItemState.Normal);
            return freezeItem;
        }



        public AccountPointsItem(Guid id, Guid userId, long points
    , AccountPointsOriginType originType, string originId
    , bool isFreeze, string remark, DateTime modifyTime
    , AccountPointsItemState state)
        {
            this.Id = id;
            this.UserId = userId;
            this.Points = points;
            this.OriginType = originType;
            this.OriginId = originId;
            this.IsFreeze = isFreeze;
            this.Remark = remark;
            this.ModifyTime = modifyTime;
            this.State = state;
        }

        public AccountPointsItem(Guid id, Guid userId, int points
            , byte originType, string originId
            , bool isFreeze
            , string remark
            , DateTime modifyTime
            , int state
            , Guid? parentId)
        {
            this.Id = id;
            this.UserId = userId;
            this.Points = points;
            this.OriginType = (AccountPointsOriginType)originType;
            this.OriginId = originId;
            this.IsFreeze = isFreeze;
            this.Remark = remark;
            this.ModifyTime = modifyTime;
            this.State = (AccountPointsItemState)state;
            this.ParentId = parentId;
        }

        public AccountPointsItem(Guid userId, long points
            , AccountPointsOriginType originType, string originId
            , bool isFreeze, string remark, DateTime modifyTime
            , AccountPointsItemState state)
        {
            this.Id = Guid.NewGuid();
            this.UserId = userId;
            this.Points = points;
            this.OriginType = originType;
            this.OriginId = originId;
            this.IsFreeze = isFreeze;
            this.Remark = remark;
            this.ModifyTime = modifyTime;
            this.State = state;
        }



    }
}

using Sxb.Domain;
using Sxb.PointsMall.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate
{

    /// <summary>
    /// 账户积分
    /// </summary>
    public class AccountPoints: Entity<Guid> ,IAggregateRoot
    {

        public Guid UserId { get; private set; }

        /// <summary>
        /// 积分
        /// </summary>
        public long Points { get; private set; }

        /// <summary>
        /// 冻结积分
        /// </summary>

        public long FreezePoints { get; private set; }


        public DateTime ModifyTime { get; private set; }

        /// <summary>
        /// 明细
        /// </summary>

        private List<AccountPointsItem> _accountPointsItems = new List<AccountPointsItem>();

        public IReadOnlyList<AccountPointsItem> AccountPointsItems => _accountPointsItems;


        /// <summary>
        /// 加积分
        /// </summary>
        /// <param name="points">小于0时变为减积分</param>
        /// <param name="originType"></param>
        /// <param name="originId"></param>
        /// <param name="remark"></param>
        public void AddPoints(long points, AccountPointsOriginType originType, string originId, string remark)
        {
            if (points < 0 && (points * -1) > this.Points)
                throw new AccountPointsNotEnoughException("账户积分余额不足以扣除。");
            this.Points += points;
            this.ModifyTime = DateTime.Now;
            var normalItem = AccountPointsItem.CreateNormalItem(this.UserId, points, originType, originId, remark);
            _accountPointsItems.Add(normalItem);

        }

        /// <summary>
        /// 加冻结积分
        /// </summary>
        /// <param name="points">增加账户冻结积分部分</param>
        /// <param name="originType"></param>
        /// <param name="originId"></param>
        /// <param name="remark"></param>
        public Guid AddFreezePoints(long points, AccountPointsOriginType originType, string originId, string remark)
        {
            if (points <= 0) throw new ArgumentException("冻结积分不能小于等于0。");
            this.FreezePoints += points;
            this.ModifyTime = DateTime.Now;
            var freezeItem = AccountPointsItem.CreateFreezeItem(this.UserId, points, originType, originId, remark);
            _accountPointsItems.Add(freezeItem);
            return freezeItem.Id;
        }



        /// <summary>
        /// 冻结积分
        /// </summary>
        /// <param name="points">冻结积分不能小于0</param>
        /// <param name="originType"></param>
        /// <param name="originId"></param>
        /// <param name="remark"></param>
        /// <returns>冻结ID(解冻对应的积分时作校验使用)</returns>
        public Guid PointsToFreezePoints(long points, AccountPointsOriginType originType,string originId,string remark)
        {
            if (points <= 0) throw new ArgumentException("冻结积分不能小于等于0。");
            if (points > this.Points) throw new AccountPointsNotEnoughException("当前账户积分余额不足以冻结该积分额度。");
            this.Points -= points;
            this.FreezePoints  += points;
            this.ModifyTime = DateTime.Now;
            var normalItem = AccountPointsItem.CreateNormalItem(this.UserId, points * -1, originType, originId, remark);
            var freezeItem = AccountPointsItem.CreateFreezeItem(this.UserId, points, originType, originId, remark);
            freezeItem.SetParent(normalItem);
            _accountPointsItems.Add(normalItem);
            _accountPointsItems.Add(freezeItem);
            return freezeItem.Id;
        }
         

        /// <summary>
        /// 解冻积分
        /// </summary>
        /// <param name="points"></param>
        /// <param name="originType"></param>
        /// <param name="originId"></param>
        /// <param name="remark"></param>
        public void DeFreezePoints(AccountPointsItem freezeItem)
        {
            if (freezeItem.Points <= 0) throw new ArgumentException("解冻冻积分不能小于等于0。");
            if (freezeItem.Points > this.FreezePoints) throw new AccountPointsNotEnoughException("当前账户冻结积分余额不足以解冻该积分额度。");
            this.Points += freezeItem.Points;
            this.FreezePoints -= freezeItem.Points;
            this.ModifyTime = DateTime.Now;
            var addpointsItem = AccountPointsItem.CreateNormalItem(this.UserId, freezeItem.Points, freezeItem.OriginType, freezeItem.OriginId, freezeItem.Remark);
            //无源冻结，解冻时才需要跟随源。
            if (freezeItem.ParentId == null)
                addpointsItem.SetParent(freezeItem);
            var defreezeItem = AccountPointsItem.CreateFreezeItem(this.UserId, freezeItem.Points * -1, freezeItem.OriginType, freezeItem.OriginId, freezeItem.Remark);
            defreezeItem.SetParent(addpointsItem);
            _accountPointsItems.Add(addpointsItem);
            _accountPointsItems.Add(defreezeItem);

        }


        /// <summary>
        /// 扣除冻结积分
        /// </summary>
        public void DeductFreezePoints(AccountPointsItem freezeItem, AccountPointsOriginType originType)
        {
            if (freezeItem.Points <= 0) throw new ArgumentException("扣除冻结积分不能小于等于0。");
            if (freezeItem.Points > this.FreezePoints) throw new AccountPointsNotEnoughException("当前账户冻结积分余额不足以扣除该冻结积分额度。");
            this.FreezePoints -= freezeItem.Points;
            this.ModifyTime = DateTime.Now;
            var deductFreezeItem = AccountPointsItem.CreateFreezeItem(this.UserId, freezeItem.Points * -1, originType, freezeItem.OriginId, freezeItem.Remark);
            //有源冻结，扣除时才需要跟随源。
            if (freezeItem.ParentId != null)
                deductFreezeItem.SetParent(freezeItem);
            _accountPointsItems.Add(deductFreezeItem);

        }


        public AccountPoints(Guid userId)
        {
            this.Id = Guid.NewGuid();
            this.UserId = userId;
            this.Points = 0;
            this.FreezePoints = 0;
            this.ModifyTime = DateTime.Now;
        }


        public AccountPoints(Guid userId,long points,long freezePoints,DateTime modifyTime)
        {
            this.Id = Guid.NewGuid();
            this.UserId = userId;
            this.Points = points;
            this.FreezePoints = freezePoints;
            this.ModifyTime = modifyTime;                
        }

        public AccountPoints(Guid id, Guid userId, long points, long freezePoints, DateTime modifyTime)
        {
            this.Id = id;
            this.UserId = userId;
            this.Points = points;
            this.FreezePoints = freezePoints;
            this.ModifyTime = modifyTime;
        }









    }
}

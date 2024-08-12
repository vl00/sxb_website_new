using Sxb.Domain;
using Sxb.SignActivity.Common.Enum;
using Sxb.SignActivity.Domain.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Domain.AggregatesModel.SignAggregate
{
    public class SignIn : Entity<Guid>, IAggregateRoot
    {
        [Column("id")]
        public Guid Id { get; private set; }
        /// <summary>
        /// 业务id
        /// </summary>
        [Column("bu_no")]
        public string BuNo { get; private set; }
        /// <summary>
        /// 签到用户id
        /// </summary>
        [Column("member_id")]
        public Guid MemberId { get; private set; }
        /// <summary>
        /// 签到日期(单位精确到日)
        /// </summary>
        [Column("sign_in_date")]
        public DateTime? SignInDate { get; private set; }
        /// <summary>
        /// 本次签到奖励金币个数
        /// </summary>
        [Column("reward_money")]
        public int RewardMoney { get; private set; }
        /// <summary>
        /// 连续签到天数（A:7天内如果有断签从0开始 B:7天签满从0开始）
        /// </summary>
        [Column("continuite_day")]
        public int ContinuiteDay { get; private set; }

        [Column("Creator")]
        public Guid? Creator { get; private set; }
        [Column("CreateTime")]
        public DateTime? CreateTime { get; private set; }
        [Column("Modifier")]
        public Guid? Modifier { get; private set; }
        [Column("ModifyDateTime")]
        public DateTime? ModifyDateTime { get; private set; }

        [Column("IsValid")]
        public bool IsValid { get; private set; }

        /// <summary>
        /// 签到次数
        /// </summary>
        [Column("sign_count")]
        public int SignCount { get; private set; }
        /// <summary>
        /// 累计奖金
        /// </summary>
        [Column("total_money")]
        public int TotalMoney { get; private set; }
        /// <summary>
        /// 下次签到奖励金币个数
        /// </summary>
        [Column("next_reward_money")]
        public int NextRewardMoney { get; private set; }
        /// <summary>
        /// 剩余签到次数
        /// </summary>
        [Column("left_sign_count")]
        public int LeftSignCount { get; private set; }
        /// <summary>
        /// 扩展字段1
        /// </summary>
        [Column("param1")]
        public string Param1 { get; private set; }
        /// <summary>
        /// 扩展字段2
        /// </summary>
        [Column("param2")]
        public string Param2 { get; private set; }

        /// <summary>
        /// 绑定的父级
        /// </summary>
        [Column("bind_parent_id")]
        public Guid? BindParentId { get; private set; }
        /// <summary>
        /// 绑定父级的时间
        /// </summary>
        [Column("bind_parent_time")]
        public DateTime? BindParentTime { get; private set; }
        /// <summary>
        /// 为父级带来的邀请次数
        /// </summary>
        [Column("bind_parent_invite_count")]
        public int BindParentInviteCount { get; private set; }
        /// <summary>
        /// 为父级带来的累计邀请金额
        /// </summary>
        [Column("bind_parent_total_money")]
        public int BindParentTotalMoney { get; private set; }
        /// <summary>
        /// 为父级解冻的次数
        /// </summary>
        [Column("bind_parent_unfreeze_invite_count")]
        public int BindParentUnfreezeInviteCount { get; private set; }

        protected SignIn()
        { }
        public SignIn(Guid userId, int leftSignCount, string param1, string param2)
        {
            string buNo = BuNoType.SHUANG11_ACTIVITY.ToString();

            Id = Guid.NewGuid();
            BuNo = buNo;
            MemberId = userId;
            ContinuiteDay = 0;
            LeftSignCount = leftSignCount;
            Creator = userId;
            CreateTime = DateTime.Now;
            IsValid = true;
            SignCount = 0;
            Param1 = param1;
            Param2 = param2;

            AddDomainEvent(new NewSignDomainEvent(this));//触发领域事件
        }

        public SignIn SignOnce(DateTime signDate, int[] rewards, Guid modifier)
        {
            SignInDate = signDate;
            if (LeftSignCount <= 0)
            {
                throw new ArgumentOutOfRangeException("无剩余签到次数");
            }

            //暂时这样
            if (signDate.Date.Equals(DateTime.Today.AddDays(-1)))
            {
                ContinuiteDay++;
            }
            else
            {
                ContinuiteDay = 1;
            }

            LeftSignCount--;
            SignCount++;
            var (currentRewardMoney, nextRewardMoney) = CalcMoney(rewards, SignCount);

            RewardMoney = currentRewardMoney;
            NextRewardMoney = nextRewardMoney;
            TotalMoney += RewardMoney;

            Modifier = modifier;
            ModifyDateTime = DateTime.Now;
            return this;
        }

        public static (int current, int next) CalcMoney(int[] rewards, int signCount)
        {
            int current = 0;
            int next = 0;
            if (rewards.Length >= signCount)
            {
                current = rewards[signCount - 1];
            }
            if (rewards.Length >= signCount + 1)
            {
                next = rewards[signCount];
            }
            return (current, next);
        }


        public SignIn BindParent(Guid parentUserId)
        {
            if (BindParentId == null || BindParentId == Guid.Empty)
            {
                BindParentId = parentUserId;
                BindParentTime = DateTime.Now;
                BindParentInviteCount = 0;
                BindParentTotalMoney = 0;
                ModifyDateTime = DateTime.Now;
            }
            return this;
        }

        public (int current, int next) BindParentInviteCountIncrease(int[] parentRewards)
        {
            BindParentInviteCount++;

            var (currentRewardMoney, nextRewardMoney) = CalcMoney(parentRewards, BindParentInviteCount);

            BindParentTotalMoney += currentRewardMoney;
            ModifyDateTime = DateTime.Now;
            return (currentRewardMoney, nextRewardMoney);
        }

        public void UnBlock()
        {
            BindParentUnfreezeInviteCount++;

            ModifyDateTime = DateTime.Now;
        }

        public SignInHistory CreateSignInHistory(string param3)
        {
            var signInHistory = new SignInHistory()
            {
                Id = Guid.NewGuid(),
                BuNo = BuNo,
                MemberId = MemberId,
                SignInDate = SignInDate,
                RewardMoney = RewardMoney,
                ContinuiteDay = ContinuiteDay,
                Creator = Creator,
                CreateTime = DateTime.Now,
                Modifier = Modifier,
                ModifyDateTime = DateTime.Now,
                IsValid = true,
                SignCount = SignCount,
                TotalMoney = TotalMoney,
                NextRewardMoney = NextRewardMoney,
                LeftSignCount = LeftSignCount,
                Param1 = Param1,
                Param2 = Param2,
                Blocked = true,
                Param3 = param3
            };
            return signInHistory;
        }

        public SignInParentHistory CreateSignInParentHistory(int parentRewardMoney, int parentTotalReward, DateTime? signInDate = null)
        {
            var signInParentHistory = new SignInParentHistory()
            {
                Id = Guid.NewGuid(),
                BuNo = BuNo,
                MemberId = MemberId,
                CreateTime = DateTime.Now,
                ModifyTime = DateTime.Now,
                SignInDate = signInDate ?? SignInDate.GetValueOrDefault(),
                ParentRewardMoney = parentRewardMoney,
                ParentInviteCount = BindParentInviteCount,
                ParentTotalMoney = parentTotalReward,
                IsValid = true,
                ParentId = BindParentId.GetValueOrDefault(),
                Blocked = true,
                Param2 = Param2,
                Param3 = ""
            };
            return signInParentHistory;
        }
    }
}

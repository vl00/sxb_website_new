using Sxb.Domain;
using Sxb.PointsMall.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.PointsMall.Domain.AggregatesModel.UserSignInInfoAggregate
{

    /// <summary>
    /// 用户的签到信息表
    /// </summary>
    public class UserSignInInfo : Entity<Guid>, IAggregateRoot
    {

        public Guid UserId { get; private set; }

        /// <summary>
        /// 是否启用签到通知
        /// </summary>
        public bool EnableNotify { get; private set; }

        /// <summary>
        /// 连续天数
        /// </summary>
        public int ContinueDays { get; private set; }

        /// <summary>
        /// 7天周期的第几天 1-7
        /// </summary>
        public int SignInDays => (ContinueDays - 1) % 7 + 1;

        public DateTime ModifyTime { get; private set; }

        private  DateTime? _lastSignDate;
        public DateTime? LastSignDate => _lastSignDate;

        /// <summary>
        /// 上一次的连续天数
        /// </summary>
        private readonly int _preContinueDays;

        public int PreContinueDays => _preContinueDays;

        public UserSignInInfo(Guid id, Guid userId, bool enableNotify, int continueDays, int preContinueDays, DateTime modifyTime, DateTime? lastSignDate = null)
        {
            this.Id = id;
            this.UserId = userId;
            this.EnableNotify = enableNotify;
            this.ContinueDays = continueDays;
            this._preContinueDays = preContinueDays;
            this.ModifyTime = modifyTime;
            this._lastSignDate = lastSignDate;
        }



        public UserSignInInfo(Guid userId, bool enableNotify, int continueDays, int preContinueDays, DateTime modifyTime, DateTime? lastSignDate = null)
        {
            this.Id = Guid.NewGuid();
            this.UserId = userId;
            this.EnableNotify = enableNotify;
            this.ContinueDays = continueDays;
            this._preContinueDays = preContinueDays;
            this.ModifyTime = modifyTime;
            this._lastSignDate = lastSignDate;
        }

        public UserSignInInfo(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new DaySignInException("用户Id为空");
            }

            this.Id = Guid.NewGuid();
            this.UserId = userId;
            this.EnableNotify = false;
            this.ContinueDays = 0;
            this._preContinueDays = 0;
            this.ModifyTime = DateTime.Now;
            this._lastSignDate = null;
        }

        /// <summary>
        /// 日签到
        /// </summary>
        /// <returns></returns>
        public void DaySignIn()
        {
            var now = DateTime.Now;
            if (this.LastSignDate != null && this.LastSignDate.Value.Date == now.Date)
                throw new DaySignInException("今日已签到。");

            if (this.LastSignDate == null || this.LastSignDate.Value.Date < now.AddDays(-1).Date)
                this.ContinueDays = 0;

            this.ContinueDays++;
            this._lastSignDate = DateTime.Now;
            this.ModifyTime = DateTime.Now;


        }

        public string[] SetNotify(bool enabled)
        {
            EnableNotify = enabled;
            ModifyTime = DateTime.Now;
            return new string[] { nameof(EnableNotify), nameof(ModifyTime) };
        }

    }
}

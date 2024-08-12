using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Framework.Foundation
{
    public static class RedisKeys
    {

        #region sign in
        //打卡签到锁
        public const string SignInLockKey = "sign:signin:lock:{0}";
        //打卡成功的最小金额
        public const string SignInAmountLimitKey = "sign:signin:amountlimit";
        //打卡成功的人数
        public const string SignInUserCountKey = "sign:signin:usercount";
        //打卡签到提醒锁
        public const string SignInRemindLockKey = "sign:signin:remind:lock";
        #endregion

        #region points mall
        //领取任务
        public const string TakeViewEvaluationTaskEncrptKey = "pointsmall:task:take:{0}";
        #endregion
    }
}

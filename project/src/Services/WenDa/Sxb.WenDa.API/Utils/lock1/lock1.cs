using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.WenDa.API.Utils
{
    internal interface ILock1Factory
    {
        Task<ILock1> LockAsync(string ck, int expMs = 30000, int retry = 2, int retryDelayMs = 1000);
        Task<ILock1> LockAsync(Lock1Option opt);
    }

    internal interface ILock1 : IAsyncDisposable
    {
        string CK { get; }
        string ID { get; }

        /// <summary>
        /// 锁是否可用.
        /// </summary>
        bool IsAvailable { get; }

        Task<bool> ExtendAsync(int? expMs = null);
    }

    internal class Lock1Option
    {
        public Lock1Option(string ck)
            : this(ck, 30000)
        { }

        public Lock1Option(string ck, TimeSpan exp)
            : this(ck, (int)exp.TotalMilliseconds)
        { }

        public Lock1Option(string ck, int expMs)
        {
            this.CK = ck;
            this.ExpMs = expMs;
            this.Retry = 2;
            this.RetryDelayMs = 1000;
        }

        public string CK { get; set; }
        public int ExpMs { get; set; }
        /// <summary>
        /// 重试次数.<br/>
        /// 0 = 不重试<br/>
        /// -1 = 无限重试<br/>
        /// </summary>
        public int Retry { get; set; }
        public int RetryDelayMs { get; set; }
        public bool RetryDelayIsRandom { get; set; } = true;
        public bool IsLongLck { get; set; }

        public Lock1Option SetExpSec(int sec)
        {
            ExpMs = sec * 1000;
            return this;
        }

        public Lock1Option SetRetry(int retry, int delayMs = 1000, bool delayIsRandom = true)
        {
            Retry = retry;
            RetryDelayMs = delayMs;
            RetryDelayIsRandom = delayIsRandom;
            return this;
        }

        public Lock1Option SetIsLongLck(bool isLongLck = true)
        {
            IsLongLck = isLongLck;
            return this;
        }

        internal Lock1Option SetExpExpectBySec(int expectSec = 30, int actualSec = 60 * 10)
        {
            CK = $"lck1:{actualSec}-{expectSec}:{CK}";
            ExpMs = actualSec * 1000;
            return this;
        }
    }
}

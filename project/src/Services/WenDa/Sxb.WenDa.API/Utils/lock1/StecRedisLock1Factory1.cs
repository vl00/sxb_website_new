using StackExchange.Redis;
using Sxb.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.WenDa.API.Utils
{
	internal class StecRedisLock1Factory1 : ILock1Factory
	{
		private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
		readonly Func<Task<IDatabase>> _factory;

		public StecRedisLock1Factory1(Func<Task<IDatabase>> factory)
		{
			_factory = factory;
		}

		public StecRedisLock1Factory1(IEasyRedisClient easyRedisClient)
		{
			_factory = () => Task.FromResult(easyRedisClient.CacheRedisClient.Database);
		}

		public Task<ILock1> LockAsync(string ck, int expMs = 30000, int retry = 2, int retryDelayMs = 1000)
		{
			return LockAsync(new Lock1Option(ck) { ExpMs = expMs }.SetRetry(retry, retryDelayMs));
		}

		public async Task<ILock1> LockAsync(Lock1Option opt)
		{
			var lckid = Guid.NewGuid().ToString("n");
			Random rnd = null;
			for (int i = 0, c = opt.Retry; ;)
			{
				Exception ex = null;
				try
				{
					var db = await _factory().ConfigureAwait(false);
					if (await db.LockTakeAsync(opt.CK, lckid, TimeSpan.FromMilliseconds(opt.ExpMs)).ConfigureAwait(false))
					{
						break;
					}
				}
				catch (Exception ex0)
				{
					ex = ex0;
				}
				if ((++i) <= c || c == -1) 
				{
					var ms = opt.RetryDelayIsRandom ? (rnd ??= new Random()).Next(0, opt.RetryDelayMs + 1) : opt.RetryDelayMs;
					await Task.Delay(ms).ConfigureAwait(false);
				}
				else
				{
					lckid = null;
					break;
				}
			}
			return lckid != null ? new Lock(this, opt.CK, lckid, opt.ExpMs, opt.IsLongLck) : new Lock(null, null, null, default, default);
		}

		class Lock : ILock1
		{
			StecRedisLock1Factory1 _this;
			readonly int _expMs;
			volatile int _avail;
			string _lckid;
			DateTime _lckAtTime;
			bool _isLongLck;

			public Lock(StecRedisLock1Factory1 factory, string ck, string lckid, int expMs, bool isLongLck)
			{
				_this = factory;
				CK = ck;
				_lckid = lckid;
				_expMs = expMs;
				_isLongLck = isLongLck;

				if (lckid != null)
				{
					_avail = 1;
					_lckAtTime = DateTime.Now;
				}
			}

			public string CK { get; }
			public string ID => _lckid;

			public bool IsAvailable
			{
				get
				{
					if (_lckid == null || _this == null) return false;
					if (_avail == 0) return false;
					return _lckAtTime.AddMilliseconds(_expMs) > DateTime.Now;
				}
			}

			public async Task<bool> ExtendAsync(int? expMs = null)
			{
				if (_avail == 0) return false;
				try
				{
					var pttl = TimeSpan.FromMilliseconds(expMs ?? _expMs);
					var startTimestamp = Stopwatch.GetTimestamp();
					if (await (await _this._factory()).LockExtendAsync(CK, _lckid, pttl))
					{
						if (Interlocked.CompareExchange(ref _avail, 1, 1) == 0)
							return false;

						_lckAtTime = GetRemainingValidityTicks(pttl, startTimestamp, Stopwatch.GetTimestamp());
						return true;
					}
					return false;
				}
				catch
				{
					return false;
				}
			}

			public ValueTask DisposeAsync() => new ValueTask(CoreDisposeAsync());

			async Task CoreDisposeAsync()
			{
				if (_avail == 0 || Interlocked.CompareExchange(ref _avail, 0, 1) == 0)
					return;

				var lckid = _lckid;
				_lckid = null;
				try
				{
					var db = await _this._factory();
					if (_isLongLck) await db.KeyDeleteAsync(CK);
					else await db.LockReleaseAsync(CK, lckid);
				}
				catch { }
				finally { _this = null; }
			}
		}

		static DateTime GetRemainingValidityTicks(TimeSpan expiryTime, long startTimestamp, long endTimestamp)
		{
			var swTicks = (long)(TimestampToTicks * (endTimestamp - startTimestamp));
			var driftTicks = (long)(expiryTime.Ticks * 0.01) + TimeSpan.FromMilliseconds(2).Ticks;
			return DateTime.Now.AddTicks(-1 * swTicks - driftTicks);
		}
	}
}

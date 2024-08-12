using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Framework.Foundation
{
    public class ExcuteTimeHelper
    {
        public static async Task<T> ExcuteTime<T>(Func<Task<T>> func)
        {
            var now1 = DateTime.Now;
            var result = await func();
            var now2 = DateTime.Now;
            //_logger.LogDebug($"总耗时:{(now2 - now1).TotalMilliseconds}毫秒");
            return result;
        }
    }
}

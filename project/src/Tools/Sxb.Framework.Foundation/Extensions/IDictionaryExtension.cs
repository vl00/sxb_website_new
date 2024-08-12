using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxb.Framework.Foundation
{
    public static class IDictionaryExtension
    {
        //
        // 摘要:
        //     Gets the value associated with the specified key.
        //
        // 参数:
        //   key:
        //     The key of the value to get.
        //
        //   def:
        //
        // 返回结果:
        //     the System.Collections.Generic.Dictionary`2 contains an element
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue def = default)
        {
            if (dic.TryGetValue(key, out var value))
            {
                return value;
            }
            return def;
        }
    }
}

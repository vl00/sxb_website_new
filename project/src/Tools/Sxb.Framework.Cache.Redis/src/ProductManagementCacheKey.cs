using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Framework.Cache.Redis
{
    public class ProductManagementCacheKey
    {
        private readonly string[] _keys;

        protected ProductManagementCacheKey(params string[] keys)
        {
            _keys = keys;
        }

        public override string ToString()
        {
            return string.Join(":", _keys);
        }

        public string Key(params string[] keys)
        {
            return new ProductManagementCacheKey(keys).ToString();
        }
    }
}

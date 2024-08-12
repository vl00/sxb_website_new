using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sxb.Framework.AspNetCoreHelper.CheckException
{
    public static class Check
    {
        const string ErrorMsg = "数据错误";

        public static void Throw(string msg)
        {
            if (string.IsNullOrEmpty(msg)) msg = ErrorMsg;

            throw new ResponseResultException(msg, (int)ResponseCode.Failed);
        }

        public static void IsNull(object obj, string msg = "")
        {
            Debug.Assert(obj == null);
            if (obj != null) Throw(msg);
        }

        public static void IsNotNull(object obj, string msg = "")
        {
            Debug.Assert(obj != null);
            if (obj == null) Throw(msg);
        }

        public static void IsTrue(bool obj, string msg = "")
        {
            Debug.Assert(obj);
            if (!obj) Throw(msg);
        }

        public static void IsFalse(bool obj, string msg = "")
        {
            Debug.Assert(!obj);
            if (obj) Throw(msg);
        }

        public static void IsPositiveNumber(long obj, string msg = "")
        {
            Debug.Assert(obj > 0);
            if (obj <= 0) Throw(msg);
        }

        public static void IsEquals(object obj1, object obj2, string msg = "")
        {
            Debug.Assert(obj1.Equals(obj2));
            if (!obj1.Equals(obj2)) Throw(msg);
        }

        public static void IsNotNullOrWhiteSpace(string str, string msg = "")
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(str));
            if (string.IsNullOrWhiteSpace(str)) Throw(msg);
        }

        public static void HasValue<T>(IEnumerable<T> enumerable, string msg = "")
        {
            Debug.Assert(enumerable != null && enumerable.Any());
            if (enumerable == null || !enumerable.Any()) Throw(msg);
        }
    }
}

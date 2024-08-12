using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Sxb.Framework.Foundation
{
    public static class ObjectHelper
    {
        /// <summary>
        /// 返回两个对象属性值不等的描述数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerable<(bool IsModify, string Description)> CompareObjFieldValue<T>(this T source, T target) where T : class
        {
            var result = new List<(bool IsModify, string Description)>();
            if (source == default(T) && target != default(T))
            {
                result.Add((false, "9527"));
                return result;
            }
            if (source == default(T) || target == default(T)) return default;

            var objectType = source.GetType();
            var sourceFields = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(t => t.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() != null).ToList();
            var targetFields = objectType.GetProperties();
            foreach (PropertyInfo property in sourceFields)
            {
                var fieldDescriptionAttr = property.GetCustomAttributes(typeof(DescriptionAttribute), true)?.FirstOrDefault();
                var fieldDescription = string.Empty;
                if (fieldDescriptionAttr != default)
                {
                    fieldDescription = (fieldDescriptionAttr as DescriptionAttribute).Description;
                }
                var sourceValue = property.GetValue(source, null);
                var targetValue = property.GetValue(target, null);
                if (sourceValue == default && targetValue == default) continue;

                if (sourceValue == null && targetValue != null) result.Add((false, fieldDescription));
                if (sourceValue != null && !sourceValue.Equals(targetValue)) result.Add((true, fieldDescription));
            }
            return result;
        }
    }
}

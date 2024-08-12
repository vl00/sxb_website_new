using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sxb.Framework.Foundation;

namespace Sxb.WenDa.Query.ElasticSearch.Base
{

    public class HighlightHelper
    {
        /// <summary>
        /// 根据ES返回的高亮字段, 返回高亮词
        /// </summary>
        /// <param name="high"></param>
        /// <param name="name"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string GetHighLightValue(IReadOnlyDictionary<string, IReadOnlyCollection<string>> high, string name, string def = "")
        {
            string esName = name.ToFirstLower();
            string esNameCntext = $"{esName}.cntext";
            string esNamePinyin = $"{esName}.pinyin";
            var namehigh = high.Where(s => s.Key == esName || s.Key == esNameCntext || s.Key == esNamePinyin).FirstOrDefault().Value;
            var value = namehigh?.FirstOrDefault();
            return value ?? def;
        }

        /// <summary>
        /// <para>added by Labbor on 20201130 设置高亮字段到原数据</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="highs"></param>
        /// <param name="originPropName"></param>
        /// <param name="highlightPropName"></param>
        public static void SetHighlights<T>(ref List<T> source, List<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> highs, string originPropName, string highlightPropName = "")
        {
            Type type = typeof(T);
            var originProp = type.GetProperty(originPropName);
            if (originProp == null)
                throw new ArgumentNullException(nameof(originPropName));

            PropertyInfo highlightProp;
            if (string.IsNullOrWhiteSpace(highlightPropName))
            {
                highlightProp = originProp;
            }
            else
            {
                highlightProp = type.GetProperty(highlightPropName);
                if (highlightProp == null)
                    throw new ArgumentNullException(nameof(highlightPropName));
            }

            var length = source.Count;
            for (int position = 0; position < length; position++)
            {
                var item = source[position];

                var high = highs.ElementAtOrDefault(position);
                var originPropValue = originProp.GetValue(item);
                var originPropStrValue = originPropValue == null ? string.Empty : originPropValue.ToString();

                //把高亮数据赋值到原数据
                var highValue = GetHighLightValue(high, originPropName, def: originPropStrValue);
                highlightProp.SetValue(item, highValue);
            }
        }

        /// <summary>
        /// <para>added by Labbor on 20201130 设置高亮字段到原数据</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="keyword"></param>
        /// <param name="name"></param>
        public static void SetHighlights<T>(ref List<T> source, string keyword, string name)
        {
            if (source == null || !source.Any())
            {
                return;
            }

            //var type = typeof(T);
            var type = source.First().GetType();//解决转为object拿不到
            var prop = type.GetProperty(name);
            if (prop == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var length = source.Count;
            for (int position = 0; position < length; position++)
            {
                var item = source[position];

                var propValue = prop.GetValue(item);
                var propStrValue = propValue == null ? string.Empty : propValue.ToString();

                //把高亮数据赋值到原数据
                var highValue = GetHighLightValue(keyword, propStrValue, def: propStrValue);
                prop.SetValue(item, highValue);
            }
        }

        /// <summary>
        /// 根据搜索keyword, 设置结果中的高亮字
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="name"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string GetHighLightValue(string keyword, string name, string def = "")
        {
            if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(name))
            {
                return def;
            }

            string high = string.Empty;
            List<char> chars = new List<char>();
            foreach (var n in name)
            {
                if (keyword.Any(s => s == n))
                {
                    chars.Add('<');
                    chars.Add('e');
                    chars.Add('m');
                    chars.Add('>');
                    chars.Add(n);
                    chars.Add('<');
                    chars.Add('/');
                    chars.Add('e');
                    chars.Add('m');
                    chars.Add('>');
                }
                else
                {
                    chars.Add(n);
                }
            }
            return new string(chars.ToArray());
        }
    }
}

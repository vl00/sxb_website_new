using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sxb.Framework.Foundation
{
    public static class StringHelper
    {
        public static long ToLong(this string input, long def = 0L)
        {
            if (string.IsNullOrWhiteSpace(input)) return def;

            if (long.TryParse(input, out long result))
                return result;

            return def;
        }

        public static string ToHeadImgUrl(this string input)
        {
            string def = "https://cos.sxkid.com/images/school_v3/head.png";
            return !string.IsNullOrWhiteSpace(input) ? input : def;
        }

        /// <summary>
        /// 将字符串根据长度从头截取 , 加上...返回
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="length">截取的长度`</param>
        /// <returns></returns>
        public static string GetShortString(this string input, int length)
        {
            if (string.IsNullOrWhiteSpace(input) || length < 1) return input;
            string result;
            if (input.Length > length)
            {
                result = input.Substring(0, length) + "...";
            }
            else
            {
                result = input;
            }
            return result;
        }

        /// <summary>
        /// 截取HTML页面的HEAD相关
        /// <para>过滤 空格，回车，双引号，尖括号</para>
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="length">截取长度</param>
        /// <returns></returns>
        public static string GetHtmlHeaderString(this string input, int length)
        {
            if (string.IsNullOrWhiteSpace(input) || length < 1) return input;
            //result = Regex.Replace(input, @"<\/?[p|(span)|b|img].*?\/?>", "", RegexOptions.IgnoreCase);
            string result = input.ReplaceHtmlTag();
            result = result.Replace("\r", "").Replace("\n", "").Replace("\"", "").Replace("<", "")
                .Replace(">", "").Replace("“", "").Replace("”", "").Replace("*", "").Replace(" ", "").Replace("【", "").Replace("】", "");
            return result.GetShortString(length);
        }

        /// <summary>
        /// 字符串的首字母大写转换
        /// </summary>
        /// <returns></returns>
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            return input.Substring(0, 1).ToUpper() + input.Substring(1);
        }

        /// <summary>
        /// 转换为首字母小写的副本
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToFirstLower(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                if (input.Length > 1)
                {
                    return char.ToLower(input[0]) + input.Substring(1);
                }
                return char.ToLower(input[0]).ToString();
            }
            return null;
        }

        /// <summary>
        /// 去除HTML标签
        /// </summary>
        /// <param name="html">带有html标签的文本</param>
        /// <returns></returns>
        public static string ReplaceHtmlTag(this string html)
        {
            //var a = System.Web.HttpUtility.UrlDecode(html);
            //var b = System.Web.HttpUtility.UrlEncode(html);
            string strText = Regex.Replace(html, "<[^>]+>", "");
            strText = Regex.Replace(strText, "&[^;]+;", "");
            return strText;
        }

        /// <summary>
        /// 按照文章列表定义的日期格式规则生成时间格式化
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToArticleFormattString(this DateTime time)
        {
            var passBy_H = (int)(DateTime.Now - time).TotalHours;

            if (passBy_H > 24 || passBy_H < 0)
            {
                //超过24小时,显示日期
                return time.ToString("yyyy年MM月dd日");
            }
            if (passBy_H == 0)
            {
                return time.ToString("刚刚");
            }
            else
            {
                return $"{passBy_H}小时前";
            }
        }

        public static string ToWebUrl(this string url, string domain, string def = "")
        {
            if (string.IsNullOrWhiteSpace(url))
                return def;

            if (url.StartsWith("https://") || url.StartsWith("http://"))
                return url;

            return $"{domain.TrimEnd('/')}/{url.TrimStart('/')}";
        }

        public static string ToJson(this object obj)
        {
            if (obj == null) return null;
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T FromJsonSafe<T>(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
            }
            return default;
        }

        public static IEnumerable<T> FromJsonArrayDynamic<T>(this string json, Func<string, T> func)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return Enumerable.Empty<T>();
            }

            try
            {
                var jArray = JsonConvert.DeserializeObject(json) as JArray;
                return jArray.Where(s => s.Type == JTokenType.String || s.Type == JTokenType.Object)
                   .Select(s =>
                   {
                       if (s.Type == JTokenType.String)
                       {
                           return func(s.ToString());
                       }
                       else
                       {
                           return s.ToObject<T>();
                       }
                   });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Enumerable.Empty<T>();
        }

        public static object FromJson(this string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        /// <summary>
        /// 返回SQL查询的In字符串
        /// </summary>
        /// <returns>例子 -> "('a','b')"</returns>
        public static string ToSQLInString<T>(this IEnumerable<T> source)
        {
            if (source == null || (!source.GetType().IsArray && !source.GetType().IsGenericType)) return null;

            return $"('{string.Join("','", source)}')";
        }

        /// <summary>
        /// 获取本文中所有的Guid(含-)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<Guid> GetAllGuid(this string source)
        {
            if (string.IsNullOrWhiteSpace(source)) return null;
            var matchResult = Regex.Matches(source, @"[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}", RegexOptions.IgnoreCase);
            if (matchResult?.Count > 0)
            {
                var result = new List<Guid>();
                foreach (Match item in matchResult) if (Guid.TryParse(item.Value, out Guid guid)) result.Add(guid);
                return result;
            }
            return null;
        }
    }
}

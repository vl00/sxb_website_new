using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Sxb.Framework.Foundation
{
    public static class RegexHelper
    {
        public static string RegexMatchTemplateData<T>(T data, string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "";

            string result = str;
            //var reg = new Regex("([$]{[\\w-.]+})");
            var reg = new Regex("({[\\w-.]+})");
            var envVars = reg.Matches(result);
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (Match match in envVars)
            {
                var envVarName = match.Value.TrimStart(new char[] { '$', '{' }).TrimEnd('}');
                string envVarValue = string.Empty;
                PropertyInfo p = props.FirstOrDefault(a => a.Name == envVarName);
                if (p != null)
                {
                    envVarValue = Convert.ToString(p.GetValue(data));
                }

                if (!string.IsNullOrEmpty(envVarValue))
                {
                    result = result.Replace(match.Value, envVarValue.Replace(@"\", @"\\"));
                }
            }
            return result;
        }


        /// <summary>
        /// 使用字典中的键值对, 替换源代码串
        /// 不区分关键字大小写
        /// e.g.  你好{Name}!
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RegexMatchTemplateData(Dictionary<string, string> dic, string str)
        {
            string result = str;

            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            if (dic == null)
                return result;

            //var reg = new Regex("([$]{[\\w-.]+})");
            var reg = new Regex("({[\\w-.]+})");
            var envVars = reg.Matches(result);
            foreach (Match match in envVars)
            {
                var envVarName = match.Value.TrimStart(new char[] { '$', '{' }).TrimEnd('}');
                var envVarValue = string.Empty;

                var vars = dic.Where(s => string.Compare(s.Key, envVarName, ignoreCase: true) == 0).Select(s => s.Value);
                if (vars.Any())
                {
                    envVarValue = vars.First();
                }

                if (!string.IsNullOrEmpty(envVarValue))
                {
                    result = result.Replace(match.Value, envVarValue.Replace(@"\", @"\\"));
                }
            }
            return result;
        }
    }
}

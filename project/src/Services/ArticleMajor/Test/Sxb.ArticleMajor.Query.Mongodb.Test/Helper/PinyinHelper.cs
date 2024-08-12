using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb.Test
{
    /// <summary>
    /// 汉字转拼音类
    /// </summary>
    public class PinyinHelper
    {
        /// <summary>
        /// 把汉字转换成拼音(全拼)
        /// </summary>
        /// <param name="hzString">汉字字符串</param>
        /// <returns>转换后的拼音(全拼)字符串</returns>
        public static string ConvertToPinyin(string hzString)
        {
            return string.Join("", hzString.SelectMany(s => {
                var pinyin = new ChineseChar(s).Pinyins?.FirstOrDefault();
                return pinyin?.Take(pinyin.Length - 1);
            }));
        }


        /// <summary>
        /// 把汉字转换成拼音(全拼)
        /// </summary>
        /// <param name="hzString">汉字字符串</param>
        /// <returns>转换后的拼音(全拼)字符串</returns>
        public static string ConvertToPinyinFirst(string hzString)
        {
            return string.Join("", hzString.Select(s => new ChineseChar(s).Pinyins.FirstOrDefault().FirstOrDefault()));
        }
    }
}

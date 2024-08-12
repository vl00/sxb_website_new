using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WenDa.Query.ElasticSearch
{
    public static class FilterConstant
    {
        /// <summary>
        /// 繁简体过滤器
        /// s2t ，将字符从简体中文转换为繁体中文
        /// t2s ，将字符从繁体中文转换为简体中文
        /// https://github.com/medcl/elasticsearch-analysis-stconvert
        /// </summary>
        public static ICharFilter TsFilter => new TSConvertCharFilter("t2s", "#");

        /// <summary>
        /// 拼音过滤器
        /// 取首字母缩写，拼音全连接，每个字拼音，保留原输入，小写的非中文字母，删除重复项
        /// https://github.com/medcl/elasticsearch-analysis-pinyin
        /// </summary>
        public static ITokenFilter PinyinMaxFilter => new PinyinTokenFilter
        {
            KeepSeparateFirstLetter = false,
            KeepJoinedFullPinyin = true,
            KeepFullPinyin = true,
            KeepOriginal = true,
            LimitFirstLetterLength = 16,
            Lowercase = true,
            RemoveDuplicatedTerm = true
        };


        /// <summary>
        /// 取首字母缩写，拼音全连接，不保留原输入，小写的非中文字母，删除重复项
        /// </summary>
        public static ITokenFilter PinyinFilter => new PinyinTokenFilter
        {
            KeepFirstLetter = true,
            KeepNoneChineseTogether = true,
            KeepSeparateFirstLetter = false,
            KeepJoinedFullPinyin = true,
            KeepFullPinyin = false,
            KeepOriginal = true,
            LimitFirstLetterLength = 16,
            Lowercase = true,
            RemoveDuplicatedTerm = true
        };

        public static ITokenFilter PinyinSearchFilter => new PinyinTokenFilter
        {
            KeepFirstLetter = false,
            KeepNoneChineseTogether = true,
            KeepSeparateFirstLetter = false,
            KeepJoinedFullPinyin = true,
            KeepFullPinyin = false,
            KeepOriginal = true,
            LimitFirstLetterLength = 16,
            Lowercase = true,
            RemoveDuplicatedTerm = true
        };

        /// <summary>
        /// 取首字母缩写，拼音全连接，不保留原输入，小写的非中文字母，删除重复项
        /// </summary>
        public static ITokenFilter PinyinOnlyFilter => new PinyinTokenFilter
        {
            KeepFirstLetter = true,
            KeepSeparateFirstLetter = false,
            KeepJoinedFullPinyin = true,
            KeepFullPinyin = false,
            KeepOriginal = false,
            LimitFirstLetterLength = 16,
            Lowercase = true,
            RemoveDuplicatedTerm = true
        };

        /// <summary>
        /// 取首字母缩写，拼音全连接，保留原输入，小写的非中文字母，删除重复项
        /// </summary>
        public static ITokenFilter PinyinKeywordFilter => new PinyinTokenFilter
        {
            KeepFirstLetter = true,
            KeepSeparateFirstLetter = false,
            KeepJoinedFullPinyin = true,
            KeepFullPinyin = false,
            KeepOriginal = true,
            LimitFirstLetterLength = 16,
            Lowercase = true,
            RemoveDuplicatedTerm = true
        };
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Elasticsearch.Net.Utf8Json;
using Nest;

namespace Sxb.WenDa.Query.ElasticSearch
{
    public class PinyinTokenFilter : TokenFilterBase
    {
        public PinyinTokenFilter() : base("pinyin")
        {
        }

        [PropertyName("keep_first_letter")]
        public bool KeepFirstLetter { get; set; }

        [PropertyName("keep_none_chinese_together")]
        public bool KeepNoneChineseTogether { get; set; } = true;


        [PropertyName("keep_separate_first_letter")]
        public bool KeepSeparateFirstLetter { get; set; }

        [PropertyName("limit_first_letter_length")]
        public int LimitFirstLetterLength { get; set; }

        [PropertyName("keep_joined_full_pinyin")]
        public bool KeepJoinedFullPinyin { get; set; }

        [PropertyName("keep_full_pinyin")]
        public bool KeepFullPinyin { get; set; }

        [PropertyName("keep_original")]
        public bool KeepOriginal { get; set; }

        [PropertyName("lowercase")]
        public bool Lowercase { get; set; }

        [PropertyName("remove_duplicated_term")]
        public bool RemoveDuplicatedTerm { get; set; }
    }
}

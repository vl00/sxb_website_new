using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WenDa.Query.ElasticSearch
{
    public class AnalyzerConstant
    {
        public static Func<AnalyzersDescriptor, IPromise<IAnalyzers>> CommonAnalyzer => aa => aa
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("pinyin_search_filter", "lowercase")
                            );

        public static Func<CustomAnalyzerDescriptor, ICustomAnalyzer> IkAnalyzer => ca =>ca.CharFilters("tsconvert").Tokenizer("ik_max_word").Filters("lowercase");
        public static Func<CustomAnalyzerDescriptor, ICustomAnalyzer> IkSearchAnalyzer => ca =>ca.CharFilters("tsconvert").Tokenizer("ik_smart").Filters("lowercase");

        public static Func<CustomAnalyzerDescriptor, ICustomAnalyzer> IkPinyinAnalyzer => ca =>ca.CharFilters("tsconvert").Tokenizer("ik_max_word").Filters("pinyin_max_filter", "lowercase");
        public static Func<CustomAnalyzerDescriptor, ICustomAnalyzer> IkPinyinSearchAnalyzer => ca =>ca.CharFilters("tsconvert").Tokenizer("ik_smart").Filters("pinyin_search_filter", "lowercase");

    }
}

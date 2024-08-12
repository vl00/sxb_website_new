using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Query.ElasticSearch
{

    public class AnalyzeBuilder
    {
        private static Func<TokenFiltersDescriptor, IPromise<ITokenFilters>> GetTokenFilters()
        {
            return tf => tf
                    .UserDefined("pinyin_max_filter", FilterConstant.PinyinMaxFilter)
                    .UserDefined("pinyin_keyword_filter", FilterConstant.PinyinKeywordFilter)
                    .UserDefined("pinyin_only_filter", FilterConstant.PinyinOnlyFilter)
                    .UserDefined("pinyin_filter", FilterConstant.PinyinFilter)
                    .UserDefined("pinyin_search_filter", FilterConstant.PinyinSearchFilter)
                    //.RemoveDuplicates("remove_duplicates")
                    .EdgeNGram("edge_ngram_filter",
                        etf => etf.MinGram(1).MaxGram(50)
                    )
                    .NGram("ngram_filter",
                        etf => etf.MinGram(1).MaxGram(2)
                    )
                ;
        }

        private static Func<AnalyzersDescriptor, IPromise<IAnalyzers>> GetAnalyzers()
        {
            return aa => aa
                    //不分词，仅转小写
                    .Custom("string_lowercase", ca => ca
                            .Tokenizer("keyword")
                            .Filters("lowercase")
                        )
                    .Custom(IkNgramAnalyzer, ca => ca
                            .CharFilters("tsconvert")
                            .Tokenizer("ik_max_word")
                            .Filters("ngram_filter", "lowercase") //"remove_duplicates"
                        )
                    .Custom(IkSearchAnalyzer, ca => ca
                            .CharFilters("tsconvert")
                            .Tokenizer("ik_smart")
                            .Filters("lowercase")
                        )
                    .Custom(IkPinyinAnalyzer, ca => ca
                            .CharFilters("tsconvert")
                            .Tokenizer("ik_max_word")
                            .Filters("edge_ngram_filter", "pinyin_filter", "lowercase")
                        )
                    .Custom("ik_pinyin_only_analyzer", ca => ca
                            .CharFilters("tsconvert")
                            .Tokenizer("ik_max_word")
                            .Filters("pinyin_only_filter", "edge_ngram_filter", "lowercase")
                        )
                    .Custom(IkPinyinSearchAnalyzer, ca => ca
                            .CharFilters("tsconvert")
                            .Tokenizer("ik_smart")
                            .Filters("pinyin_search_filter", "lowercase")
                        )
                    .Custom("ngram_analyzer", ca => ca
                            .CharFilters("tsconvert")
                            .Tokenizer("ngram_tokenizer")
                            .Filters("lowercase")
                        )
                    .Custom("ngram_pinyin_analyzer", ca => ca
                            .CharFilters("tsconvert")
                            .Tokenizer("ngram_tokenizer")
                            .Filters("pinyin_filter", "lowercase")
                        )
                    //不分词，转拼音，EdgeNgram分割
                    .Custom(KeywordPinyinNgramAnalyzer, ca => ca
                            .CharFilters("tsconvert")
                            .Tokenizer("keyword")
                            .Filters("pinyin_filter", "edge_ngram_filter", "lowercase")
                        )
                    //不分词，转小写，繁转中
                    .Custom("keyword_search_analyzer", ca => ca
                            .CharFilters("tsconvert")
                            .Tokenizer("keyword")
                            .Filters("lowercase")
                        )
                    //不分词，转拼音，拼音首字母
                    .Custom("keyword_pinyin_search_analyzer", ca => ca
                            .CharFilters("tsconvert")
                            .Tokenizer("keyword")
                            .Filters("pinyin_keyword_filter", "edge_ngram_filter", "lowercase")
                        )
                    .Custom("ngram_2_analyzer", ca => ca
                            .CharFilters("tsconvert")
                            .Tokenizer("keyword")
                            .Filters("lowercase", "ngram_filter")
                    )
                ;
        }

        public static string NgramAnalyzer = "ngram_analyzer";

        public static string IkNgramAnalyzer = "ik_analyzer";
        public static string IkSearchAnalyzer = "ik_search_analyzer";

        public static string IkPinyinAnalyzer = "ik_pinyin_analyzer";
        public static string IkPinyinSearchAnalyzer = "ik_pinyin_search_analyzer";

        public static string KeywordPinyinNgramAnalyzer = "keyword_pinyin_ngram_analyzer";

        public static Func<AnalysisDescriptor, IAnalysis> GetAnalysis()
        {
            return a => a
                    .CharFilters(cf => cf
                        .UserDefined("tsconvert", FilterConstant.TsFilter)
                    )
                    .TokenFilters(GetTokenFilters())
                    .Tokenizers(t =>
                        t.EdgeNGram("edge_ngram_tokenizer", nt =>
                            nt.MinGram(1).MaxGram(6)
                            .TokenChars(TokenChar.Letter, TokenChar.Digit)
                        )
                        .NGram("ngram_tokenizer", nt =>
                            nt.MaxGram(2)
                        )
                    )
                    .Analyzers(GetAnalyzers())
           ;
        }
    }
}

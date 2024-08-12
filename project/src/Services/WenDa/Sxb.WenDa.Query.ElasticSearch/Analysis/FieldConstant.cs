using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WenDa.Query.ElasticSearch
{
    public static class FieldConstant
    {

        public static TDescriptor Pinyin<TDescriptor, TInterface, T>(this CorePropertyDescriptorBase<TDescriptor, TInterface, T> cpd)
            where TDescriptor : CorePropertyDescriptorBase<TDescriptor, TInterface, T>, TInterface
            where TInterface : class, ICoreProperty
            where T : class
        {
            return cpd.Fields(f => f.Text(
                                ft => ft.Name("pinyin")
                                 //设置keyword的相关性
                                 //.Similarity("un_norms_similarity")
                                 .Analyzer("ik_pinyin_analyzer")
                                 .SearchAnalyzer("ik_pinyin_search_analyzer")
                           ));
        }


        public static TDescriptor SearchText<TDescriptor, TInterface, T>(this CorePropertyDescriptorBase<TDescriptor, TInterface, T> cpd)
            where TDescriptor : CorePropertyDescriptorBase<TDescriptor, TInterface, T>, TInterface
            where TInterface : class, ICoreProperty
            where T : class
        {
            return cpd.Fields(f => f
                        .Text(ft => ft
                            .Name("cntext")
                            .Analyzer(AnalyzeBuilder.IkNgramAnalyzer)
                            .SearchAnalyzer(AnalyzeBuilder.IkSearchAnalyzer))
                        .Text(ft => ft
                            .Name("pinyin")
                            .Analyzer(AnalyzeBuilder.IkPinyinAnalyzer)
                            .SearchAnalyzer(AnalyzeBuilder.IkPinyinSearchAnalyzer))
                        .Text(ft => ft
                            .Name("keyword")
                            .Analyzer(AnalyzeBuilder.KeywordPinyinNgramAnalyzer)
                            .SearchAnalyzer(AnalyzeBuilder.IkSearchAnalyzer))
                    );
        }
    }
}

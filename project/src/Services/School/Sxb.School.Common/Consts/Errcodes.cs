using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Sxb.School.Common.Consts
{
    public static class Errcodes
    {
        /// <summary>题目已更新</summary>
        public const int DgAyQIsChanged = 1101001;
        /// <summary>题目选项已更新</summary>
        public const int DgAyQoptIsChanged = 1101002;
        /// <summary>题目选项分数已更新</summary>
        public const int DgAyQoptPointIsChanged = 1101003;
        /// <summary>跳题关系更新</summary>
        public const int DgAyNextqidIsChanged = 1101004;
        /// <summary>保存用户答题失败</summary>
        public const int DgAySaveQaError = 1101005;
        /// <summary>分析用户答题失败</summary>
        public const int DgAyQaAnalyzeError = 1101006;
        /// <summary>不是我的问卷+报告</summary>
        public const int DgAyTheQapaperIsNotMy = 1101007;
        /// <summary>提交问卷时终端类型填错了</summary>
        public const int DgAySubmitQaTermtypError = 1101008;
    }
}

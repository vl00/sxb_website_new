namespace Sxb.WenDa.Common.Consts
{
    public static class Errcodes
    {
        ///// <summary></summary>
        //public const int Wenda_ = 1201000;

        public const int Wenda_WriteReadNotSync = 4994;
        public const int Wenda_CallApiError = 4995;
        public const int Wenda_GetLck1Failed = 44400;

        /// <summary>问答广场-(已开通的)城市</summary>
        public const int Wenda_NotFound = 1201000;
        /// <summary>已存在相同标题的问题</summary>
        public const int Wenda_QuesTitleExists = 1201001;
        /// <summary>该城市的问答广场未开放</summary>
        public const int Wenda_CityIsNotOpen = 1201002;
        /// <summary>专栏不存在</summary>
        public const int Wenda_SubjectNotExists = 1201003;
        /// <summary>分类不存在</summary>
        public const int Wenda_CategoryNotExists = 1201004;
        /// <summary>城市与分类不匹配</summary>
        public const int Wenda_CityNotHasThisCategory = 1201005;
        /// <summary>分类还未选完,下级还有分类</summary>
        public const int Wenda_CategoryHasChildCategory = 1201006;
        /// <summary>标签参数错误</summary>
        public const int Wenda_TagIdError = 1201007;
        /// <summary>不是我的问题</summary>
        public const int Wenda_IsNotMyQuestion = 1201008;
        /// <summary>问题不存在</summary>
        public const int Wenda_QuestionNotExists = 1201009;

        /// <summary>回答不存在</summary>
        public const int Wenda_AnswerNotExists = 1201201;
        /// <summary>不是我的回答</summary>
        public const int Wenda_IsNotMyAnswer = 1201202;

        /// <summary>评论不存在</summary>
        public const int Wenda_CommentNotExists = 1201301;

        public const int Wenda_UserGzError = 1201302;
        public const int Wenda_RealUserError = 1201303;
    }
}

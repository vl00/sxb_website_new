using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.API.RequestContact.Wenda
{
    /// <summary>
    /// 我的-待回答
    /// </summary>
    public class QuestionsToBeAnsweredVm : Page<ToBeAnsweredQuestionListItemDto>
    {
        public QuestionsToBeAnsweredVm() : base() { }

        /// <summary>
        /// 邀请我来回答列表第一页 推荐回答第一页才返回数据
        /// </summary>
        public Page<ToBeAnsweredQuestionListItemDto> InvitedMePage1 { get; set; } = null;

        /// <summary>
        /// 用户擅长领域 
        /// </summary>
        public IEnumerable<AttentionCategoryDto> UserCategoryAttentions { get; set; }
    }
}

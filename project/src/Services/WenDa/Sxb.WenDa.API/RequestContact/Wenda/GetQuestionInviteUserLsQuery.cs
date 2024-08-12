using Sxb.WenDa.Common.ResponseDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sxb.WenDa.API.RequestContact.Wenda
{
    public class GetQuestionInviteUserLsQuery
    {
        /// <summary>
        /// 问题id
        /// </summary>
        [Required]
        public Guid QuestionId { get; set; }
    }

    public class GetQuestionInviteUserLsQueryResult
    {
        /// <summary>
        /// 目前最多显示10个用户
        /// </summary>
        public List<GetQuestionInviteUserLsItemDto> Items { get; set; }
    }
}

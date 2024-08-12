using Sxb.WenDa.Common.OtherAPIClient.User.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class GetQuestionInviteUserLsItemDto
    {
        /// <summary>
        /// true=已邀请; false=未邀请
        /// </summary>
        public bool IsInvited { get; set; }

        /// <summary>用户id</summary>
        public Guid UserId { get; set; }
        /// <summary>用户名</summary>
        public string UserName { get; set; }
        /// <summary>用户头像</summary>
        public string HeadImg { get; set; }
        /// <summary>用户描述</summary>
        public string Desc { get; set; }

        public GetQuestionInviteUserLsItemDto() { }

        public GetQuestionInviteUserLsItemDto(UserDescDto s)
        {
            this.UserId = s.Id;
            this.UserName = s.Name;
            this.HeadImg = s.HeadImg;
            this.Desc = s.CertificationPreview;
        }
    }
}

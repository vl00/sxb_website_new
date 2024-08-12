using System;
using System.Collections.Generic;

namespace Sxb.WeWorkFinance.API.Application.Models
{
    public class OrgResultModel<T>
    {
        public bool Succeed { get; set; }

        /// <summary>
        /// 返回错误码
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }


        /// <summary>
        /// 返回Model
        /// </summary>
        public T Data { get; set; }

        
    }
    public class InviteActivityCourseResultModel
    {
        public List<InviteActivityCourseModel> Courses { get; set; }
    }
    public class InviteActivityCourseModel
    {
        public Guid Id { get; set; }
        public string Id_s { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int ExchangeType { get; set; }
        public int ExchangePoint { get; set; }

        public List<string> ExchangeKeywords { get; set; }
    }
}

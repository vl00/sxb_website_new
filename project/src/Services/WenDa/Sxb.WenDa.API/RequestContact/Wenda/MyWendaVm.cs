using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.API.RequestContact.Wenda
{
    /// <summary>
    /// 我的-提问数+回答数+获赞数
    /// </summary>
    public class MyWendaVm
    {
        /// <summary>用户id</summary>
        public Guid Id { get; set; }
        /// <summary>用户名</summary>
        public string Name { get; set; }
        /// <summary>用户头像</summary>
        public string HeadImg { get; set; }
        /// <summary>是否真实用户</summary>
        public bool IsReal { get; set; }

        /// <summary>提问数</summary>
        public int QuestionCount { get; set; }
        /// <summary>回答数</summary>
        public int AnswerCount { get; set; }
        /// <summary>
        /// 获赞数(包括评论中的获赞数)<br/>
        /// 别人点赞我(我的回答+我的评论), 我自己点赞我自己的也算
        /// </summary>
        public int GetLikeCount { get; set; }
    }
}

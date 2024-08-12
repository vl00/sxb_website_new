using MediatR;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Sxb.WenDa.API.Application.Commands
{
    public class AddQuestionCommand : IRequest<AddQuestionCommandResult>
    {
        /// <summary>用户id(后端字段,前端不管)</summary>
        [JsonIgnore]
        public Guid UserId { get; set; }

        /// <summary>城市编号</summary>
        [Required]
        public long City { get; set; }

        /// <summary>
        /// 专栏id（长短都行）<br/>
        /// 在专栏里提问时需要传此参数
        /// </summary>
        public string SubjectId { get; set; } = null;

        /// <summary>
        /// 问题标题
        /// </summary>
        [Required]
        public string Title { get; set; } // <=40字符
        /// <summary>问题补充说明</summary>
        public string Content { get; set; }
        /// <summary>图片s</summary>
        public string[] Imgs { get; set; } // <=6
        /// <summary>图片缩略图s</summary>
        public string[] Imgs_s { get; set; }

        /// <summary>
        /// 问题分类id (就是最后选择的2级/3级)
        /// </summary>
        [Required]
        public long CategoryId { get; set; }
        /// <summary>
        /// 问题标签id
        /// </summary>
        public long[] TagIds { get; set; } = null;
        /// <summary>
        /// 学校eids。分类选了学校问答时必填
        /// </summary>
        public Guid[] Eids { get; set; } = null; // <=3

        /// <summary>
        /// true=匿名; false=非匿名
        /// </summary>
        public bool IsAnony { get; set; }
    }

    public class AddQuestionCommandResult
    {
        /// <summary>问题id</summary>
        public Guid QuestionId { get; set; }
        /// <summary>问题短id</summary>
        public string QuestionNo { get; set; }

        ///// <summary>是否关注公众号</summary>
        //public bool HasGzWxGzh { get; set; }
        ///// <summary>是否已加企业微信客服</summary>
        //public bool HasJoinWxEnt { get; set; }
    }
}

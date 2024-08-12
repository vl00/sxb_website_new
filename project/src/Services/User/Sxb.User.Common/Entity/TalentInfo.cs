using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.User.Common.Entity
{
    [Display(Rename = "Talent")]
    public class TalentInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Rename = "user_id")]
        public Guid UserID { get; set; }
        /// <summary>
        /// 认证类型 0:个人身份认证，1机构身份认证，99上学帮内部认证
        /// </summary>
        public int Certification_Type { get; set; }
        /// <summary>
        /// 认证方式 0:审核认证 1邀请认证 2.机构邀请
        /// </summary>
        public int Certification_Way { get; set; }
        /// <summary>
        /// 认证身份
        /// </summary>
        public string Certification_Identity { get; set; }
        /// <summary>
        /// 认证称号
        /// </summary>
        public string Certification_Title { get; set; }
        /// <summary>
        /// 认证说明 0：认证称号，1前缀+认证称号 2 认证称号+后缀
        /// </summary>
        public int Certification_Explanation { get; set; }
        /// <summary>
        /// 认证说明预览
        /// </summary>
        public string Certification_Preview { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Createdate { get; set; }
        /// <summary>
        /// 认证时间或有效时间
        /// </summary>
        public DateTime Certification_Date { get; set; }
        /// <summary>
        /// 删除标记 0否，1是
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 达人类型 0个人 1机构  
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 机构全称
        /// </summary>
        public string Organization_Name { get; set; }
        /// <summary>
        /// 统一社会信用码
        /// </summary>
        public string Organization_Code { get; set; }
        /// <summary>
        /// 运营人员姓名
        /// </summary>
        public string Operation_Name { get; set; }
        /// <summary>
        /// 运营人员手机号
        /// </summary>
        public string Operation_Phone { get; set; }
        /// <summary>
        /// 认证状态 0未审核，1已通过，2已驳回
        /// </summary>
        public int Certification_Status { get; set; }
        /// <summary>
        /// 状态 0:禁用 1启用
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 补充说明
        /// </summary>
        public string Supplementary_Explanation { get; set; }
        /// <summary>
        /// 机构员工最大值
        /// </summary>
        public int Organization_Staff_Count { get; set; }
        /// <summary>
        /// 认证称号id
        /// </summary>
        public Guid Certification_Identity_ID { get; set; }
        /// <summary>
        /// 邀请认证状态 0 非邀请  1 邀请中 2 邀请成功 3 邀请失效
        /// </summary>
        public int Invite_Status { get; set; }
        /// <summary>
        /// 是否内部达人
        /// </summary>
        public bool IsInternal { get; set; }
        /// <summary>
        /// 达人邮箱
        /// </summary>
        public string Email { get; set; }
    }
}

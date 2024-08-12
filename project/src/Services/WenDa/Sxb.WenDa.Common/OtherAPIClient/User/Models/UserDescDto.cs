namespace Sxb.WenDa.Common.OtherAPIClient.User.Models
{
    public class UserDescDto
    {
        /// <summary>用户id</summary>
        public Guid Id { get; set; }
        /// <summary>用户名</summary>
        public string Name { get; set; }
        /// <summary>用户头像</summary>
        public string HeadImg { get; set; }

        /// <summary>认证身份</summary>
        public string CertificationIdentity { get; set; }
        /// <summary>认证称号</summary>
        public string CertificationTitle { get; set; }
        /// <summary>认证title预览</summary>
        public string CertificationPreview { get; set; }
    }

    /// <summary>
    /// 达人用户描述
    /// </summary>
    public class TalentUserDescDto : UserDescDto
    {
        /// <summary>true=是内部达人</summary>
        public bool IsInternal { get; set; }
    }
}

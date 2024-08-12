using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sxb.Domain;
using Sxb.User.Domain.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Sxb.User.Domain.AggregatesModel.TalentAggregate
{
    public class Talent : Entity<Guid>, IAggregateRoot
    {
        [Column("user_id")]
        public Guid UserId { get; private set; }
        [Column("certification_type")]
        public int CertificationType { get; private set; }
        [Column("certification_way")]
        public int CertificationWay { get; private set; }
        [Column("certification_identity")]
        public string CertificationIdentity { get; private set; }
        [Column("certification_title")]
        public string CertificationTitle { get; private set; }
        [Column("certification_explanation")]
        public int CertificationExplanation { get; private set; }
        [Column("certification_preview")]
        public string CertificationPreview { get; private set; }
        [Column("createdate")]
        public DateTime? Createdate { get; private set; }
        [Column("certification_date")]
        public DateTime? CertificationDate { get; private set; }
        [Column("type")]
        public int Type { get; private set; }
        [Column("organization_name")]
        public string OrganizationName { get; private set; }
        [Column("organization_code")]
        public string OrganizationCode { get; private set; }
        [Column("operation_name")]
        public string OperationName { get; private set; }
        [Column("operation_phone")]
        public string OperationPhone { get; private set; }
        [Column("certification_status")]
        public int CertificationStatus { get; private set; }
        [Column("status")]
        public int Status { get; private set; }
        [Column("supplementary_explanation")]
        public string SupplementaryExplanation { get; private set; }
        [Column("organization_staff_count")]
        public int OrganizationStaffCount { get; private set; }
        [Column("certification_identity_id")]
        public Guid CertificationIdentityId { get; private set; }
        [Column("invite_status")]
        public int InviteStatus { get; private set; }
        [Column("isdelete")]
        public bool Isdelete { get; private set; }

        protected Talent()
        { }
        public Talent(Guid id,Guid userId, int certificationType, string certificationIdentity, string certificationTitle)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            UserId = userId;
            CertificationType = certificationType;
            CertificationIdentity = certificationIdentity;
            CertificationTitle = certificationTitle;
            Createdate = DateTime.Now;
            this.AddDomainEvent(new SampleDomainEvent(this));//触发领域事件
        }

    }
}

using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Domain.AggregatesModel.ContactAggregate
{
    public class CustomerQrCode : Entity<string>, IAggregateRoot
    {
        public string AdviserId { get; private set; }
        public Guid? AdviserUserId { get; private set; }
        public string InviterId { get; private set; }
        public string InviterOpenId { get; private set; }

        public string ConfigId { get; private set; }
        public string QrcodeUrl { get; private set; }

        public bool IsDeletedConfigId { get; private set; }

        public DateTime CreateTime { get; private set; }


        public void Create(string id ,string adviserId,Guid adviserUserId, string inviterId, string inviterOpenId, string configId,string qrcodeUrl)
        {
            Id = id;
            AdviserId = adviserId;
            AdviserUserId = adviserUserId;
            InviterId = inviterId;
            InviterOpenId = inviterOpenId;
            ConfigId = configId;
            QrcodeUrl = qrcodeUrl;
            CreateTime = DateTime.Now;
        }

        public void Update( string configId, string qrcodeUrl)
        {
            ConfigId = configId;
            QrcodeUrl = qrcodeUrl;
        }

        public void DeletedConfigId()
        {
            IsDeletedConfigId = true;
        }
    }
}

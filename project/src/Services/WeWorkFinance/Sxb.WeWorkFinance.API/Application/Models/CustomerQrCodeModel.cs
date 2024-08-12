using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.Models
{
    public class CustomerQrCodeModel
    {
        public string AdviserId { get; set; }
        public Guid AdviserUserId { get; set; }
        public string InviterId { get; set; }
        public string InviterOpenId { get; set; }

        public string ConfigId { get; set; }
        public string QrcodeUrl { get; set; }

    }
}

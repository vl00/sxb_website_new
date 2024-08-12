using System;

namespace Sxb.User.API.RequestContract.Talent
{
    public class GetExtensionTalentUserIDRequest
    {
        /// <summary>
        /// 学部ID
        /// </summary>
        public Guid EID { get; set; }
    }
}

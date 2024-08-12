using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.User.Common.Entity
{
    public class UserInfo
    {
        [Identity]
        public Guid ID { get; set; }
        public int NationCode { get; set; }
        public string Mobile { get; set; }
        public Guid Password { get; set; }
        public string Nickname { get; set; }
        public DateTime RegTime { get; set; }
        public DateTime LoginTime { get; set; }
        public bool Blockage { get; set; }
        public string HeadImgUrl { get; set; }
        public int Sex { get; set; }
        public int City { get; set; }
        public string Channel { get; set; }
        public string Introduction { get; set; }
        public string Source { get; set; }
        public int Client { get; set; }
        public string Remark { get; set; }
    }
}

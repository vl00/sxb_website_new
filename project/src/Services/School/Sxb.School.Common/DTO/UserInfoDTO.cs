using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;

namespace Sxb.School.Common.DTO
{
    public class UserInfoDTO
    {
        public UserInfoDTO()
        {
            Role = new List<int>();
        }

        public Guid Id { get; set; }
        public string NickName { get; set; }
        public List<int> Role { get; set; }
        public string HeadImager { get; set; }

        public bool IsSchool => Role.Contains((int)UserRoleType.School);
        public bool IsTalent => Role.Contains((int)UserRoleType.PersonTalent) || Role.Contains((int)UserRoleType.OrgTalent);
    }
}

using Sxb.PointsMall.API.Application.Queries.AccountPoints;
using Sxb.PointsMall.API.Application.Queries.UserPointsTask;
using Sxb.PointsMall.API.Application.Queries.UserSignInInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Models
{
    public record PointsDetailData
    {

        public AccountPoints AccountPoints { get; set; }

        public UserSignInInfo  UserSignInInfo { get; set; }

        public IEnumerable<PointsTasksOfUser> UserPointsTaskItems { get; set; }

    }
}

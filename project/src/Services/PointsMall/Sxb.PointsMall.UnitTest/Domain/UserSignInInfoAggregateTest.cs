using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sxb.PointsMall.Domain.AggregatesModel.UserSignInInfoAggregate;
using Sxb.PointsMall.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.UnitTest.Domain
{
    [TestClass]
    public class UserSignInInfoAggregateTest
    {
        [TestMethod]
        public void DaySignIn_FirstSign_Test()
        {
            var uesrId = Guid.NewGuid();
            UserSignInInfo userSignInInfo = new UserSignInInfo(uesrId);
            userSignInInfo.DaySignIn();
            Assert.IsTrue(true);
        }


        /// <summary>
        /// 当前已签到
        /// </summary>
        [TestMethod]
        public void DaySignIn_SecondSign_HasSignToDay_Test()
        {
            Assert.ThrowsException<DaySignInException>(() =>
            {
                var uesrId = Guid.NewGuid();
                UserSignInInfo userSignInInfo = new UserSignInInfo(uesrId, false, 1, 0, DateTime.Now, DateTime.Now);
                userSignInInfo.DaySignIn();
            });
        }

        /// <summary>
        /// 当前没有签到
        /// </summary>
        [TestMethod]
        public void DaySignIn_SecondSign_NotSignToDay_Test()
        {
            var uesrId = Guid.NewGuid();
            UserSignInInfo userSignInInfo = new UserSignInInfo(uesrId, false, 1, 0, DateTime.Now,DateTime.Now.AddDays(-1));
            userSignInInfo.DaySignIn();
            Assert.IsTrue(true);
        }


        /// <summary>
        /// 没有签到尝试签到两次
        /// </summary>
        [TestMethod]
        public void DaySignIn_SecondSign_NotSignTodayAndSignTwoTimes_Test()
        {

            Assert.ThrowsException<DaySignInException>(() =>
            {
                var uesrId = Guid.NewGuid();
                UserSignInInfo userSignInInfo = new UserSignInInfo(uesrId, false, 1, 0, DateTime.Now, DateTime.Now.AddDays(-1));
                userSignInInfo.DaySignIn();
                userSignInInfo.DaySignIn();
            });

        }

    }
}

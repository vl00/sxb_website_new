using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sxb.Static.BackgroundTask.School.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.UnitTest
{
    [TestClass]
    public class ISchoolQueriesTest
    {

        [TestMethod]
        public async Task GetSchoolFromNo_Test()
        {
            ISchoolQueries schoolQueries = new SchoolQueries("Server=10.1.0.199;database=iSchoolData;user id=iSchool;password=SxbLucas$0769;MultipleActiveResultSets=true;Max Pool Size = 10000;");
            var  school  = await schoolQueries.GetSchoolFromNoAsync(464175);
            Assert.IsNotNull(school);
        }
    }
}

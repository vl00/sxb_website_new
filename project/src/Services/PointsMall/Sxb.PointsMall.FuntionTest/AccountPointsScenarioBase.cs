using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sxb.PointsMall.API;
using Microsoft.Extensions.Hosting;
using Sxb.PointsMall.API.Application.Commands;

namespace Sxb.PointsMall.FuntionTest
{
    public class AccountPointsScenarioBase
    {
        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(AccountPointsScenarioBase)).Location;
        
            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                }).UseStartup<AccountPointsTestStartup>();


            var testServer = new TestServer(hostBuilder);
          
          
            return testServer;
        }


    }



    public static class Get {
        public static string Home => "/";

        public static string CreateAccountPoints => "/api/AccountPoints/Create";

        public static string PointsDetail => "/api/AccountPoints/Detail";
        public static string DaySignIn => "/api/AccountPoints/daySignIn";
        public static string FreezePoints => "/api/AccountPoints/FreezePoints";
        public static string NotifyPointExpire => "/api/Timer/NotifyPointExpire";

        
    }

    public static class Post
    {
        public static string FreezePoints => "/api/AccountPoints/FreezePoints";

        public static string DeFreezePoints => "/api/AccountPoints/DeFreezePoints";

        public static string DeductFreezePoints => "/api/AccountPoints/DeductFreezePoints";

        public static string AddAccountPoints => "/api/AccountPoints/AddAccountPoints";

        public static string AddFreezePoints => "/api/AccountPoints/AddFreezePoints";

        


    }
}

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
using Microsoft.Extensions.Hosting;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Sxb.School.API;
using System.Net.Http;

namespace Sxb.School.FuntionTest
{
    public class SchoolScenariosBase
    {

       protected  WebApplicationFactory<Program> application;
        public SchoolScenariosBase()
        {
            application = new WebApplicationFactory<Program>();
        }


        public HttpClient CreateIdempotentClient()
        {
            var client = application.CreateClient();
            client.DefaultRequestHeaders.Add("x-requestid", Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Add("sxb-innerToken", "sxb-yyds");
            client.DefaultRequestHeaders.Add("Cookie", "iSchoolAuth=CfDJ8A3fTXA7dahOp7dzX7udrT3fCkE2L4cAJeK2BtMT11mJjbVnrzpgdwzFXrFRpFqAp8cS6Iq8fFrNeQnXI3niWjtpimeKXdtqBJIwGco77qp48i-hLBI6Wpun5RCoCjmEw272BomCOkcw08aec83W3t7imT0uaLX4FQgzEcOvtn_NdRULjL63FXAWNairEej2TGFn9MamzOHxPhRQkEAvq0iKfFpOEwrBy0NI0z-rRilbDYAu2lHcM7aGIdpWpGSM8gKOVRKQOrWBRY-RFCCno5NheDpfR5neZxKBMJdt_U32pAhTBDamhF4_q_OT89AGkOso2YwpfGcZ1YLni-TNPcX8900kCkDdmCAmLcWETRmnXCgCBLMO90du2z7S973eKVsx2spMhifnIiJCJ5qhs4_waOqafQxGKQmAX2NXSxmn09nUBm9nDXWV5zir8RwCEJ5Oz_O0jLzpr6po404OxgqIRk_zsdvMrL4erEK-f58byNP5YAay1xQzjLxX7ahhTaJHXapvAO06thVsnB3xJcW-DoBs1wfnuA3BClXnD5Z2Fs6oiFi_jLOfvPv1EENnCLQ0PMpPcTXZrTGYsPGz8ghMQis2rO7UrPhcLUfB0yxOrWfnH7jrYTZ6930FQRtxPNOmUIVmiaRiSUC_eRI87SrnwvhbQ2cG9jqHO9ZFKb9rbUOMSFgpObEUF9OIXLuJEr3DOa48WYYIeWEZvdb8r9o");
            return client;
        }


        public static class Get
        {

            public static string GetQRCode(Guid extId) => $"/api/ViewPay/GetQRCode?extId={extId}";

            public static string GetQRCodeState(Guid orderId) => $"/api/ViewPay/getQRCodeState?orderId={orderId}";
        }

        public static class Post
        {

            public static string RCVWPCB_VS(string checkCode) => $"/api/ViewPay/RCVWPCB_VS?checkCode={checkCode}";


            public static string RCVPAYCB_VS => $"/api/ViewPay/RCVPAYCB_VS";

            public static string RCVWXWORKCB_VS => $"/api/ViewPay/RCVWXWORKCB_VS";

            
        }
        

    }
}

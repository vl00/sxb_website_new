using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.FuntionTest
{
    static class HttpClientExtensions
    {
        public static async Task<T> ReadAs<T>(this HttpContent content) where T:class
        {
            
            var body = await content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(body);

        }
    }
}

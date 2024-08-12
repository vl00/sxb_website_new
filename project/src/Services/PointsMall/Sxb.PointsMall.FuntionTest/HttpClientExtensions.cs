using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.FuntionTest
{
    static class HttpClientExtensions
    {
        public static HttpClient CreateIdempotentClient(this TestServer server)
        {
            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("x-requestid", Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Add("sxb-innerToken", "sxb-yyds");
            client.DefaultRequestHeaders.Add("Cookie", "iSchoolAuth=CfDJ8A3fTXA7dahOp7dzX7udrT1vG8PyLbmgLsK-U8g4x5mas74gcGvQ4l4kP-3Ya6M5K8hxXVa2YRUPSVE8UFKyEz1Q_ZIuEEU4tnRzK90kUe_b7dpZ5zGNHV2dUN7i-5x2-FtPHLNJalbrerH1FReTH75NE0qirnjLYp8nVfvy3vnhjRfAmSUZxvSNz-9S932XwiKmDjei11fDGmEr1KVzDZd29UCjwC5ekAdigswyFPa5lVSLqDvDQySjclJJBtayOuAJAeEqD1v_D9cybMYPPWr_Vlja4tF_vAvrdOCTsg3OuyE7cYC_FB8GVS3VvwAePToM_z4zKmWe2HF2XXtSlOcS2enbdGox77a0SmhGYLopuyAmqi0XbyTiT0BLeoYeKPTdvWZTp2q6FuhR-DFvQJLbZwTZ2F24Wf811ckm3SbA2-CnBOOlnBMBngMRogpEuHyKexgZgTcJPLz-zKCfCPRvY4QSUFU0VTQ4dSaadau-WXlHychFp-XKeYm5T4N1zgV3LnzZhe8d5jgchAom77NQh2v4I3NXXnV6VSR-m-GhiF7_NZkezO6L-tgAIaCy3-wLpwyhxmyQ2iAqaNGElmK-V0a_7RI5ETb3CSlGoH9j0KZfQFbgnHfdzNqVdlXvFJYSVK3B-DXa3dcDVDCYW_qB4nefkw1mG2v3NGB4Oww8kG8liH7WUxOqTIIpajWgKe82Irsf4a6PuQbsWnLgC98");
            return client;
        }

        public static async Task<T> ReadAs<T>(this HttpContent content) where T:class
        {
            var body = await content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(body);

        }
    }
}

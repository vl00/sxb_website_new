using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Framework.Foundation
{
    public class DownLoadFileHelper
    {

        public static async Task<Stream> DownloadStream(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStreamAsync();
            }

        }
    }
}

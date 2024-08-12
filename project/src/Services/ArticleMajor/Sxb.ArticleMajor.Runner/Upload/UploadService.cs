using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Runner
{
    internal class UploadService
    {
        string fileBaseUrl = "https://file_local.sxkid.com";
        IHttpClientFactory _httpClientFactory;
        HttpClient _client;

        public UploadService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _client = _httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri(fileBaseUrl);
        }

        public async Task<UploadImgResponse> UploadImg(string type, string filenName, string filePath, string path = "", string resize = "")
        {
            using var stream = File.OpenRead(filePath);
            //必须await, 不然流会关闭
            return await UploadImg(type, filenName, stream, path, resize);
        }

        public async Task<UploadImgResponse> UploadImg(string type, string filenName, Stream stream, string path = "", string resize = "")
        {
            //避免读错
            stream.Seek(0, SeekOrigin.Begin);

            string url = $"/upload/{type}?filename={filenName}";
            if (!string.IsNullOrWhiteSpace(path))
            {
                url += $"&path={path}";
            }
            if (!string.IsNullOrWhiteSpace(resize))
            {
                url += $"&resize={resize}";
            }
            var content = new StreamContent(stream);
            var response = await _client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var o = JsonConvert.DeserializeObject<UploadImgResponse>(result);


            if (!o.successs)
            {
                Console.WriteLine("UploadImg:error={0}", o.ErrorDescription);
            }
            return o;
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.HttpDelegationHandler
{
    public class LogDelegationHandler : DelegatingHandler
    {
        ILogger<LogDelegationHandler> _logger;
        public LogDelegationHandler(ILogger<LogDelegationHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            _logger.LogInformation("\n 请求路径：{requestUrl},请求方式：{method},请求体:{requestBody}\n 响应状态码:{statuCode},响应体:{responseBody}\n"
                , request.RequestUri
                , request.Method.Method
                , (await request.Content.ReadAsStringAsync())
                , response.StatusCode
                , (await response.Content.ReadAsStringAsync()));
            return response;
        }

    }
}

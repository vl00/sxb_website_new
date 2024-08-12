namespace Sxb.School.API.Infrastructures.Services.Models
{
    public class WeChatGatewayBaseResponse<TData>
    {
        public TData data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
        public string msg { get; set; }
    }
}

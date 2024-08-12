namespace Sxb.School.API.Infrastructures.Services.Models
{
    public class PayGatewayBaseResponse<TData>
    {
        public TData data { get; set; }
        public bool succeed { get; set; }
        public int status { get; set; }
        public string msg { get; set; }
    }
}

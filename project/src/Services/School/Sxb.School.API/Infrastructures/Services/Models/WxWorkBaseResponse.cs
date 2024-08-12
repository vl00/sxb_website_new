namespace Sxb.School.API.Infrastructures.Services.Models
{
    public class WxWorkBaseResponse<TData>
    {
        public TData data { get; set; }
        public bool succeed { get; set; }
        public int status { get; set; }
        public string msg { get; set; }
    }
}

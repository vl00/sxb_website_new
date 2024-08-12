using System;
namespace Sxb.WeWorkFinance.API.Application.Models
{
    public class AccessTokenResultModel
    {
        public bool Success { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public AccessTokenModel Data { get; set; }
    }
    public class AccessTokenModel
    {
        public string AppID { get; set; }
        public string AppName { get; set; }
        public string Token { get; set; }

    }
}

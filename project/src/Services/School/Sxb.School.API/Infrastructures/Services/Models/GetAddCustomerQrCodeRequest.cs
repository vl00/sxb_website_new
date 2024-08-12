namespace Sxb.School.API.Infrastructures.Services.Models
{
    public class GetAddCustomerQrCodeRequest
    {
        public string openId { get; set; }
        public string notifyUrl { get; set; }
        public string attachData { get;private set; }

        /// <summary>
        /// 场景值，用作区分客服类型，空->默认
        ///  DegreeAnalyze
        /// </summary>
        public string scene { get; set; }

        public void SetAttachData(object attachData)
        {
            this.attachData = Newtonsoft.Json.JsonConvert.SerializeObject(attachData);
        }

    }
}

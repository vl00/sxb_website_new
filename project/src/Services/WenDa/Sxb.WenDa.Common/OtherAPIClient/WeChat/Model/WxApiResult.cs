using Sxb.Framework.AspNetCoreHelper.ResponseModel;

namespace Sxb.WenDa.Common.OtherAPIClient.WeChat
{
    public class WxApiResult<T>
    {
        public bool Success { get; set; }
        public int Status { get; set; }
        public string Msg { get; set; }
        public T Data { get; set; }

        public ResponseResult ToResponseResult()
        {
            return new ResponseResult()
            {
                Succeed = Success,
                status = Success ? ResponseCode.Success : ResponseCode.Failed,
                Data = Data,
                Msg = Msg
            };
        }
    }
}
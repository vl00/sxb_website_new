using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.OtherAPIClient.WeChat;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.Repositories;

namespace Sxb.WenDa.API.Application.Query
{
    public interface IUserQuery
    {
        Task BindWxQrCodeCallBackAsync(WPScanCallBackData data);
        Task<Common.Entity.RealUser> GetRealUser(Guid userId, bool noErr = true);

        Task<UserGzWxDto> GetUserGzWxDto(Guid userId);

        /// <summary>
        /// 更新 RealUser.HasJoinWxEnt
        /// </summary>
        Task<UserUpJoinWxEntResDto> UpJoinWxEnt(UserUpJoinWxEntReqDto req);
        Task<WxApiResult<string>> GetWxQrCodeAsync(Guid userId, SubscibeSence sence);
        Task SubscribeWxQrCodeCallBackAsync(WPScanCallBackData data);
    }
}
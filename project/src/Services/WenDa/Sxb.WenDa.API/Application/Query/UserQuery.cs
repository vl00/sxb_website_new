using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.Utils;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Common.OtherAPIClient.User;
using Sxb.WenDa.Common.OtherAPIClient.WxWork;
using Sxb.WenDa.Common.OtherAPIClient.WxWork.Models;
using Sxb.WenDa.Query.SQL.Repositories;
using Sxb.WenDa.Common.OtherAPIClient.WeChat;
using Sxb.WenDa.Common.Data;
using Sxb.Framework.AspNetCoreHelper.CheckException;
using Sxb.Framework.AspNetCoreHelper.Utils;
using Sxb.Framework.AspNetCoreHelper.Extensions;

namespace Sxb.WenDa.API.Application.Query
{
    public class UserQuery : IUserQuery
    {
        private readonly ILogger _log;
        private readonly IConfiguration _config;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IUserApiService _userApiService;
        private readonly IUserRepository _userRepository;
        private readonly IWxWorkApiService _wxWorkApiService;
        private readonly IWxChatApiService _wxChatApiService;

        private readonly AppSettingsData _appSettingsData;

        public UserQuery(IUserApiService userApiService, ILoggerFactory loggerFactory, IUserRepository userRepository,
            IConfiguration config, IWxWorkApiService wxWorkApiService,
            IEasyRedisClient easyRedisClient, IWxChatApiService wxChatApiService, AppSettingsData appSettingsData)
        {
            this._easyRedisClient = easyRedisClient;
            _userApiService = userApiService;
            _log = loggerFactory.CreateLogger(this.GetType());
            _userRepository = userRepository;
            _config = config;
            _wxWorkApiService = wxWorkApiService;
            _wxChatApiService = wxChatApiService;
            _appSettingsData = appSettingsData;
        }

        public async Task<RealUser> GetRealUser(Guid userId, bool noErr = true)
        {
            try
            {
                var rUser = await _userRepository.GetRealUser(userId);
                var isNeedUp = rUser == null;

                if (rUser?.IsRealUser != true)
                {
                    rUser ??= new Common.Entity.RealUser { UserId = userId };
                    try
                    {
                        var b = await _userApiService.IsRealUser(userId);
                        isNeedUp = rUser.IsRealUser != b;
                    }
                    catch { return null; }
                }

                if (isNeedUp)
                {
                    await _userRepository.SaveRealUser(rUser);
                }

                return rUser;
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "getoradd realuser error. userid={userid}, errcode={errcode}", userId, Errcodes.Wenda_RealUserError);
            }
            return noErr ? null : throw new ResponseResultException("系统繁忙", Errcodes.Wenda_RealUserError);
        }

        public async Task<UserGzWxDto> GetUserGzWxDto(Guid userId)
        {
            try
            {
                var result = new UserGzWxDto { UserId = userId };
                var isNeedUp = false;

                var rUser = await GetRealUser(userId, false);
                result.IsRealUser = rUser?.IsRealUser ?? false;

                result.HasGzWxGzh = await _userApiService.HasGzWxGzh(userId);
                if (!result.HasGzWxGzh)
                {                    
                    result.GzWxQrcode = (await GetWxQrCodeAsync(userId, SubscibeSence.Subscribe)).Data;
                }

                // 是否已加企业微信客服
                var hasJoinWxEnt0 = rUser.HasJoinWxEnt;
                if (hasJoinWxEnt0 != true)
                {
                    await CallHasJoinWxEnt(result, rUser);

                    isNeedUp = isNeedUp || rUser.HasJoinWxEnt != hasJoinWxEnt0;
                }
                result.HasJoinWxEnt = rUser.HasJoinWxEnt ?? false;

                if (isNeedUp)
                {
                    await _userRepository.SaveRealUser(rUser);
                }

                return result;
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "获取用户是否关注wx等失败.userid={userid}, errcode={errcode}", userId, Errcodes.Wenda_UserGzError);
                throw new ResponseResultException("系统繁忙", Errcodes.Wenda_UserGzError);
            }
            return null;
        }

        async Task CallHasJoinWxEnt(UserGzWxDto result, RealUser rUser)
        {
            var unionIdDto = await _userApiService.GetUserWxUnionId(rUser.UserId.ToString());
            if (unionIdDto?.UnionId == null)
            {
                _log.LogError("用户={userId}没有unionId.", rUser.UserId);
                return;
            }

            var rr = await _wxWorkApiService.GetAddCustomerQrCode(new GetAddCustomerQrCodeReqDto
            {
                OpenId = unionIdDto.UnionId,
                NotifyUrl = $"{_config["ExternalInterface:Sxb-OpenApi"].TrimEnd('/')}/wenda" + "/api/users/up/joinwxent",
            });
            rUser.HasJoinWxEnt = rr.HasJoinWxEnt;
            result.HasJoinWxEnt = rr.HasJoinWxEnt;
            result.JoinWxEntQrcode = rr.QrcodeUrl;
        }

        public async Task<UserUpJoinWxEntResDto> UpJoinWxEnt(UserUpJoinWxEntReqDto req)
        {
            _log.LogInformation("接收到加入企业微信de回调,参数req=`{req}`", req);

            var result = new UserUpJoinWxEntResDto { };
            try
            {
                var unionIdDto = await _userApiService.GetUserWxUnionId(req.UnionId);
                if (unionIdDto?.UnionId == null)
                {
                    _log.LogError("unionid='{unionid}'没有找到userId.", req.UnionId);
                    return result;
                }
                var userId = unionIdDto.UserId;

                var rUser = await GetRealUser(userId, false);
                rUser.HasJoinWxEnt = req.HasJoinWxEnt;

                await _userRepository.SaveRealUser(rUser);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "更新 RealUser.HasJoinWxEnt失败.req={req}", req);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取微信关注公众号二维码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sence"></param>
        /// <returns></returns>
        public async Task<WxApiResult<string>> GetWxQrCodeAsync(Guid userId, SubscibeSence sence)
        {
            var callBackUrl = HttpContextModel.HttpContext.Request.FullUrlByHeader("wenda", sence.GetDefaultValue<string>());
            _log.LogInformation("回调链接callBackUrl={callBackUrl}", callBackUrl);
            return await _wxChatApiService.GetSenceQRCode(new WPScanRequestData()
            {
                Attach = userId.ToString(),
                CallBackUrl = callBackUrl
            });
        }

        /// <summary>
        /// 关注微信回调 - 绑定微信
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task BindWxQrCodeCallBackAsync(WPScanCallBackData data)
        {
            try
            {
                var template = _appSettingsData.WeChatMsg.BindAccountKFMsg;
                Check.IsNotNull(template, "请先配置绑定微信客服消息模板");

                NewsCustomMsg msg = new()
                {
                    ToUser = data.OpenId,
                    Url = template.RedirectUrl.Replace("{userId}", data.Attach),
                    Description = template.Description,
                    PicUrl = template.ImgUrl,
                    Title = template.Title
                };

                //使用服务号发送消息
                await _wxChatApiService.SendNewsMsg(msg);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "BindWx,data={data}", data.ToJson());
            }
        }

        /// <summary>
        /// 仅关注微信回调
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SubscribeWxQrCodeCallBackAsync(WPScanCallBackData data)
        {
            try
            {
                var template = _appSettingsData.WeChatMsg.WelcomKFMsg;
                Check.IsNotNull(template, "请先配置绑定微信客服消息模板");

                NewsCustomMsg msg = new()
                {
                    ToUser = data.OpenId,
                    Url = template.RedirectUrl.Replace("{userId}", data.Attach),
                    Description = template.Description,
                    PicUrl = template.ImgUrl,
                    Title = template.Title
                };

                //使用服务号发送消息
                await _wxChatApiService.SendNewsMsg(msg);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Subscribe,data={data}", data.ToJson());
            }
        }
    }
}

using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SKIT.FlurlHttpClient.Wechat.Work;
using Sxb.Framework.Cache.Redis;
using Sxb.WeWorkFinance.API.Application.ES;
using Sxb.WeWorkFinance.API.Application.ES.SearchModels;
using Sxb.WeWorkFinance.API.Application.Models;
using Sxb.WeWorkFinance.API.Config;
using Sxb.WeWorkFinance.API.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sxb.Framework.Foundation;
using Sxb.WeWorkFinance.API.Application.Queries.ViewModels;
using Sxb.WeWorkFinance.API.Application.Queries;

namespace Sxb.WeWorkFinance.API.Application.Commands
{
    public class GetChatDataCommandHandle : IRequestHandler<GetChatDataCommand, bool>
    {

        private readonly WorkWeixinConfig _workConfig;
        private readonly InviteActivityConfig _inviteActivityConfig;
        private readonly IMediator _mediator;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IChatDataES _chatDataES;
        private readonly IWeixinQueries _weixinQueries;

        private readonly ILogger<GetChatDataCommandHandle> _logger;

        public GetChatDataCommandHandle(IOptions<WorkWeixinConfig> workConfig, IOptions<InviteActivityConfig> inviteActivityConfig,
            ILogger<GetChatDataCommandHandle> logger, IEasyRedisClient easyRedisClient, IMediator mediator, IChatDataES chatDataES,
            IWeixinQueries weixinQueries)
        {
            _workConfig = workConfig.Value;
            _inviteActivityConfig = inviteActivityConfig.Value;
            _easyRedisClient = easyRedisClient;
            _mediator = mediator;
            _chatDataES = chatDataES;
            _weixinQueries = weixinQueries;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(GetChatDataCommand request, CancellationToken cancellationToken)
        {
            
            var sdk = InitSDK();
            long seq = 0;
            int limit = 1000;//企业微信限制 每次最多请求1000 条
            int timeOut = 500;

            seq = await _weixinQueries.GetChatDataLastSeq();

            long slice = Finance.NewSlice();
            bool next = true;
            do
            {
                Console.WriteLine("seq:"+ seq);
                var ret = Finance.GetChatData(sdk, seq, limit, "", "", timeOut, slice);//获取会话记录数据
                //List<MessageModel> messages = new();
                List<ChatDataViewModel> chatDatas = new();
                if (ret == 0)
                {
                    //获取返回文本
                    var resResultStr = this.GetContentFromSlice(slice);

                    //// 验证
                    JToken jToken = JToken.Parse(resResultStr);

                    if (jToken["errcode"].ToString() == "0")
                    {
                        var resData = jToken["chatdata"].ToString();

                        

                        JArray jArrayData = JArray.Parse(resData);

                        if (jArrayData.Count == 0) 
                        {
                            next = false;
                        }

                        foreach (var item in jArrayData)
                        {

                            seq = Convert.ToInt32(item["seq"]) > seq? Convert.ToInt32(item["seq"]): seq;

                            var chatData = this.DecryptChatData(item["encrypt_random_key"]?.ToString(), item["encrypt_chat_msg"]?.ToString());
                            //Console.WriteLine(chatData);

                            JToken jTokenA = JToken.Parse(chatData);
                            //Console.WriteLine(jTokenA.Last.ToString());

                            string id = jTokenA["msgid"].ToString();

                            //messages.Add(new MessageModel
                            //{
                            //    Id = id,
                            //    MessageJson = chatData
                            //});
                            string action = jTokenA["action"]?.ToString();
                            string roomid =  jTokenA["roomid"]?.ToString();
                            string msgtype = jTokenA["msgtype"]?.ToString();
                            DateTime time = Convert.ToInt64(jTokenA["msgtime"] != null ? jTokenA["msgtime"] : jTokenA["time"]).I2D();
                            string content = null;

                            switch (msgtype)
                            {
                                case "text":
                                    content = jTokenA["text"]["content"].ToString();
                                    break;
                                case "image":
                                    content = (jTokenA["image"]?.ToString()??"").Replace("\r\n   ", "").Replace("\r\n", "");
                                    break;
                                case "revoke":
                                    content = (jTokenA["revoke"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "disagree":
                                    content = (jTokenA["disagree"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "agree":
                                    content = (jTokenA["agree"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "video":
                                    content = (jTokenA["video"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "voice":
                                    content = (jTokenA["voice"]?.ToString() ?? "").Replace("\r\n  ", "").Replace("\r\n", "");
                                    break;
                                case "card":
                                    content = (jTokenA["card"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "location":
                                    content = (jTokenA["location"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "emotion":
                                    content = (jTokenA["emotion"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "file":
                                    content = (jTokenA["file"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "link":
                                    content = (jTokenA["link"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "weapp":
                                    content = (jTokenA["weapp"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "chatrecord":
                                    content = (jTokenA["chatrecord"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "collect":
                                    content = (jTokenA["collect"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "redpacket":
                                    content = (jTokenA["redpacket"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "meeting":
                                    content = (jTokenA["meeting"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "docmsg":
                                    content = (jTokenA["doc"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "markdown":
                                    content = (jTokenA["info"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "news":
                                    content = (jTokenA["info"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "calendar":
                                    content = (jTokenA["calendar"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "mixed":
                                    content = (jTokenA["mixed"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                case "external_redpacket":
                                    content = (jTokenA["redpacket"]?.ToString() ?? "").Replace("\r\n  ","").Replace("\r\n","");
                                    break;
                                default:
                                    content = (chatData ?? "").Replace("\r\n  ", "").Replace("\r\n", "");
                                    break;
                            }

                            chatDatas.Add(new ChatDataViewModel
                            {
                                Msgid = id,
                                Action = action,
                                From = jTokenA["from"]?.ToString(),
                                Tolist = string.IsNullOrWhiteSpace(roomid) ? jTokenA["tolist"]?.ToString().Replace("\r\n  ","").Replace("\r\n","") : "",
                                Roomid = roomid,
                                Msgtime = time,
                                Msgtype = jTokenA["msgtype"]?.ToString(),
                                Contents = content,
                                Seq = seq
                            });


                        }
                        //_chatDataES.ImportChatData(messages);
                        await _weixinQueries.InsertChatData(chatDatas);

                    }
                    else
                    {
                        next = false;
                    }
                }
            } while (next);



            return true;
        }


        private long InitSDK()
        {
            long sdk = Finance.NewSdk();
            string sCorpID = _workConfig.CorpId;
            string sCorpSecret = _workConfig.CorpSecret;
            // 这里填写 企业微信 corpid，secret
            Finance.Init(sdk, sCorpID, sCorpSecret);

            return sdk;
        }

        /// <summary>
        ///  解密 chatdata
        /// </summary>
        /// <param name="encrypt_random_key"></param>
        /// <param name="encrypt_chat_msg"></param>
        /// <returns></returns>
        public string DecryptChatData(string encrypt_random_key, string encrypt_chat_msg)
        {
            var privatekey = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wechat_privateKey.xml"));

            if (string.IsNullOrWhiteSpace(privatekey))
                throw new Exception("privatekey 私钥为空！");

            var random_key = RSAHelper.Decrypt(privatekey, encrypt_random_key);

            if (string.IsNullOrWhiteSpace(random_key))
                throw new Exception("random_key 解密失败！");

            var sliceMsg = Finance.NewSlice();

            var ret = Finance.DecryptData(random_key, encrypt_chat_msg, sliceMsg);

            //CheckResultInt(ret, nameof(DecryptChatData));

            //获取返回文本
            var resResultStr = this.GetContentFromSlice(sliceMsg);

            return resResultStr;
        }
        /// <summary>
        /// 获取文本
        /// </summary>
        /// <param name="slice"></param>
        /// <returns></returns>
        private string GetContentFromSlice(long slice)
        {
            int len = Finance.GetSliceLen(slice);

            byte[] vbyte = new byte[len];

            var intPtr = Finance.GetContentFromSlice(slice);

            System.Runtime.InteropServices.Marshal.Copy(intPtr, vbyte, 0, vbyte.Length);

            return Encoding.UTF8.GetString(vbyte);
        }
        /// <summary>
        ///  验证 sdk 返回的数据信息
        /// </summary>
        /// <param name="result">SDK返回的结果集</param>
        /// <param name="dataColumn">data 列名</param>
        /// <param name="methodName">请求的方法名</param>
        /// <returns></returns>
        private string CheckAndGetResultText(string result, string dataColumn, string methodName = "")
        {
            if (string.IsNullOrWhiteSpace(result))
                throw new Exception($"CheckResultText 【{methodName}】 验证失败，返回结果为空");

            try
            {
                JToken jToken = JToken.Parse(result);

                if (jToken["errcode"].ToString() == "0")
                {
                    return jToken[dataColumn].ToString();
                }

                throw new Exception($"【{methodName}】数据返回失败，errmsg：{jToken["errmsg"]}");

            }
            catch (Exception ex)
            {
                throw new Exception($"【{methodName}】解析失败，错误：{ex.Message}");
            }
        }
    }
}

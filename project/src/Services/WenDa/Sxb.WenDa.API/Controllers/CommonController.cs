using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.API.Config;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sts.V20180813;
using TencentCloud.Sts.V20180813.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Sxb.WenDa.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly TencentCloudOptions _tencentCloudOptions;
        public CommonController(IOptions<TencentCloudOptions> tencentCloudOptions)
        {
            _tencentCloudOptions = tencentCloudOptions.Value;
        }

        [HttpGet]
        [Authorize]
        public ResponseResult GetUploadCosKey(string type)
        {
            try
            {
                Dictionary<string, string> pathDic = new Dictionary<string, string>
                {
                    { "wenda", "wd" },//问答
                    { "topic", "topic" }//头像
                };

                if (type == null || !pathDic.ContainsKey(type)) return ResponseResult.Failed("业务类型的路径不存在");

                var userInfo = HttpContext.GetUserInfo();
                var userId = userInfo.UserId;

                //用户上传路径： /u/{用户id}/{业务type}/文件名.xxx
                string updatePath = "/u/" + userId + "/" + pathDic[type] + "/";

                Credential cred = new Credential
                {
                    SecretId = _tencentCloudOptions.SecretId,
                    SecretKey = _tencentCloudOptions.SecretKey
                };
                var bucket = _tencentCloudOptions.Bucket;
                var region = _tencentCloudOptions.Region;

                BucketPolicy bucketPolicy = new BucketPolicy {
                    Version = "2.0",
                    Statement = new List<BucketPolicy.StatementModel> {
                        new BucketPolicy.StatementModel
                        {
                            Effect = "allow",
                            Action = new List<string> { "name/cos:PutObject" },
                            Resource = new List<string> { "qcs::cos:" + region + ":uid/1252413095:" + bucket + updatePath + "*" }
                        }
                    }
                };
                var setting = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                };
                string policy = Newtonsoft.Json.JsonConvert.SerializeObject(bucketPolicy, setting);

                //string policy = "{\"version\":\"2.0\",\"statement\":[{\"effect\":\"allow\",\"action\":[\"name/cos:PutObject\"],\"resource\":[\"qcs::cos:" + region + ":uid/1252413095:" + bucket + "/resume/*\"]}]}";


                HttpProfile httpProfile = new HttpProfile
                {
                    Endpoint = ("sts.tencentcloudapi.com")
                };
                ClientProfile clientProfile = new ClientProfile
                {
                    HttpProfile = httpProfile
                };

                StsClient client = new StsClient(cred, region, clientProfile);
                GetFederationTokenRequest req = new GetFederationTokenRequest
                {
                    Name = "webUpload",
                    Policy = policy
                };
                GetFederationTokenResponse resp = client.GetFederationTokenSync(req);
                //Console.WriteLine(AbstractModel.ToJsonString(resp));

                return ResponseResult.Success(new BucketKeyView { UpdatePath = updatePath, Key = resp });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return ResponseResult.Failed();
            }
        }

        public class BucketKeyView
        {
            public string UpdatePath { get; set; }
            public GetFederationTokenResponse Key { get; set; }
        }

        public class BucketPolicy
        {
            public string Version { get; set; } = "2.0";
            public List<StatementModel> Statement { get; set; }
            public class StatementModel
            {
                public List<string> Action { get; set; }
                public string Effect { get; set; }
                public List<string> Resource { get; set; }
            }
        }
    }
}

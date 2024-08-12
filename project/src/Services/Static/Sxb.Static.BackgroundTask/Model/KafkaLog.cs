using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Model
{
    //{ "userid":"def125db-f091-4e6d-adf0-7bb5330d703d","deviceid":"2595fdb6-acda-4a2e-b8b0-551a9aa19124","url":"/pagesmine/pages/pull-new/pull-new","adcode":0,"fw":"","fx":null,"appid":"wx0da8ff0241f39b11","version":null,"module":null,"time":"2021-09-17 22:08:12","ip":150995210,"platform":2,"system":1,"client":2,"sessionid":"app_org_1631887292210_1089_1089","ua":"wx_miniprogram-8.0.11-systemInfo.model-360-725","method":null,"params":{ "url":"/pagesmine/pages/pull-new/pull-new","fw":"","referer":"/pagesmine/pages/mine-consultant/mine-consultant","event":"onload","sessionid":"app_org_1631887292210_1089_1089"},"event":"onload","referer":"/pagesMine/pages/mine-consultant/mine-consultant"}
    public class KafkaLog
    {
        public string UserId { get; set; }

        public string DeviceId { get; set; }

        public string Url { get; set; }

        public string FW { get; set; }

        public  DateTime? Time { get; set; }
    }
}

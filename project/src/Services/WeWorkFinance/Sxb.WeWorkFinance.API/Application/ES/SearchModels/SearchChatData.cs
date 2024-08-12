using System;
using System.Collections.Generic;

namespace Sxb.WeWorkFinance.API.Application.ES.SearchModels
{
    public class SearchChatData
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Msgid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Tolist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Roomid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Msgtime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Msgtype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TextModel Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RevokeModel Revoke { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AgreeModel Agree { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public VoiceModel Voice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public VideoModel Video { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CardModel Card { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public LocationModel Location { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EmotionModel Emotion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FileModel File { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public LinkModel Link { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public WeappModel Weapp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ChatrecordModel Chatrecord { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CollectModel Collect { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RedpacketModel Redpacket { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MeetingModel Meeting { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DocModel Doc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public InfoModel Info { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CalendarModel Calendar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MixedModel Mixed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Voiceid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MeetingVoiceCallModel Meeting_voice_call { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Voipid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public VoipDocShareModel Voip_doc_share { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SphfeedModel Sphfeed { get; set; }


        public class TextModel
        {
            /// <summary>
            /// 
            /// </summary>
            public string Content { get; set; }
        }

        public class RevokeModel
        {
            /// <summary>
            /// 
            /// </summary>
            public string Pre_msgid { get; set; }
        }

        public class AgreeModel
        {
            /// <summary>
            /// 
            /// </summary>
            public string Userid { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Agree_time { get; set; }
        }

        public class VoiceModel
        {
            /// <summary>
            /// 
            /// </summary>
            public string Md5sum { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Voice_size { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Play_length { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Sdkfileid { get; set; }
        }

        public class VideoModel
        {
            /// <summary>
            /// 
            /// </summary>
            public string Md5sum { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Filesize { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Play_length { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Sdkfileid { get; set; }
        }

        public class CardModel
        {
            /// <summary>
            /// 微信联系人
            /// </summary>
            public string Corpname { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Userid { get; set; }
        }

        public class LocationModel
        {
            /// <summary>
            /// 
            /// </summary>
            public double Longitude { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double Latitude { get; set; }
            /// <summary>
            /// 北京市xxx区xxx路xxx大厦x座
            /// </summary>
            public string Address { get; set; }
            /// <summary>
            /// xxx管理中心
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Zoom { get; set; }
        }

        public class EmotionModel
        {
            /// <summary>
            /// 
            /// </summary>
            public int Type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Width { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Height { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Imagesize { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Md5sum { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Sdkfileid { get; set; }
        }

        public class FileModel
        {
            /// <summary>
            /// 
            /// </summary>
            public string Md5sum { get; set; }
            /// <summary>
            /// 资料.docx
            /// </summary>
            public string Filename { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Fileext { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Filesize { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Sdkfileid { get; set; }
        }

        public class LinkModel
        {
            /// <summary>
            /// 邀请你加入群聊
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 技术支持群，进入可查看详情
            /// </summary>
            public string description { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string link_url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string image_url { get; set; }
        }

        public class WeappModel
        {
            /// <summary>
            /// 开始聊天前请仔细阅读服务须知事项
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 客户需同意存档聊天记录
            /// </summary>
            public string description { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string username { get; set; }
            /// <summary>
            /// 服务须知
            /// </summary>
            public string displayname { get; set; }
        }

        public class ItemItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int msgtime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string content { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string from_chatroom { get; set; }

        }

        public class InfoItem
        {

            /// <summary>
            /// 
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string description { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string picurl { get; set; }
        }

        public class ChatrecordModel
        {
            /// <summary>
            /// 群聊
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<ItemItem> item { get; set; }
        }

        public class DetailsItem
        {
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 表项1，文本
            /// </summary>
            public string ques { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string type { get; set; }
        }

        public class CollectModel
        {
            /// <summary>
            /// 这是一个群
            /// </summary>
            public string room_name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string creator { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string create_time { get; set; }
            /// <summary>
            /// 这是填表title
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<DetailsItem> details { get; set; }
        }

        public class RedpacketModel
        {
            /// <summary>
            /// 
            /// </summary>
            public int type { get; set; }
            /// <summary>
            /// 恭喜发财，大吉大利
            /// </summary>
            public string wish { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int totalcnt { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int totalamount { get; set; }
        }

        public class MeetingModel
        {
            /// <summary>
            /// 夕会
            /// </summary>
            public string topic { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int starttime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int endtime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string address { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string remarks { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int meetingtype { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int meetingid { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int status { get; set; }
        }

        public class DocModel
        {
            /// <summary>
            /// 测试&演示客户
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string doc_creator { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string link_url { get; set; }
        }



        public class InfoModel
        {
            /// <summary>
            /// 请前往系统查看，谢谢。
            /// </summary>
            public string content { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<InfoItem> item { get; set; }
        }

        public class CalendarModel
        {
            /// <summary>
            /// xxx业绩复盘会
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string creatorname { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> attendeename { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int starttime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int endtime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string place { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string remarks { get; set; }
        }

        public class MixedModel
        {
            /// <summary>
            /// 
            /// </summary>
            public List<ItemItem> item { get; set; }
        }

        public class DemofiledataItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string filename { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string demooperator { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int starttime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int endtime { get; set; }
        }

        public class SharescreendataItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string share { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int starttime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int endtime { get; set; }
        }

        public class MeetingVoiceCallModel
        {
            /// <summary>
            /// 
            /// </summary>
            public int endtime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string sdkfileid { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<DemofiledataItem> demofiledata { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<SharescreendataItem> sharescreendata { get; set; }
        }

        public class VoipDocShareModel
        {
            /// <summary>
            /// 欢迎使用微盘.pdf.pdf
            /// </summary>
            public string filename { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string md5sum { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int filesize { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string sdkfileid { get; set; }
        }

        public class SphfeedModel
        {
            /// <summary>
            /// 
            /// </summary>
            public int feed_type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string sph_name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string feed_desc { get; set; }
        }
    }
}

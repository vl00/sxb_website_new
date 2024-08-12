using Sxb.Framework.Foundation;

namespace Sxb.WenDa.Query.SQL.QueryDto
{
    public class NotifyUserQueryDto
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// 问题短链
        /// </summary>
        public long No { get; set; }

        public string QuestionNo => UrlShortIdUtil.Long2Base32(No);

        /// <summary>
        /// 回答短链
        /// </summary>
        public long Ano { get; set; }
        public string AnswerNo => UrlShortIdUtil.Long2Base32(Ano);
    }
}

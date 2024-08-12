using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class GetCommentsPageListQryResDto : Page<CommentItemDto>
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        [JsonConverter(typeof(DateTimeToTimestampJsonConverter))]
        public DateTime Time { get; set; }
    }
}

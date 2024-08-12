using Sxb.Static.BackgroundTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask
{
   public static  class KafkaContextExtension
    {
        public static KafkaLog AsKafkaLog(this KafkaContext context)
        {
            var kafkalog = Newtonsoft.Json.JsonConvert.DeserializeObject<KafkaLog>(context.ConsumeResult.Message.Value);
            return kafkalog;
        }
    }
}

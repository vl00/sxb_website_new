using Confluent.Kafka;
using Sxb.Static.BackgroundTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask
{
    public interface IMiddleware
    {

        Task InvokeAsync(KafkaContext context, Func<KafkaContext, Task> next);
    }
}

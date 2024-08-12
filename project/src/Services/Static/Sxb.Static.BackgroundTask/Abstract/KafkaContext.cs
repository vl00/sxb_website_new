using Confluent.Kafka;
using Sxb.Static.BackgroundTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask
{
    public class KafkaContext
    {
        public ConsumeResult<Ignore, string> ConsumeResult   { get; init; }

        public IServiceProvider Services { get; init; }


        public CancellationToken CancellationToken { get; init; }


    }
}

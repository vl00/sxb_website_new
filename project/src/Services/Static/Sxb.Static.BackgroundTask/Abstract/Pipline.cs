using Confluent.Kafka;
using Sxb.Static.BackgroundTask.Middleware;
using Sxb.Static.BackgroundTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask
{
    public class Pipline
    {
        int middlewareOffset = 0;

        private List<IMiddleware> _middlewares;
        public IReadOnlyList<IMiddleware> Middlewares { get { return _middlewares.AsReadOnly(); } }

        public Pipline AddMiddleware(IMiddleware middleware)
        {
            if (_middlewares == null)
                _middlewares = new List<IMiddleware>();
            _middlewares.Add(middleware);
            return this;
        }


        public async Task Start(KafkaContext context)
        {

            await Next(context);

        }

        public async Task Next(KafkaContext context)
        {
            if (middlewareOffset == _middlewares.Count)
            {
                return;
            }
            await _middlewares[middlewareOffset++].InvokeAsync(context, Next);
        }
    }
}

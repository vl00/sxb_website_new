﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sxb.Framework.Cache.Redis.Configuration;
using StackExchange.Redis;

namespace Sxb.Framework.Cache.Redis.ServerIteration
{
    public class ServerIteratorFactory
    {
        public static IEnumerable<IServer> GetServers(
            ConnectionMultiplexer multiplexer,
            ServerEnumerationStrategy serverEnumerationStrategy)
        {
            switch (serverEnumerationStrategy.Mode)
            {
                case ServerEnumerationStrategy.ModeOptions.All:
                    var serversAll = new ServerEnumerable(multiplexer,
                        serverEnumerationStrategy.TargetRole,
                        serverEnumerationStrategy.UnreachableServerAction);
                    return serversAll;

                case ServerEnumerationStrategy.ModeOptions.Single:
                    var serversSingle = new ServerEnumerable(multiplexer,
                        serverEnumerationStrategy.TargetRole,
                        serverEnumerationStrategy.UnreachableServerAction);
                    return serversSingle.Take(1);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}

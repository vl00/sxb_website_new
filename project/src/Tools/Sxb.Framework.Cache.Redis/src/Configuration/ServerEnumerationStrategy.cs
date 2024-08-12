﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Framework.Cache.Redis.Configuration
{
    public class ServerEnumerationStrategy
    {
        public enum ModeOptions
        {
            All = 0,
            Single
        }

        public enum TargetRoleOptions
        {
            Any = 0,
            PreferSlave
        }

        public enum UnreachableServerActionOptions
        {
            Throw = 0,
            IgnoreIfOtherAvailable
        }

        public ModeOptions Mode { get; set; }

        public TargetRoleOptions TargetRole { get; set; }

        public UnreachableServerActionOptions UnreachableServerAction { get; set; }
    }
}

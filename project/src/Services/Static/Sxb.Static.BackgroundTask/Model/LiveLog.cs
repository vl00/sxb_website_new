﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Model
{
    public record LiveLog : DataLog
    {
        public int City { get; set; }
    }
}

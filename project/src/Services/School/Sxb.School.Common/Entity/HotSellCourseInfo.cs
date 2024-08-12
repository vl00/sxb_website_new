﻿using System;
using System.Collections.Generic;

namespace Sxb.School.Common.Entity
{
    public class HotSellCourseInfo
    {
        public class HttpWrapper
        {
            public IEnumerable<HotSellCourseInfo> Courses { get; set; }
        }
        public Guid ID { get; set; }
        public string ID_S { get; set; }
        public string OrgName { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int Subject { get; set; }
        public string Banner { get; set; }
        public string Price { get; set; }
        public string OrigPrice { get; set; }
        public bool Authentication { get; set; }
    }
}

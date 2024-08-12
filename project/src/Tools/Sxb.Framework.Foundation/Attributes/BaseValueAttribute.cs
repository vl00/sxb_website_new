﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Framework.Foundation
{
    public abstract class BaseValueAttribute : Attribute
    {
        public string Value { get; set; }

        public BaseValueAttribute(string value)
        {
            Value = value;
        }
    }
}

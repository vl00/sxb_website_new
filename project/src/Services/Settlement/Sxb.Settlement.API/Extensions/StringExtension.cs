using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Extensions
{
    public static class StringExtension
    {

        public static bool IsSxbUrl(this string str)
        {
            Regex sxbUrlValidate = new Regex(@"http(s)?://[a-zA-Z0-9-]+\.sxkid\.com");
            return sxbUrlValidate.IsMatch(str);
        }
    }
}

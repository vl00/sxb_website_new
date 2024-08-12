using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Framework.Foundation
{
    public class UriHelper
    {

        /// <summary>
        /// 组合uri parts
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static string Combine(params string[] parts)
        {
            if (parts == null)
                return string.Empty;
            
            if (parts.Length == 1)
                return parts[0];


            var sb = new StringBuilder();
            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;

                if (part.StartsWith("/"))
                    sb.Append(part.Remove(0, 1));
                else
                    sb.Append(part);

                if (!part.EndsWith("/"))
                    sb.Append('/');
            }

            return sb.ToString();
        }
    }
}

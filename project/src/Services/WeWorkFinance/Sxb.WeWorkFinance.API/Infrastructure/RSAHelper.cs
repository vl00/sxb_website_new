using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API
{
    public class RSAHelper
    {
        /// <summary>
        /// RAS 私钥解密
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Decrypt(string privateKey, string text)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            byte[] byteKeys = Convert.FromBase64String(text);

            rsa.FromXmlString(privateKey);

            var cByte = rsa.Decrypt(byteKeys, false);

            return System.Text.Encoding.UTF8.GetString(cByte);
        }
    }
}

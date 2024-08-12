using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Sxb.Framework.Foundation
{
    public class MD5Helper
    {
        private static string MD5Tail;
        public MD5Helper(string _MD5Tail)
        {
            MD5Tail = _MD5Tail;
        }
        public static string GetMD5(string str)
        {
            return GetSimpleMD5(str + MD5Tail);
        }
        public static string GetMD5(string str, string _MD5Tail)
        {
            return GetSimpleMD5(str + _MD5Tail);
        }
        public static string GetSimpleMD5(string str)
        {
            byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(str);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] OutBytes = md5.ComputeHash(data);

            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
            return OutString.ToUpper();
        }

        ///
        /// MD5 16位加密 加密后密码为大写
        ///
        public static string GetMd5Str(string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");

            t2 = t2.ToUpper();

            return t2;
        }
        public static string GetSHA256(string str)
        {

            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] hash = SHA256.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }

        public static string GetHMACSHA256(string key, string message)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] msgBytes = Encoding.UTF8.GetBytes(message);
            using (HMACSHA256 sha256 = new HMACSHA256(keyBytes))
            {
                byte[] hash = sha256.ComputeHash(msgBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

    }
}

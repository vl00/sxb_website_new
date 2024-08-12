using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Settlement.Test
{
    [TestClass]
    public class AESHelperTest
    {

        [TestMethod]
        public void EncrytionTest()
        {
            string secret = "0688f7a191da4fbab177fd1c8ef19901";
            string aesIV = "ff465fdecc764337";
            string plaintext = "{\"settlement_code\":[\"JS19BUB14F5D8D4C\"],\"random_code\":[\"19BUB14F5D8D4C\",\" 19BUAD0E89D780\"]}";

            var ciypertext = AESHelper.EncryptionToBase64(plaintext, secret, aesIV);
            string expecttext = @"236agZcupcSsMZghtlmzhb7lEWzGZc3FO5GWQyrSB5kP/y1ESvd+CuBgQiWU/fwAICY/s0mideku/rXSKEb8IsPHYHlydSb3h/JFABX4xHKBrLyTgsah8gNjocKqzYRn";
            Assert.IsTrue(ciypertext.Equals(expecttext));
        }

        [TestMethod]
        public void DecrytionTest()
        {
            string secret = "0688f7a191da4fbab177fd1c8ef19901";
            string aesIV = "ff465fdecc764337";
            string expecttext = "{\"settlement_code\":[\"JS19BUB14F5D8D4C\"],\"random_code\":[\"19BUB14F5D8D4C\",\" 19BUAD0E89D780\"]}";
            string ciypertext = @"236agZcupcSsMZghtlmzhb7lEWzGZc3FO5GWQyrSB5kP/y1ESvd+CuBgQiWU/fwAICY/s0mideku/rXSKEb8IsPHYHlydSb3h/JFABX4xHKBrLyTgsah8gNjocKqzYRn";
            string plaintext = AESHelper.DecryptionFromBase64(ciypertext, secret, aesIV);
            Assert.IsTrue(plaintext.Equals(expecttext));
        }
    }
}

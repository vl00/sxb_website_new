using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Config
{
    public class Configs
    {
        public static string GetAddAccountPointsRedisKey(Guid userId)
        {
            return $"AddAccountPoints:{userId}";
        }

        public static string GetTaskRSATail()
        {
            return $"!@#zsq213";
        }
        public static string GetTaskRSAPublicKey()
        {
            return $"<RSAKeyValue><Modulus>x9Volhyv+CrS5W4mAEk3MlT0WYmetJuuuuLXicQAWLUF81QHwZYYdXxxDxkU8UpPJqpNYG5ZPc1x38MRiHbEorbT1OyWYCGGnI1dFLHIiMJKQAV1pwZufRJRvLmLTzCCyGaWhqenRE3yjemGq1eXmroIxnJRQNl0H8htzrm1kL9P1Zq+AfHNYi5mjDb9RuYtOFXsQK/OSALx66zB6/bHLFaasoEGQJ7SOJnASJ7jKDTEI9OuCWN0WSWZoLzLodPMSfSZ6EQBHAA3XnQGiHoA7qmJQxcltKt7SqPcqAkOyKzjDI2BRaZozHRzEQDKpHG4Ck7F57jatHEm3m1rJVSNeQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        }


        public static string GetTaskRSAPrivateKey()
        {
            return $"<RSAKeyValue><Modulus>x9Volhyv+CrS5W4mAEk3MlT0WYmetJuuuuLXicQAWLUF81QHwZYYdXxxDxkU8UpPJqpNYG5ZPc1x38MRiHbEorbT1OyWYCGGnI1dFLHIiMJKQAV1pwZufRJRvLmLTzCCyGaWhqenRE3yjemGq1eXmroIxnJRQNl0H8htzrm1kL9P1Zq+AfHNYi5mjDb9RuYtOFXsQK/OSALx66zB6/bHLFaasoEGQJ7SOJnASJ7jKDTEI9OuCWN0WSWZoLzLodPMSfSZ6EQBHAA3XnQGiHoA7qmJQxcltKt7SqPcqAkOyKzjDI2BRaZozHRzEQDKpHG4Ck7F57jatHEm3m1rJVSNeQ==</Modulus><Exponent>AQAB</Exponent><P>6f7mcyznfCEkODEijnKo26iwxuMxDCwpUuN0k9BJjoQAMnGyHZdQsl3MseC+hfcGBAq+Z0DIIbRbUyc2OWt0eZQPoZEaBdzoa3VSa1e2iFo1rAOjQTUfD2uvI0u2lDvRrIC54bKtmTC4lYTTpayEzireFDX6ppnsV722I3+j3Pc=</P><Q>2qAbYpechAy10ImjYKCr3fQVNc21Ek3jJkeeVqZm0NHM+bjYmKBtotCJgvICke4d4Wdury8WWzIHZxsFt3bqFcEkKwECCMrWiXSLlyGGijlyF5FOy0+7hyCwJI0xoFME5aHNmajXvYsl3jQNUB4Y6C9ygU3trSVPdws/Vo9afQ8=</Q><DP>VciG/45kxtL3jkolGwfZ871iIWfE4WuHMsYERPRFIyQtVlDsNYD1qo5MrqFv93jaEb1GlF/hfsm5+UAU1vQX3UJ0gIzLd37HDi4Sqxua5V388zTaMtugqWyW5l+pbaqIca83dhClJ9X+tYHefYxDm7mHdO1dGJqxVHFORrFSh30=</DP><DQ>IeBLPgS2GPQLPCwHp9Jdrz/CBZtBYnu1JdpVG6IfLl5D/7H3xfad5muf0y2C3m/iK2omiXinYywmX4Cdayc+8G4EF7HJSel8QsJPWvSz7zklh6dZaUGKkk6rXI6QUQJsMNTYkeXKwDEOhhbUtURkoZ2whX64xGqSnKSehOAjqls=</DQ><InverseQ>NiC/6qRRHfG6ClfnRzRTcBRYA1bboKzgifeuBz4cI1FBv5lbQE0ekrYIfQ1f45ZYB1622YPIsLtt0i6OpqMUoVObGSj/PpCktvSIsJ0CiJK59Zdj21Dl35UJFWtYoXKmDfCHgdyLdPERVURh+3SfAOADCTCqi+FaAHt6AH9V9sM=</InverseQ><D>HypkVk0BELY1D/0exhqlFh9zwGcbLlW9aX4pWV4NI8iPtgRb9VAgXmpbA5zIzFznfUNAMBLlriLVaK12lY7YWfwji2iehz2TTeUqKCkV+mY6yENRtwvkvXLW9pUFagNJ0lFVloidKKTzeDh8fUp65XGwHqsDixae6roKjhweegVlqc0tIy91unyR6TE1ostDeXZ4STeOSJPw/zC/DpY9aZMjfubmUowYVDp2jJpbsgj2gkRWDmZTZT2JMqbr9sIlEJLlM1Oll4X6BE7FMU8yaVUcbG1ywSdWFWqCvs8D2dNX6TepQnuH4rhSC39jZdmPrc/54gdKlQGNzENZgT9OZQ==</D></RSAKeyValue>";
        }
    }
}

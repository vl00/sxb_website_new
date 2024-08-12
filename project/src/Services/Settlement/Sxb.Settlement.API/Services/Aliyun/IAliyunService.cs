using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Services.Aliyun
{
    public interface IAliyunService
    {


      /// <summary>
      /// 获取银行卡四要素认证结果
      /// </summary>
      /// <returns></returns>
      Task<AliyunResponse<BankCertificationResult>>  BankCertificationAsync(BankCertificationRequest request);


    }
}

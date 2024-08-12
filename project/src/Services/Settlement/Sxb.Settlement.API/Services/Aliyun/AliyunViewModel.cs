using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Services.Aliyun
{



    public class AliyunResponse<TResult> where TResult : class
    {
        /// <summary>
        /// "00000"成功
        /// </summary>
        public string errcode { get; set; }
        public TResult result { get; set; }
        public string jobid { get; set; }
        public string responsetime { get; set; }
        public string errmsg { get; set; }


    }

    /// <summary>
    /// 银行卡认证结果
    /// </summary>
    public class BankCertificationResult
    {
        public string provincename { get; set; }
        public string areaname { get; set; }
        public string bankname { get; set; }
        public string bankalias { get; set; }
        public string bankcardtype { get; set; }
        public string weburl { get; set; }
        public string logo { get; set; }
        public string isluhn { get; set; }
        public string tel { get; set; }
        public string cardname { get; set; }
        public string carddigits { get; set; }
        public string bindigits { get; set; }
        public string cardbin { get; set; }
        public int consumemoney { get; set; }
        public string consumestate { get; set; }
    }

    /// <summary>
    /// 名称	类型	是否必须	描述
    /// bankcard STRING  可选 银行卡号
    /// customername STRING  可选 商户名称
    /// idcard STRING  可选 证件号
    /// idcardtype STRING  可选 证件类型（选填） 01：身份证（默认） 02：军官证 03：护照 04：回乡证 05：台胞证 06：警官证 07：士兵证 08：驾驶证 09：学⽣证 10：港澳证 99：其它证件
    /// mobile  STRING 可选  银行卡预留手机号
    /// realname    STRING 可选  姓名
    /// scenecode   STRING 可选  商户业务应用场景（01：直销银行；02：消费金融；03：银行二三类账户开户；04：征信；05：保险；06：基金；07：证券；08 租赁；09：海关申报；99：其他）
    /// </summary>
    public class BankCertificationRequest
    {

        public string? bankcard { get; set; }

        public string? customername { get; set; }

        public string? idcard { get; set; }

        /// <summary>
        /// 可选 证件类型（选填） 01：身份证（默认） 02：军官证 03：护照 04：回乡证 05：台胞证 06：警官证 07：士兵证 08：驾驶证 09：学⽣证 10：港澳证 99：其它证件
        /// </summary>
        public string? idcardtype { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string? mobile { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string? realname { get; set; }

        /// <summary>
        /// 商户业务应用场景（01：直销银行；02：消费金融；03：银行二三类账户开户；04：征信；05：保险；06：基金；07：证券；08 租赁；09：海关申报；99：其他）
        /// </summary>
        public string? scenecode { get; set; }
    }


}

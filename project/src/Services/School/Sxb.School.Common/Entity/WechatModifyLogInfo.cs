using Kogel.Dapper.Extension.Attributes;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 微信院校库导入修改记录
    /// </summary>
    public class WechatModifyLogInfo
    {
        [Identity(false)]
        public Guid ID { get; set; }
        /// <summary>
        /// 学部ID
        /// </summary>
        public Guid EID { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int? Yaer { get; set; }
        /// <summary>
        /// 修改的字段
        /// <para>JSON</para>
        /// </summary>
        public string Attrs { get; set; }
        [Display(IsField = false)]
        public IEnumerable<string> Attrs_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Attrs))
                {
                    var result = Attrs.FromJsonSafe<IEnumerable<string>>();
                    if (result?.Any(p => !string.IsNullOrWhiteSpace(p)) == true) return result;
                }
                return default;
            }
        }
        /// <summary>
        /// 是否修改(否则新增)
        /// </summary>
        public bool IsModify { get; set; }
        /// <summary>
        /// 修改日期
        /// <para>yyyy-MM-dd</para>
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}

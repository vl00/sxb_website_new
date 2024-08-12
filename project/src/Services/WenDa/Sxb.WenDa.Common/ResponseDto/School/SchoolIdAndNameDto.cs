
namespace Sxb.WenDa.Common.ResponseDto.School
{
    public class SchoolIdAndNameDto
    {
        /// <summary>学校eid</summary>
        public Guid Eid { get; set; }
        /// <summary>学校短id</summary>
        public string SchoolNo { get; set; }
        /// <summary>学校名</summary>
        public string Schname { get; set; }
        /// <summary>学部名</summary>
        public string Extname { get; set; }
        /// <summary>true=正常; false=已删除</summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 学校全称
        /// </summary>
        public string SchoolName => $"{Schname}-{Extname}";
    }
}

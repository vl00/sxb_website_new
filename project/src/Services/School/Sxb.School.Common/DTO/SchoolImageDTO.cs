using Sxb.School.Common.Enum;

namespace Sxb.School.Common.DTO
{
    public class SchoolImageDTO
    {
        public string Url { get; set; }
        public string SUrl { get; set; }
        public string ImageDesc { get; set; }
        public SchoolImageType Type { get; set; }
        public int Sort { get; set; }
    }
}

namespace Sxb.ArticleMajor.Common.QueryDto
{
    public class CityDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public static CityDto ParseProvince(ProvinceDto proviceDto)
        {
            return new CityDto { Id = proviceDto.Id, Name = proviceDto.ProvinceName, ShortName = proviceDto.ShortName };
        }

        public override bool Equals(object obj)
        {
            if ((obj as CityDto) != null)
            {
                return Id == ((CityDto)obj).Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }


    public class ProvinceDto
    {
        /// <summary>
        /// 省
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 省会城市id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 省会名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 省会简称
        /// </summary>
        public string ShortName { get; set; }
    }
}

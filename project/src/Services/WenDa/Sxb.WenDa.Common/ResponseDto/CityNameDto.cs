namespace Sxb.WenDa.Common.ResponseDto
{
    public class CityNameDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

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
}

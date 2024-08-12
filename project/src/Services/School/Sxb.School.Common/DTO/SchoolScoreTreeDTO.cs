using System.Collections.Generic;

namespace Sxb.School.Common.DTO
{
    public class SchoolScoreTreeDTO
    {
        public IndexItem CueentIndex { get; set; }
        public List<SchoolScoreTreeDTO> SubItems { get; set; }
        public double? Score { get; set; }
        public string Str_Score
        {
            get
            {
                if (Score.HasValue)
                {
                    if (Score.Value >= 90)
                    {
                        return "A+";
                    }
                    else if (Score.Value >= 80 && Score.Value < 90)
                    {
                        return "A";
                    }
                    else if (Score.Value >= 70 && Score.Value < 80)
                    {
                        return "B";
                    }
                    else if (Score.Value >= 60 && Score.Value < 70)
                    {
                        return "C";
                    }
                    else
                    {
                        return "D";
                    }
                }
                return string.Empty;
            }
        }
    }
    public class IndexItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
    }
}

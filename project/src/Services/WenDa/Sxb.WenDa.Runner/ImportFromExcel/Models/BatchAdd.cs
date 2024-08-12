using Sxb.WenDa.Common.Entity;

namespace Sxb.WenDa.Runner.ImportFromExcel.Models
{
    public class BatchAdd
    {
        public List<Question> Questions { get; set; }
        public List<QuestionAnswer> Answers { get; set; }
        public List<QuestionTag> Tags { get; set; }
    }
}

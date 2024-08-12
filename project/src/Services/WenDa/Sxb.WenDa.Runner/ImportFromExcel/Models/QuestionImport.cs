namespace Sxb.WenDa.Runner.ImportFromExcel.Models
{
    public class QuestionImport
    {
        public string CityName { get; set; }
        public string CategoryName1 { get; set; }
        public string CategoryName2 { get; set; }
        public string CategoryName3 { get; set; }
        public string TagNames { get; set; }

        public string Url { get; set; }
        public string From { get; set; }
        //问题	问题时间
        public string Title { get; set; }
        public string Time { get; set; }

        public List<AnswerImport> Answers { get; set; }
    }
}

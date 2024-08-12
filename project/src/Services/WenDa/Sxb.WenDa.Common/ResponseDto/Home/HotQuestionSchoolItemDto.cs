namespace Sxb.WenDa.Common.ResponseDto.Home
{
    public class HotQuestionSchoolItemDto
    {
        public string SchoolNo {  get; set; }

        public string SchoolName { get; set; }

        public List<QuestionLinkDto> Questions { get; set; }
    }
}

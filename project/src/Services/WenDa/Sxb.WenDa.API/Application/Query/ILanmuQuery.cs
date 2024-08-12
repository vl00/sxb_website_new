using Sxb.WenDa.Common.Enum;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Common.ResponseDto.Home;

namespace Sxb.WenDa.API.Application.Query
{
    public interface ILanmuQuery
    {
        Task<List<QuestionLinkDto>> GetHotQuestions();
        Task<List<HotQuestionSchoolItemDto>> GetHotSchools(ArticlePlatform platform, int? city, int pageIndex, int pageSize);
        Task<List<HotSubjectItemDto>> GetHotSubjects(ArticlePlatform platform);
        Task<List<SubCategoryItemDto>> GetSubCategories(ArticlePlatform platform, int city);
        Task<List<SubHotSubjectItemDto>> GetSubHotSubjects(ArticlePlatform platform, int? city);
    }
}
using Sxb.WenDa.Common.Enum;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public interface ISchoolQuestionRepository
    {
        Task<IEnumerable<Guid>> GetHotSchoolIdsAsync(ArticlePlatform platform, int top);
    }
}
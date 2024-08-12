using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public interface IQaSubjectRepository
    {
        Task<SubjectDbDto> GetSubject(Guid id = default, long no = default);
        Task<IEnumerable<SubjectDbDto>> GetSubjects(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default);

        Task<bool> IsCollectedByUser(Guid subjectId, Guid userId);

        Task<Page<Guid>> GetUserCollectSubjectIds(Guid userId, int pageIndex, int pageSize);

        /// <summary>专栏里问题列表分页</summary>
        Task<Page<Guid>> GetQuestionsIdsPageListBySubject(Guid subjectId, int pageIndex, int pageSize, SubjectQuestionListOrderByEnum orderBy);

        Task<IEnumerable<(Guid, int)>> GetSubjectsViewCounts(IEnumerable<Guid> subjectIds);
        Task<(int, int)> UpSubjectViewCount(Guid subjectId, int incr = 1);
    }
}

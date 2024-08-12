using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.API.Application.Query
{
    public interface ISubjectQuery
    {
        Task<SubjectItemDto> GetSubjectItem(Guid id = default, long no = default);
        Task<IEnumerable<SubjectItemDto>> GetSubjectItems(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default);
        Task<SubjectDetailVm> GetSubjectDetail(string id, long? city = null, Guid? me = null);

        Task<Page<SubjectItemDto>> GetMyCollectSubjects(Guid me, int pageIndex, int pageSize);

        /// <summary>
        /// 专栏里问答列表分页
        /// </summary>
        Task<Page<QaQuestionListItemDto>> GetQuestionsBySubject(GetQuestionsPageListBySubjectQuery query);
        Task<IEnumerable<SubjectItemDto>> GetSubjectItems(IEnumerable<Guid> ids, Guid userId);
    }
}
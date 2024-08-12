using Sxb.WenDa.Common.ResponseDto.School;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.OtherAPIClient.School
{
    public interface ISchoolApiService
    {
        /// <summary>根据学部eid查询学校名,id,no</summary>
        Task<List<SchoolIdAndNameDto>> GetSchoolsIdAndName(IEnumerable<string> eids);
        Task<List<SchoolIdAndNameDto>> GetSchoolsIdAndName(IEnumerable<Guid> eids);
    }
}

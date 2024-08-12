using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.IRepository;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class CorrectionMsgQuery : ICorrectionMsgQuery
    {
        readonly ICorrectionMsgRepository _correctionMsgRepository;
        public CorrectionMsgQuery(ICorrectionMsgRepository correctionMsgRepository)
        {
            _correctionMsgRepository = correctionMsgRepository;
        }

        public async Task<bool> InsertAsync(CorrectionMessageInfo entity)
        {
            var result = await _correctionMsgRepository.InsertAsync(entity);
            return result > 0;
        }
    }
}

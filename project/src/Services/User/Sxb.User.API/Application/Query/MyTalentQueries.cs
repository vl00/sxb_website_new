using Sxb.User.API.Application.Query.ViewModels;
using Sxb.User.Common.DTO;
using Sxb.User.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.User.API.Application.Query
{
    public class MyTalentQueries : IMyTalentQueries
    {
        ITalentRepository _talentRepository;
        public MyTalentQueries(ITalentRepository talentRepository)
        {
            _talentRepository = talentRepository;
        }

        public async Task<TalentDTO> GetByUserID(Guid userID)
        {
            if (userID == default) return null;
            return await _talentRepository.GetByUserID(userID);
        }

        public async Task<Guid> GetExtensionTalentUserID(Guid eid)
        {
            return await _talentRepository.GetExtensionTalentUserID(eid);
        }

        public MyTalentViewModel GetTalent(Guid userId)
        {
            return new MyTalentViewModel
            {
                UserName = "AA"
            };
        }

        public async Task<IEnumerable<TalentDTO>> ListByUserIDs(IEnumerable<Guid> userIDs)
        {
            return await _talentRepository.ListByUserIDs(userIDs);
        }
    }
}

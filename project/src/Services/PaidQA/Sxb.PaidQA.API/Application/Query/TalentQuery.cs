using Sxb.Framework.Foundation;
using Sxb.PaidQA.Common.EntityExtend;
using Sxb.PaidQA.Common.OtherAPIClient.User;
using Sxb.PaidQA.Query.SQL.IRepository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PaidQA.API.Application.Query
{
    public class TalentQuery : ITalentQuery
    {
        readonly ITalentRepository _talentRepository;
        readonly IUserAPIClient _userAPIClient;
        public TalentQuery(ITalentRepository talentRepository, IUserAPIClient userAPIClient)
        {
            _talentRepository = talentRepository;
            _userAPIClient = userAPIClient;
        }

        public async Task<TalentDetailExtend> GetDetail(Guid userID)
        {
            if (userID == default) return null;
            var talentSetting = await _talentRepository.GetTalentSetting(userID);
            if (talentSetting == default || talentSetting.TalentUserID == default) return null;
            var userInfoTask = _userAPIClient.GetTalentDetail(userID);
            var regions = await _talentRepository.GetTalentRegions(userID);
            var levelName = await _talentRepository.GetTalentLevelName(userID);
            var userInfo = await userInfoTask;
            return new TalentDetailExtend()
            {
                TalentUserID = userID,
                TalentLevelName = levelName,
                TalentRegions = regions,
                IsEnable = talentSetting.IsEnable,
                AuthName = userInfo?.Certification_preview,
                HeadImgUrl = userInfo?.HeadImgUrl,
                NickName = userInfo?.Nickname,
                TelentIntroduction = userInfo?.Introduction,
                TalentType = userInfo?.Role ?? 0
            };
        }

        public async Task<Guid> GetExtensionTalentUserIDAsync(Guid eid)
        {
            if (eid == default) return default;
            return await _talentRepository.GetExtensionTalentUserIDAsync(eid);
        }

        public async Task<TalentDetailExtend> RandomByGrade(int grade, bool isInternal = false)
        {
            if (grade < 1) return null;
            var settings = await _talentRepository.GetTalentSettingsByGradeIndex(grade, isInternal);
            if (settings?.Any() == true)
            {
                var talentUserID = CommonHelper.ListRandom(settings).FirstOrDefault()?.TalentUserID;
                if (talentUserID != default) return await GetDetail(talentUserID.Value);
            }
            return null;
        }
    }
}

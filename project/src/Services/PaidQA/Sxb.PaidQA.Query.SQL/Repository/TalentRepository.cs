using Kogel.Dapper.Extension.MsSql;
using Sxb.PaidQA.Common.Entity;
using Sxb.PaidQA.Query.SQL.DB;
using Sxb.PaidQA.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.PaidQA.Query.SQL.Repository
{
    public class TalentRepository : ITalentRepository
    {
        readonly SchoolPaidQADB _paidQADB;
        public TalentRepository(SchoolPaidQADB schoolPaidQADB)
        {
            _paidQADB = schoolPaidQADB;
        }

        public async Task<Guid> GetExtensionTalentUserIDAsync(Guid eid)
        {
            if (eid == default) return default;
            var find = await _paidQADB.SlaveConnection.QuerySet<ExtensionTalentInfo>().Where(p => p.EID == eid).GetAsync();
            if (find?.ID > 0) return find.TalentUserID;
            return default;
        }

        public async Task<IEnumerable<GradeInfo>> GetTalentGrade(Guid userID)
        {
            if (userID == default) return null;
            return await _paidQADB.SlaveConnection.QuerySet<GradeInfo>().Join<GradeInfo, TalentGradeInfo>((x, y) => x.ID == y.GradeID).Where<TalentGradeInfo>(p => p.TalentUserID == userID).ToIEnumerableAsync();
        }

        public async Task<string> GetTalentLevelName(Guid userID)
        {
            if (userID == default) return null;
            return await _paidQADB.SlaveConnection.QuerySet<LevelTypeInfo>().Join<LevelTypeInfo, TalentSettingInfo>((x, y) => x.ID == y.TalentLevelTypeID).Where<TalentSettingInfo>(p => p.TalentUserID == userID).GetAsync(p => p.Name);
        }

        public async Task<IEnumerable<RegionTypeInfo>> GetTalentRegions(Guid userID)
        {
            if (userID == default) return null;
            return await _paidQADB.SlaveConnection.QuerySet<RegionTypeInfo>().Join<RegionTypeInfo, TalentRegionInfo>((x, y) => x.ID == y.RegionTypeID).Where<TalentRegionInfo>(p => p.UserID == userID).ToIEnumerableAsync();
        }

        public async Task<TalentSettingInfo> GetTalentSetting(Guid userID)
        {
            if (userID == default) return null;
            return await _paidQADB.SlaveConnection.QuerySet<TalentSettingInfo>().Where(p => p.TalentUserID == userID && p.IsEnable == true).GetAsync();
        }

        public async Task<IEnumerable<TalentSettingInfo>> GetTalentSettingsByGradeIndex(int gradeIndex, bool isInternal = false)
        {
            if (gradeIndex < 1) return null;
            var query = _paidQADB.SlaveConnection.QuerySet<TalentSettingInfo>()
                .Join<TalentSettingInfo, TalentGradeInfo>((x, y) => x.TalentUserID == y.TalentUserID)
                .Join<TalentGradeInfo, GradeInfo>((x, y) => x.GradeID == y.ID)
                .Where<GradeInfo>(p => p.Sort == gradeIndex)
                .Where<TalentSettingInfo>(p=>p.IsEnable == true)
                .Where("NOT EXISTS (SELECT 1 FROM iSchoolUser.dbo.[talentSchoolExtension] WHERE iSchoolUser.dbo.[talentSchoolExtension].TalentID = [TalentSetting].[TalentUserID])");
            if (isInternal)
            {
                query.Join<dynamic>("Left Join iSchoolUser.dbo.[Talent] on [Talent].[User_ID] = [TalentSetting].[TalentUserID]").Where("[Talent].[IsInternal] = 1");
            }
            return await query.ToIEnumerableAsync();
        }
    }
}
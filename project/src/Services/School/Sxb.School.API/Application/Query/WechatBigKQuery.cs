using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class WechatBigKQuery : IWechatBigKQuery
    {
        readonly IWechatBigKRepository _wechatBigKRepository;
        public WechatBigKQuery(IWechatBigKRepository wechatBigKRepository)
        {
            _wechatBigKRepository = wechatBigKRepository;
        }

        public async Task<IEnumerable<WeChatRecruitArticleInfo>> GetArticles(IEnumerable<Guid> ids)
        {
            return await _wechatBigKRepository.GetArticlesAsync(ids);
        }

        public async Task<IEnumerable<WeChatRecruitInfo>> GetAsync(int areaCode, WeChatRecruitType type, SchoolGradeType grade, int? year)
        {
            if (areaCode < 100000 || type == WeChatRecruitType.Unknow || grade == SchoolGradeType.Defalut) return default;
            return await _wechatBigKRepository.GetAsync(type, areaCode, grade, year);
        }

        public async Task<IEnumerable<WeChatRecruitAttachmentInfo>> GetAttachments(IEnumerable<Guid> ids)
        {
            return await _wechatBigKRepository.GetAttachmentsAsync(ids);
        }

        public async Task<IEnumerable<WeChatRecruitItemInfo>> GetRalations(IEnumerable<Guid> recruitIDs, WeChatRecruitItemType type = WeChatRecruitItemType.Unknow)
        {
            return await _wechatBigKRepository.GetItemsAsync(recruitIDs, type);
        }

        public async Task<IEnumerable<WeChatRecruitScheduleInfo>> GetSchedules(IEnumerable<Guid> ids)
        {
            return await _wechatBigKRepository.GetSchedulesAsync(ids);
        }

        public async Task<IEnumerable<int>> GetYearsAsync(int areaCode, WeChatRecruitType type, SchoolGradeType grade)
        {
            var finds = await _wechatBigKRepository.GetYearsAsync(areaCode, type, grade);
            if (finds?.Any() == true) return finds;
            return default;
        }

        public async Task<bool> InsertAsync(WeChatRecruitInfo entity)
        {
            return await _wechatBigKRepository.InsertAsync(entity) > 0;
        }

        public async Task<bool> InsertAsync(WeChatRecruitArticleInfo entity)
        {
            return await _wechatBigKRepository.InsertAsync(entity) > 0;
        }

        public async Task<bool> InsertAsync(WeChatRecruitAttachmentInfo entity)
        {
            return await _wechatBigKRepository.InsertAsync(entity) > 0;
        }

        public async Task<bool> InsertAsync(WeChatRecruitItemInfo entity)
        {
            return await _wechatBigKRepository.InsertAsync(entity) > 0;
        }

        public async Task<bool> InsertAsync(WeChatRecruitScheduleInfo entity)
        {
            return await _wechatBigKRepository.InsertAsync(entity) > 0;
        }

        public async Task<int> RemoveManyAsync(IEnumerable<WeChatRecruitInfo> entities)
        {
            if (entities == null || !entities.Any()) return 0;
            return await _wechatBigKRepository.RemoveByParamsAsync(entities.Select(p => $"{(int)p.Type}{(int)p.Grade}{p.Year}{p.AreaCode}"));
        }
    }
}

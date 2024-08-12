using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface IWechatBigKQuery
    {
        Task<bool> InsertAsync(WeChatRecruitInfo entity);

        Task<bool> InsertAsync(WeChatRecruitArticleInfo entity);

        Task<bool> InsertAsync(WeChatRecruitAttachmentInfo entity);

        Task<bool> InsertAsync(WeChatRecruitItemInfo entity);

        Task<bool> InsertAsync(WeChatRecruitScheduleInfo entity);

        Task<int> RemoveManyAsync(IEnumerable<WeChatRecruitInfo> entities);
        Task<IEnumerable<WeChatRecruitInfo>> GetAsync(int areaCode, WeChatRecruitType type, SchoolGradeType grade, int? year);
        Task<IEnumerable<WeChatRecruitItemInfo>> GetRalations(IEnumerable<Guid> recruitIDs, WeChatRecruitItemType type = WeChatRecruitItemType.Unknow);
        Task<IEnumerable<WeChatRecruitArticleInfo>> GetArticles(IEnumerable<Guid> ids);
        Task<IEnumerable<WeChatRecruitScheduleInfo>> GetSchedules(IEnumerable<Guid> ids);
        Task<IEnumerable<WeChatRecruitAttachmentInfo>> GetAttachments(IEnumerable<Guid> ids);
        Task<IEnumerable<int>> GetYearsAsync(int areaCode, WeChatRecruitType type, SchoolGradeType grade);
    }
}

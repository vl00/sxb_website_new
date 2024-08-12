using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.IRepository
{
    public interface IWechatBigKRepository
    {
        /// <summary>
        /// 获取大条目
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="type">类型</param>
        /// <param name="areaCode">区域代号</param>
        /// <param name="grade">学段</param>
        /// <returns></returns>
        Task<IEnumerable<WeChatRecruitInfo>> GetAsync(WeChatRecruitType type, int areaCode, SchoolGradeType? grade, int? year = null);
        /// <summary>
        /// 获取关联关系
        /// </summary>
        /// <param name="recruitID">大表ID</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        Task<IEnumerable<WeChatRecruitItemInfo>> GetItemsAsync(IEnumerable<Guid> recruitIDs, WeChatRecruitItemType type);
        /// <summary>
        /// 批量获取文章
        /// </summary>
        /// <param name="ids">文章IDs</param>
        /// <returns></returns>
        Task<IEnumerable<WeChatRecruitArticleInfo>> GetArticlesAsync(IEnumerable<Guid> ids);
        /// <summary>
        /// 获取附件
        /// </summary>
        /// <param name="ids">附件IDs</param>
        /// <returns></returns>
        Task<IEnumerable<WeChatRecruitAttachmentInfo>> GetAttachmentsAsync(IEnumerable<Guid> ids);
        /// <summary>
        /// 获取日程
        /// </summary>
        /// <param name="ids">日程IDs</param>
        /// <returns></returns>
        Task<IEnumerable<WeChatRecruitScheduleInfo>> GetSchedulesAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// 获取有效期内的日程
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<IEnumerable<WeChatRecruitScheduleInfo>> GetAvaliableSchedulesAsync(DateTime date);

        /// <summary>
        /// 根据参数删除
        /// </summary>
        /// <param name="deleteParams">删除参数
        /// <para>{Type}{SchFType}{Year}{AreaCode}</para>
        /// </param>
        /// <returns></returns>
        Task<int> RemoveByParamsAsync(IEnumerable<string> deleteParams);
        Task<int> InsertAsync(WeChatRecruitInfo entity);
        Task<int> InsertAsync(WeChatRecruitArticleInfo entity);
        Task<int> InsertAsync(WeChatRecruitScheduleInfo entity);
        Task<int> InsertAsync(WeChatRecruitItemInfo entity);
        Task<int> InsertAsync(WeChatRecruitAttachmentInfo entity);

        Task<IEnumerable<int>> GetYearsAsync(int areaCode, WeChatRecruitType type, SchoolGradeType grade);
    }
}

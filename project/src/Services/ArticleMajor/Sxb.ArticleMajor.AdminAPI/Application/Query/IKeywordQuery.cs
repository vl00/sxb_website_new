using Kogel.Dapper.Extension;
using Sxb.ArticleMajor.Common.Entity;
using Sxb.ArticleMajor.Common.Enum;

namespace Sxb.ArticleMajor.AdminAPI.Application.Query
{
    public interface IKeywordQuery
    {
        Task<KeywordInfo> GetAysnc(Guid id);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="siteType">站点类型</param>
        /// <param name="cityCode">城市代码</param>
        /// <param name="pageType">页面类型</param>
        /// <param name="positionType">位置类型</param>
        /// <returns></returns>
        Task<IEnumerable<KeywordInfo>> ListAsync(KeywordSiteType? siteType, int? cityCode, KeywordPageType? pageType, KeywordPositionType? positionType);
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        Task<PageList<KeywordInfo>> PageAsync(int pageIndex = 1, int pageSize = 10);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        Task<bool> RemoveAsync(Guid id);
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<bool> SaveAsync(KeywordInfo entity);
    }
}

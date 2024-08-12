using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
        Task<IEnumerable<CategoryChildDto>> GetPlatformChildren();

        /// <summary>
        /// 用户可选择的关注领域的分类ids 查询 问题的分类ids (2级 查 2级or3级)
        /// </summary>
        /// <param name="attentionCategoryIds">用户可选择的关注领域的分类ids</param>
        /// <returns></returns>
        Task<IEnumerable<Category>> GetQuesCategoryIdsByAttentionCategoryIds(IEnumerable<long> attentionCategoryIds);
    }
}
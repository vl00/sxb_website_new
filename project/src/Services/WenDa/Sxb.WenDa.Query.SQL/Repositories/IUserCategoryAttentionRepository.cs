namespace Sxb.WenDa.Query.SQL.Repositories
{
    public interface IUserCategoryAttentionRepository
    {
        Task<IEnumerable<long>> GetUserCategoryIdsAsync(Guid userId);

        /// <summary>关注同学段同领域的用户</summary>
        Task<IEnumerable<Guid>> GetUserByAttentionSameCategory(long categoryId, int top);
    }
}
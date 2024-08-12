using Sxb.WenDa.Query.SQL.QueryDto;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public interface IInvitationRepository
    {
        Task<IEnumerable<NotifyUserQueryDto>> GetInvitationToUserAsync(DateTime startTime, DateTime endTime, int pageIndex, int pageSize);
    }
}
using Sxb.User.Common.DTO;
using Sxb.User.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.User.API.Application.Query
{
    public class UserQuery : IUserQuery
    {
        readonly IUserRepository _userRepository;
        public UserQuery(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> CheckIsSubscribe(Guid userId)
        {
            return await _userRepository.GetSubscribe(userId);
        }

        public Task<bool> IsRealUser(Guid userId)
        {
            return _userRepository.IsRealUser(userId);
        }

        public Task<bool> IsUserBindMobile(Guid userId)
        {
            return _userRepository.IsUserBindMobile(userId);
        }

        public Task<IEnumerable<UserDescDto>> GetUsersDesc(IEnumerable<Guid> userIds)
        {
            return _userRepository.GetUsersDesc(userIds);
        }
        public Task<IEnumerable<UserDescDto>> GetUsersDesc2(IEnumerable<Guid> userIds)
        {
            return _userRepository.GetUsersDesc2(userIds);
        }

        public Task<UserWxUnionIdDto> GetUserWxUnionId(string id)
        {
            var userId = Guid.TryParse(id, out var _userId) ? _userId : default;
            var unionId = userId == default ? id : default;
            return _userRepository.GetUserWxUnionId(userId, unionId);
        }

        public async Task<IEnumerable<TalentUserDescDto>> GetTopNTalentUserByGrade(int grade, int top)
        {
            var ls_id = await _userRepository.GetTopNTalentUserIdByGrade(grade, top);
            if (!ls_id.Any()) return Enumerable.Empty<TalentUserDescDto>();

            var ls_user = await _userRepository.GetUsersDesc(ls_id.Select(_ => _.UserId));
            var result = ls_user.Select(x =>
            {
                var t = new TalentUserDescDto();
                t.Id = x.Id;
                t.Name = x.Name;
                t.HeadImg = x.HeadImg;
                t.CertificationIdentity = x.CertificationIdentity;
                t.CertificationTitle = x.CertificationTitle;
                t.CertificationPreview = x.CertificationPreview;
                t.IsInternal = ls_id.FirstOrDefault(_ => _.UserId == x.Id).IsInternal;
                return t;
            });
            return result.ToArray();
        }

        public async Task<IEnumerable<UserDescDto>> GetTopNRandVirtualUser(int top)
        {
            var ls_id = await _userRepository.GetTopNRandVirtualUserId(top);
            if (!ls_id.Any()) return Enumerable.Empty<UserDescDto>();

            var ls_user = await _userRepository.GetUsersDesc(ls_id);
            return ls_user.ToArray();
        }

        public async Task<IEnumerable<(Guid, string)>> GetNicknames(IEnumerable<Guid> ids)
        {
            return await _userRepository.GetNicknames(ids);
        }

        public async Task<IEnumerable<UserWxFwhDto>> GetFwhOpenIdAndNicknames(IEnumerable<Guid> ids)
        {
            return await _userRepository.GetFwhOpenIdAndNicknames(ids);
        }
    }
}

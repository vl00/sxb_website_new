using MediatR;
using Sxb.Framework.AspNetCoreHelper.CheckException;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Domain.AggregateModel.UserCategoryAttentionAggregate;
using Sxb.WenDa.Infrastructure.Repositories;

namespace Sxb.WenDa.API.Application.Commands
{
    public class UserCategoryAttentionCommandHandler : IRequestHandler<UserCategoryAttentionCommand, ResponseResult>
    {
        private readonly IUserCategoryAttentionRepository _userCategoryAttentionRepository;

        public UserCategoryAttentionCommandHandler(IUserCategoryAttentionRepository userCategoryAttentionRepository)
        {
            _userCategoryAttentionRepository = userCategoryAttentionRepository;
        }

        public async Task<ResponseResult> Handle(UserCategoryAttentionCommand request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;
            var categoryIds = request.CategoryIds;
            var createTime = request.CreateTime;

            //暂时不验证categoryId是否存在
            //一个学段最多选择3个擅长领域
            int max = 3 * 4;
            Check.IsTrue(userId != Guid.Empty, "请先登录");
            Check.HasValue(categoryIds, "请选择您关注的领域");
            Check.IsTrue(categoryIds.Count <= max, "每个学段最多选择3个擅长领域");

            var entities = _userCategoryAttentionRepository
                                .GetAllIQueryable(s => s.UserId == userId)
                                .ToList();

            //未选中的设为fale
            foreach (var entity in entities)
            {
                if (!categoryIds.Contains(entity.CategoryId))
                    entity.SetIsValid(false);
            }

            var addEntities = new List<UserCategoryAttention>();
            foreach (var categoryId in request.CategoryIds)
            {
                var entity = entities.FirstOrDefault(e => e.CategoryId == categoryId);
                if (entity == null)
                {
                    entity = new UserCategoryAttention(userId, categoryId, createTime);

                    _userCategoryAttentionRepository.Add(entity);
                    entities.Add(entity);
                }
                //更新为true
                entity.SetIsValid(true);
            }

            await _userCategoryAttentionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            //批量更新
            _userCategoryAttentionRepository.UpdateRange(entities);

            return ResponseResult.Success();
        }
    }
}

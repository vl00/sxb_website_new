using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;
using Sxb.WenDa.Domain.AggregateModel.UserCategoryAttentionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Infrastructure.EntityConfigurations
{
    public class UserCategoryAttentionETC : IEntityTypeConfiguration<UserCategoryAttention>
    {
        public void Configure(EntityTypeBuilder<UserCategoryAttention> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable(nameof(UserCategoryAttention));
        }
    }
}

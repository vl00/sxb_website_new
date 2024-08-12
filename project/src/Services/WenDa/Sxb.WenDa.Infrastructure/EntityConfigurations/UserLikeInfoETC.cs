using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.WenDa.Domain.AggregateModel.LikeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Infrastructure.EntityConfigurations
{
    public class UserLikeInfoETC : IEntityTypeConfiguration<UserLikeInfo>
    {
        public void Configure(EntityTypeBuilder<UserLikeInfo> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable(nameof(UserLikeInfo));
        }
    }
}

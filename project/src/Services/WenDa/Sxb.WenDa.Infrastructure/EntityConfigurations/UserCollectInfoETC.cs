using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Infrastructure.EntityConfigurations
{
    public class UserCollectInfoETC : IEntityTypeConfiguration<UserCollectInfo>
    {
        public void Configure(EntityTypeBuilder<UserCollectInfo> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable(nameof(UserCollectInfo));
        }
    }
}

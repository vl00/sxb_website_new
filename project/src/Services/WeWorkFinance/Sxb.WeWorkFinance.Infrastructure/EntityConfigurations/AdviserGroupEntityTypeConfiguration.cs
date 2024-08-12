using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.WeWorkFinance.Domain.AggregatesModel.ContactAggregate;
using Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Infrastructure.EntityConfigurations
{
    public class AdviserGroupEntityTypeConfiguration : IEntityTypeConfiguration<AdviserGroup>
    {
        public void Configure(EntityTypeBuilder<AdviserGroup> builder)
        {
            builder.HasKey(p => p.Id);
            builder.ToTable("AdviserGroup");
        }
    }
}

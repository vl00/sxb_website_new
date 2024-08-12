using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.WeWorkFinance.Domain.AggregatesModel.LogAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Infrastructure.EntityConfigurations
{
    public class LogCloseOrderEntityTypeConfiguration : IEntityTypeConfiguration<LogCloseOrder>
    {
        public void Configure(EntityTypeBuilder<LogCloseOrder> builder)
        {
            builder.HasKey(p => p.Id);
            builder.ToTable("LogCloseOrder");
        }
    }
}

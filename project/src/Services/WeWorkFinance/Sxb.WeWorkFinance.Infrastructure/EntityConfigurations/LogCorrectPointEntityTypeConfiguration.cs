using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.WeWorkFinance.Domain.AggregatesModel.LogAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Infrastructure.EntityConfigurations
{
    public class LogCorrectPointEntityTypeConfiguration : IEntityTypeConfiguration<LogCorrectPoint>
    {
        public void Configure(EntityTypeBuilder<LogCorrectPoint> builder)
        {
            builder.HasKey(p => p.Id);
            builder.ToTable("LogCorrectPoint");
        }
    }
}

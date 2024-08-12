using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.SignActivity.Infrastructure.EntityConfigurations
{
    public class SignConfigEntityTypeConfiguration : IEntityTypeConfiguration<SignConfig>
    {
        public void Configure(EntityTypeBuilder<SignConfig> builder)
        {
            builder.HasNoKey();
            builder.ToTable("sign_config");
        }
    }
}

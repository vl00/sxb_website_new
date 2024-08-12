using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.SignActivity.Infrastructure.EntityConfigurations
{
    public class SignInParentHistoryEntityTypeConfiguration : IEntityTypeConfiguration<SignInParentHistory>
    {
        public void Configure(EntityTypeBuilder<SignInParentHistory> builder)
        {
            builder.HasKey(p => p.Id);
            builder.ToTable("sign_in_parent_history");
        }
    }
}

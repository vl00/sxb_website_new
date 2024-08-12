using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.SignActivity.Infrastructure.EntityConfigurations
{
    public class SignInEntityTypeConfiguration : IEntityTypeConfiguration<SignIn>
    {
        public void Configure(EntityTypeBuilder<SignIn> builder)
        {
            builder.HasKey(p => p.Id);
            builder.ToTable("sign_in");
        }
    }
}

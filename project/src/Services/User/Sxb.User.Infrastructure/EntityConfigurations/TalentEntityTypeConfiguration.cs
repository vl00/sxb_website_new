using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.User.Domain.AggregatesModel.TalentAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.User.Infrastructure.EntityConfigurations
{
    public class TalentEntityTypeConfiguration : IEntityTypeConfiguration<Talent>
    {
        public void Configure(EntityTypeBuilder<Talent> builder)
        {
            builder.HasKey(p => p.Id);
            builder.ToTable("talent");
        }
    }
}

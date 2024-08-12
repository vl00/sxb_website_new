﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Infrastructure.EntityConfigurations
{
    public class GroupUserEntityTypeConfiguration : IEntityTypeConfiguration<GroupUser>
    {
        public void Configure(EntityTypeBuilder<GroupUser> builder)
        {
            builder.HasKey(p => p.Id);
            builder.ToTable("GroupUser");
        }
    }
}

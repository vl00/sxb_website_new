using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sxb.Infrastructure.Core;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using Sxb.SignActivity.Infrastructure.EntityConfigurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.SignActivity.Infrastructure
{

    public class Data { public decimal Total { get; set; } }
    public class OrganizationContext : EFContext
    {
        public OrganizationContext(DbContextOptions options, IMediator mediator, ICapPublisher capBus) : base(options, mediator, capBus)
        {
        }

        public DbSet<SignIn> SignIns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 注册领域模型与数据库的映射关系
            modelBuilder.ApplyConfiguration(new SignInEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SignInHistoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SignInParentHistoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SignConfigEntityTypeConfiguration());

            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}

using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sxb.Infrastructure.Core;
using Sxb.WeWorkFinance.Domain.AggregatesModel.ContactAggregate;
using Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate;
using Sxb.WeWorkFinance.Domain.AggregatesModel.LogAggregate;
using Sxb.WeWorkFinance.Infrastructure.EntityConfigurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Infrastructure
{
    public class UserContext : EFContext
    {
        public UserContext(DbContextOptions options, IMediator mediator, ICapPublisher capBus) : base(options, mediator, capBus)
        {
        }

        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<AdviserGroup> AdviserGroups { get; set; }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<CustomerQrCode> CustomerQrCodes { get; set; }

        public DbSet<LogCloseOrder> LogCloseOrders { get; set; }
        public DbSet<LogCorrectPoint> LogCorrectPoints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 注册领域模型与数据库的映射关系
            modelBuilder.ApplyConfiguration(new GroupChatEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GroupMemberEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GroupUserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AdviserGroupEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ContactEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerQrCodeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LogCloseOrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LogCorrectPointEntityTypeConfiguration());
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}

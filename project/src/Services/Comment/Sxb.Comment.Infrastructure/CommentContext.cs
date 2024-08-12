using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sxb.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Comment.Infrastructure
{
    public class CommentContext : EFContext
    {
        public CommentContext(DbContextOptions options, IMediator mediator, ICapPublisher capBus) : base(options, mediator, capBus)
        {
        }

        //public DbSet<Talent> Talents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 注册领域模型与数据库的映射关系
            //modelBuilder.ApplyConfiguration(new TalentEntityTypeConfiguration());
            #endregion
            base.OnModelCreating(modelBuilder);
        }
    }
}

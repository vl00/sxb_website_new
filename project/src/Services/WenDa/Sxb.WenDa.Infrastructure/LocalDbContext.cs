using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sxb.Infrastructure.Core;
using Sxb.WenDa.Domain.AggregateModel.LikeAggregate;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;
using Sxb.WenDa.Domain.AggregateModel.SubjectAggregate;
using Sxb.WenDa.Domain.AggregateModel.UserCategoryAttentionAggregate;
using Sxb.WenDa.Infrastructure.EntityConfigurations;

namespace Sxb.WenDa.Infrastructure
{
    public class LocalDbContext : EFContext
    {
        public LocalDbContext(DbContextOptions options, IMediator mediator, ICapPublisher capBus) : base(options, mediator, capBus)
        {
        }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Comment > Comments { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        
        public DbSet<UserCollectInfo> UserCollectInfos { get; set; }
        public DbSet<UserLikeInfo> UserLikeInfos { get; set; }
        public DbSet<UserCategoryAttention> UserCategoryAttentions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new QuestionETC());
            modelBuilder.ApplyConfiguration(new AnswerETC());
            modelBuilder.ApplyConfiguration(new CommentETC());
            modelBuilder.ApplyConfiguration(new SubjectETC());


            modelBuilder.ApplyConfiguration(new UserLikeInfoETC());
            modelBuilder.ApplyConfiguration(new UserCollectInfoETC());
            modelBuilder.ApplyConfiguration(new UserCategoryAttentionETC());
            

            base.OnModelCreating(modelBuilder);
        }
    }
}
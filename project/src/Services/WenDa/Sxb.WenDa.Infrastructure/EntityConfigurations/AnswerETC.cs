using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;

namespace Sxb.WenDa.Infrastructure.EntityConfigurations
{
    public class AnswerETC : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("QuestionAnswer");
        }
    }
}

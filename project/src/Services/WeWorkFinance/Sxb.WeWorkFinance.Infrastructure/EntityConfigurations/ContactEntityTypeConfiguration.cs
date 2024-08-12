using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.WeWorkFinance.Domain.AggregatesModel.ContactAggregate;

namespace Sxb.WeWorkFinance.Infrastructure.EntityConfigurations
{
    public class ContactEntityTypeConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.HasKey(p => p.Id);
            builder.ToTable("Contact");
        }
    }
}

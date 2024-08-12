using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sxb.WeWorkFinance.Domain.AggregatesModel.ContactAggregate;

namespace Sxb.WeWorkFinance.Infrastructure.EntityConfigurations
{
    public class CustomerQrCodeEntityTypeConfiguration : IEntityTypeConfiguration<CustomerQrCode>
    {
        public void Configure(EntityTypeBuilder<CustomerQrCode> builder)
        {
            builder.HasKey(p => p.Id);
            builder.ToTable("CustomerQrCode");
        }
    }
}

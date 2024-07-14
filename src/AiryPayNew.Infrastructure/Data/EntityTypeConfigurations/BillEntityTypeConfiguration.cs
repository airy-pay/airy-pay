using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Bills.BillSecrets;
using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Infrastructure.Data.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryPayNew.Infrastructure.Data.EntityTypeConfigurations;

internal class BillEntityTypeConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.ToTable("bills")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value, 
                id => new BillId(id))
            .HasValueGenerator<IdValueGenerator<BillId>>()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.PaymentSystemName)
            .HasMaxLength(128);
        builder.Property(x => x.PaymentMethodId)
            .HasMaxLength(128);
        
        builder.Property(x => x.BillSecret)
            .HasConversion(
                x => x.Key,
                x => new BillSecret(x))
            .HasMaxLength(512)
            .HasColumnName("bill_secret");

        builder.HasOne(x => x.Product)
            .WithMany(x => x.Bills)
            .HasForeignKey(x => x.ProductId);
        builder.HasOne(x => x.Purchase)
            .WithOne(x => x.Bill)
            .HasForeignKey<Purchase>(x => x.BillId);
    }
}
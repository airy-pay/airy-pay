using AiryPay.Domain.Entities.Bills;
using AiryPay.Domain.Entities.Bills.BillSecrets;
using AiryPay.Domain.Entities.Purchases;
using AiryPay.Infrastructure.Data.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryPay.Infrastructure.Data.EntityTypeConfigurations;

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

        builder.OwnsOne(x => x.Payment, payment =>
        {
            payment.Property(p => p.SystemId)
                .HasColumnName("system_id");
            payment.Property(p => p.MethodId)
                .HasColumnName("method_id")
                .HasMaxLength(128);;
            payment.Property(p => p.SystemName)
                .HasColumnName("system_name")
                .HasMaxLength(128);;
        });

        builder.Navigation(b => b.Payment)
            .IsRequired();
        
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
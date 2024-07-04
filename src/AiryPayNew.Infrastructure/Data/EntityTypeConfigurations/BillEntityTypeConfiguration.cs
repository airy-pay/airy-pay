using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Bills.BillSecrets;
using AiryPayNew.Infrastructure.Data.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryPayNew.Infrastructure.Data.EntityTypeConfigurations;

public class BillEntityTypeConfiguration : IEntityTypeConfiguration<Bill>
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

        builder.Property(x => x.BillSecret)
            .HasConversion(
                x => x.Key,
                x => new BillSecret(x))
            .HasColumnName("bill_secret");

        builder.HasOne(x => x.Product)
            .WithMany(x => x.Bills);
    }
}
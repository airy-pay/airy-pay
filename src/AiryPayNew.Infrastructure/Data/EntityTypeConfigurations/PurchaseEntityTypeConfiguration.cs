using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Infrastructure.Data.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryPayNew.Infrastructure.Data.EntityTypeConfigurations;

internal class PurchaseEntityTypeConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("purchases")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value, 
                id => new PurchaseId(id))
            .HasValueGenerator<IdValueGenerator<PurchaseId>>()
            .ValueGeneratedOnAdd();

        builder.HasOne(x => x.Shop);
        builder.HasOne(x => x.Bill);
    }
}
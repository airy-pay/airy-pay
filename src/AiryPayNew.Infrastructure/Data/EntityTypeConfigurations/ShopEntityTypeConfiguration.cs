using AiryPayNew.Domain.Entities.Shops;
using AiryPayNew.Infrastructure.Data.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryPayNew.Infrastructure.Data.EntityTypeConfigurations;

internal class ShopEntityTypeConfiguration : IEntityTypeConfiguration<Shop>
{
    public void Configure(EntityTypeBuilder<Shop> builder)
    {
        builder.ToTable("shops")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value, 
                id => new ShopId(id))
            .HasValueGenerator<IdValueGenerator<ShopId>>()
            .ValueGeneratedOnAdd();

        builder.HasMany(x => x.Products)
            .WithOne(x => x.Shop);
        builder.HasMany(x => x.Purchases)
            .WithOne(x => x.Shop);
        builder.HasMany(x => x.Bills)
            .WithOne(x => x.Shop)
            .HasForeignKey(x => x.ShopId);
        builder.HasMany(x => x.Withdrawals)
            .WithOne(x => x.Shop)
            .HasForeignKey(x => x.ShopId);
    }
}
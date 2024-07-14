using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Infrastructure.Data.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryPayNew.Infrastructure.Data.EntityTypeConfigurations;

internal class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value, 
                id => new ProductId(id))
            .HasValueGenerator<IdValueGenerator<ProductId>>()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Emoji)
            .HasMaxLength(256);
        builder.Property(x => x.Name)
            .HasMaxLength(128);
        
        builder.HasOne(x => x.Shop)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.ShopId);
    }
}
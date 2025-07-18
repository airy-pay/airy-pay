using AiryPayNew.Domain.Entities.ShopComplaints;
using AiryPayNew.Infrastructure.Data.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryPayNew.Infrastructure.Data.EntityTypeConfigurations;

public class ShopComplaintEntityTypeConfiguration : IEntityTypeConfiguration<ShopComplaint>
{
    public void Configure(EntityTypeBuilder<ShopComplaint> builder)
    {
        builder.ToTable("shop_complaints")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value, 
                id => new ShopComplaintId(id))
            .HasValueGenerator<IdValueGenerator<ShopComplaintId>>()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Reason)
            .HasMaxLength(ShopComplaint.Constants.ReasonMaxLength)
            .IsRequired();
        builder.Property(x => x.Details)
            .HasMaxLength(ShopComplaint.Constants.DetailsMaxLength);
        
        builder.HasOne(x => x.Shop)
            .WithMany(x => x.Complaints)
            .HasForeignKey(x => x.ShopId);
    }
}
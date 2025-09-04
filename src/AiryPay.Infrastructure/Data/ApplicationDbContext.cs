using System.Reflection;
using AiryPay.Domain.Entities.Bills;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Purchases;
using AiryPay.Domain.Entities.Shops;
using AiryPay.Domain.Entities.Withdrawals;
using Microsoft.EntityFrameworkCore;

namespace AiryPay.Infrastructure.Data;

internal class ApplicationDbContext(DbContextOptions<ApplicationDbContext> contextOptions) : DbContext(contextOptions)
{
    public DbSet<Bill> Bills { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Shop> Shops { get; set; }
    public DbSet<Withdrawal> Withdrawals { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
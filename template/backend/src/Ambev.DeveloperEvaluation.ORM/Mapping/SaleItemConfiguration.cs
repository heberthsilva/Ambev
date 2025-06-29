using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
    {
        public void Configure(EntityTypeBuilder<SaleItem> builder)
        {
            builder.ToTable("SaleItems");

            builder.HasKey(si => si.Id);
            builder.Property(si => si.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

            builder.Property(si => si.ProductId).IsRequired().HasColumnType("uuid");
            builder.Property(si => si.ProductName).IsRequired().HasMaxLength(255);
            builder.Property(si => si.Quantity).IsRequired();
            builder.Property(si => si.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(si => si.Discount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(si => si.TotalItemAmount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(si => si.IsCancelled).IsRequired();

            // Foreign key to Sale
            builder.Property<Guid>("SaleId").IsRequired();
        }
    }
}
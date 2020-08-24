using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace E_Voucher.Entities.Database.EVoucherSystem.DBModelMap
{
    public class TblPaymentMethodMap : IEntityTypeConfiguration<TblPaymentMethod>
    {
        public void Configure(EntityTypeBuilder<TblPaymentMethod> entity)
        {
            entity.HasKey(e => e.PaymentMethod)
                    .HasName("PK_TBL_PAYMENT_METHOD");

            entity.ToTable("Tbl_Payment_Method");

            entity.Property(e => e.PaymentMethod)
                .HasColumnName("Payment_Method")
                .HasMaxLength(10);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.DiscountPercentage)
                .HasColumnName("Discount_Percentage")
                .HasDefaultValueSql("('0')");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.IsDiscount)
                .IsRequired()
                .HasColumnName("Is_Discount")
                .HasDefaultValueSql("('0')");

            entity.Property(e => e.Status).HasDefaultValueSql("('0')");
        }
    }
}

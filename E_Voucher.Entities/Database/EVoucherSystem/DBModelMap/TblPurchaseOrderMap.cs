using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace E_Voucher.Entities.Database.EVoucherSystem.DBModelMap
{
    public class TblPurchaseOrderMap : IEntityTypeConfiguration<TblPurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<TblPurchaseOrder> entity)
        {
            entity.HasKey(e => e.PurchaseOrderNo)
                    .HasName("PK_TBL_PURCHASE_ORDER");

            entity.ToTable("Tbl_Purchase_Order");

            entity.Property(e => e.PurchaseOrderNo)
                .HasColumnName("PurchaseOrder_No")
                .HasMaxLength(20);

            entity.Property(e => e.BuyType)
                .IsRequired()
                .HasColumnName("Buy_Type")
                .HasMaxLength(20);

            entity.Property(e => e.BuyerName)
                .HasColumnName("Buyer_Name")
                .HasMaxLength(300);

            entity.Property(e => e.BuyerPhone)
                .IsRequired()
                .HasColumnName("Buyer_Phone")
                .HasMaxLength(50);

            entity.Property(e => e.ExpiryDate)
                .HasColumnName("Expiry_Date")
                .HasColumnType("datetime");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.ImagePath)
                .HasColumnName("Image_Path")
                .HasMaxLength(100);

            entity.Property(e => e.OrderDate)
                .HasColumnName("Order_Date")
                .HasColumnType("datetime");

            entity.Property(e => e.PaymentMethod)
                .IsRequired()
                .HasColumnName("Payment_Method")
                .HasMaxLength(10);

            entity.Property(e => e.SellingDiscount)
                .HasColumnName("Selling_Discount")
                .HasDefaultValueSql("('0')");

            entity.Property(e => e.SellingPrice)
                .HasColumnName("Selling_Price")
                .HasColumnType("money");

            entity.Property(e => e.TotalSellingAmount)
                .HasColumnName("Total_Selling_Amount")
                .HasColumnType("money");

            entity.Property(e => e.VoncherAmount)
                .HasColumnName("Voncher_Amount")
                .HasColumnType("money");

            entity.Property(e => e.VoucherGenerated)
                .IsRequired()
                .HasColumnName("Voucher_Generated")
                .HasDefaultValueSql("('0')");

            entity.Property(e => e.VoucherNo)
                .IsRequired()
                .HasColumnName("Voucher_No")
                .HasMaxLength(20);
        }
    }
}

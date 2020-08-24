using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace E_Voucher.Entities.Database.EVoucherSystem.DBModelMap
{
    public class TblEvoucherMap : IEntityTypeConfiguration<TblEvoucher>
    {
        public void Configure(EntityTypeBuilder<TblEvoucher> entity)
        {
            entity.HasKey(e => e.VoucherNo)
                    .HasName("PK_TBL_EVOUCHER");

            entity.ToTable("Tbl_EVoucher");

            entity.Property(e => e.VoucherNo)
                .HasColumnName("Voucher_No")
                .HasMaxLength(20);

            entity.Property(e => e.BuyType).HasColumnName("Buy_Type");

            entity.Property(e => e.CreatedBy)
                .IsRequired()
                .HasColumnName("Created_By")
                .HasMaxLength(300);

            entity.Property(e => e.CreatedOn)
                .HasColumnName("Created_On")
                .HasColumnType("datetime");

            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.Property(e => e.ExpiryDate)
                .HasColumnName("Expiry_Date")
                .HasColumnType("datetime");

            entity.Property(e => e.GiftPerUserLimit).HasColumnName("Gift_Per_User_Limit");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.ImagePath)
                .HasColumnName("Image_Path")
                .HasMaxLength(100);

            entity.Property(e => e.LastUpdatedBy)
                .HasColumnName("Last_Updated_By")
                .HasColumnType("datetime");

            entity.Property(e => e.LastUpdatedOn)
                .HasColumnName("Last_Updated_On")
                .HasColumnType("datetime");

            entity.Property(e => e.MaxLimit).HasColumnName("Max_Limit");

            entity.Property(e => e.PaymentMethod)
                .HasColumnName("Payment_Method")
                .HasMaxLength(10);

            entity.Property(e => e.SellingDiscount).HasColumnName("Selling_Discount");

            entity.Property(e => e.SellingPrice)
                .HasColumnName("Selling_Price")
                .HasColumnType("money");

            entity.Property(e => e.Status).HasDefaultValueSql("('0')");

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.VoucherAmount)
                .HasColumnName("Voucher_Amount")
                .HasColumnType("money");

            entity.Property(e => e.Quantity)
                .HasColumnName("Quantity");

            entity.Property(e => e.Id)
                .HasColumnName("Id");
               
        }
    }
}

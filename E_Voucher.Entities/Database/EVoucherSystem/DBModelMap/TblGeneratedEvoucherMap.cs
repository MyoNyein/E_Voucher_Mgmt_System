using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace E_Voucher.Entities.Database.EVoucherSystem.DBModelMap
{
    public class TblGeneratedEvoucherMap : IEntityTypeConfiguration<TblGeneratedEvoucher>
    {
        public void Configure(EntityTypeBuilder<TblGeneratedEvoucher> entity)
        {
            entity.HasKey(e => e.PromoCode)
                   .HasName("PK_TBL_GENERATED_EVOUCHER");

            entity.ToTable("Tbl_Generated_EVoucher");

            entity.HasIndex(e => e.Id)
                .HasName("UQ__Tbl_Gene__3213E83E351C0AE2")
                .IsUnique();

            entity.HasIndex(e => e.PromoCode)
                .HasName("UQ__Tbl_Gene__BB785E2C9FC6502E")
                .IsUnique();

            entity.Property(e => e.PromoCode)
                .HasColumnName("Promo_Code")
                .HasMaxLength(11);

            entity.Property(e => e.ExpiryDate)
                .HasColumnName("Expiry_Date")
                .HasColumnType("datetime");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.OwnerName)
                .HasColumnName("Owner_Name")
                .HasMaxLength(300);

            entity.Property(e => e.OwnerPhone)
                .IsRequired()
                .HasColumnName("Owner_Phone")
                .HasMaxLength(50);

            entity.Property(e => e.PurchaseOrderNo)
                .IsRequired()
                .HasColumnName("PurchaseOrder_No")
                .HasMaxLength(20);

            entity.Property(e => e.QrImagePath)
                .IsRequired()
                .HasColumnName("QR_Image_Path")
                .HasMaxLength(100);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasDefaultValueSql("('0')");

            entity.Property(e => e.VoncherAmount)
                .HasColumnName("Voncher_Amount")
                .HasColumnType("money");

            entity.Property(e => e.VoucherImagePath)
                .IsRequired()
                .HasColumnName("Voucher_Image_Path")
                .HasMaxLength(100);

            entity.Property(e => e.VoucherNo)
                .IsRequired()
                .HasColumnName("Voucher_No")
                .HasMaxLength(20);
        }
    }
}

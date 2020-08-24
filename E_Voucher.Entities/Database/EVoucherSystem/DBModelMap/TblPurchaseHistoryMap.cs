using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace E_Voucher.Entities.Database.EVoucherSystem.DBModelMap
{
    public class TblPurchaseHistoryMap : IEntityTypeConfiguration<TblPurchaseHistory>
    {
        public void Configure(EntityTypeBuilder<TblPurchaseHistory> entity)
        {
            entity.HasKey(e => e.PurchaseHistoryId)
                    .HasName("PK_TBL_PURCHASE_HISTORY");

            entity.ToTable("Tbl_Purchase_History");

            entity.Property(e => e.PurchaseHistoryId).HasColumnName("Purchase_History_Id");

            entity.Property(e => e.PromoCode)
                .HasColumnName("Promo_Code")
                .HasMaxLength(11);

            entity.Property(e => e.PurchaseDate)
                .HasColumnName("Purchase_Date")
                .HasColumnType("datetime");

            entity.Property(e => e.PurchaseOrderNo)
                .IsRequired()
                .HasColumnName("PurchaseOrder_No")
                .HasMaxLength(20);

            entity.Property(e => e.VoucherNo)
                .IsRequired()
                .HasColumnName("Voucher_No")
                .HasMaxLength(20);
        }
    }
}

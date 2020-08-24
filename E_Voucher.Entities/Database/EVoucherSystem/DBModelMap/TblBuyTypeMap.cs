using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace E_Voucher.Entities.Database.EVoucherSystem.DBModelMap
{
    public class TblBuyTypeMap : IEntityTypeConfiguration<TblBuyType>
    {
        public void Configure(EntityTypeBuilder<TblBuyType> entity)
        {
            entity.ToTable("Tbl_Buy_Type");

            entity.Property(e => e.BuyType)
                .IsRequired()
                .HasColumnName("Buy_Type")
                .HasMaxLength(20);

            entity.Property(e => e.Description).HasMaxLength(100);

            entity.Property(e => e.MaxLimit).HasColumnName("Max_Limit");

            entity.Property(e => e.Status).HasDefaultValueSql("('0')");

        }
    }
}

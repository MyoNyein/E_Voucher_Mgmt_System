using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace E_Voucher.Entities.Database.EVoucherSystem.DBModelMap
{
    public class TblRefreshTokenMap : IEntityTypeConfiguration<TblRefreshToken>
    {
        public void Configure(EntityTypeBuilder<TblRefreshToken> entity)
        {
            entity.HasKey(e => e.RefreshToken)
                    .HasName("PK_TBL_REFRESH_TOKEN");

            entity.ToTable("Tbl_Refresh_Token");

            entity.Property(e => e.RefreshToken)
                .HasColumnName("Refresh_Token")
                .HasMaxLength(300);

            entity.Property(e => e.ExpiryDate)
                .HasColumnName("Expiry_Date")
                .HasColumnType("datetime");

            entity.Property(e => e.RefreshTokenId)
                .HasColumnName("RefreshToken_Id")
                .ValueGeneratedOnAdd(); ;

            entity.Property(e => e.UserId).HasColumnName("User_Id");
        }
    }
}

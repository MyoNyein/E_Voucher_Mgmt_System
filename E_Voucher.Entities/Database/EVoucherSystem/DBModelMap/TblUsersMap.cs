using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace E_Voucher.Entities.Database.EVoucherSystem.DBModelMap
{
    public class TblUsersMap : IEntityTypeConfiguration<TblUsers>
    {
        public void Configure(EntityTypeBuilder<TblUsers> entity)
        {
            entity.HasKey(e => e.UserId)
                    .HasName("PK_TBL_USERS");

            entity.ToTable("Tbl_Users");

            entity.Property(e => e.UserId)
                .HasColumnName("User_Id")
                .ValueGeneratedNever();

            entity.Property(e => e.CreatedOn)
                .HasColumnName("Created_On")
                .HasColumnType("datetime");

            entity.Property(e => e.DisplayName).HasMaxLength(1000);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasColumnName("First_Name")
                .HasMaxLength(300);

            entity.Property(e => e.LastName)
                .HasColumnName("Last_Name")
                .HasMaxLength(300);

            entity.Property(e => e.LastUpdatedOn)
                .HasColumnName("Last_Updated_On")
                .HasColumnType("datetime");

            entity.Property(e => e.LoginId)
                .IsRequired()
                .HasColumnName("Login_Id")
                .HasMaxLength(300);

            entity.Property(e => e.MiddleName)
                .HasColumnName("Middle_Name")
                .HasMaxLength(300);

            entity.Property(e => e.MultiSessionEnable)
                .IsRequired()
                .HasColumnName("Multi_Session_Enable")
                .HasDefaultValueSql("('0')");

            entity.Property(e => e.Status).HasDefaultValueSql("('0')");
        }
    }
}

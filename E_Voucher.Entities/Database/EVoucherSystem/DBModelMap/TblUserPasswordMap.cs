using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace E_Voucher.Entities.Database.EVoucherSystem.DBModelMap
{
    public class TblUserPasswordMap : IEntityTypeConfiguration<TblUserPassword>
    {
        public void Configure(EntityTypeBuilder<TblUserPassword> entity)
        {
            entity.HasKey(e => e.UserId)
                   .HasName("PK_TBL_USER_PASSWORD");

            entity.ToTable("Tbl_User_Password");

            entity.Property(e => e.UserId)
                .HasColumnName("User_Id")
                .ValueGeneratedNever();

            entity.Property(e => e.ActivePassword)
                .HasColumnName("Active_Password")
                .HasMaxLength(50);

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Password1)
                .HasColumnName("Password_1")
                .HasMaxLength(50);

            entity.Property(e => e.Password2)
                .HasColumnName("Password_2")
                .HasMaxLength(50);

            entity.Property(e => e.Password3)
                .HasColumnName("Password_3")
                .HasMaxLength(50);

            entity.Property(e => e.Status).HasDefaultValueSql("('0')");

            entity.HasOne(d => d.User)
                .WithOne(p => p.TblUserPassword)
                .HasForeignKey<TblUserPassword>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}

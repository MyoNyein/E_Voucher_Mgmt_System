using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using E_Voucher.Entities.Database.EVoucherSystem.DBModelMap;
using Microsoft.EntityFrameworkCore;

namespace E_Voucher.Entities.Database.EVoucherSystem
{
    public class EVoucher_System_DBContex : DbContext
    {
        public EVoucher_System_DBContex(DbContextOptions options)
           : base(options)
        {
        }

        public virtual DbSet<TblRefreshToken> TblRefreshToken { get; set; }
        public virtual DbSet<TblUserPassword> TblUserPassword { get; set; }
        public virtual DbSet<TblUsers> TblUsers { get; set; }
        public virtual DbSet<TblEvoucher> TblEvouchers { get; set; }
        public virtual DbSet<TblBuyType> TblBuyTypes { get; set; }
        public virtual DbSet<TblPaymentMethod> TblPaymentMethods { get; set; }
        public virtual DbSet<TblPurchaseOrder> TblPurchaseOrders { get; set; }
        public virtual DbSet<TblPurchaseHistory> TblPurchaseHistories { get; set; }
        public virtual DbSet<TblGeneratedEvoucher> TblGeneratedEvouchers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TblUsersMap());
            modelBuilder.ApplyConfiguration(new TblRefreshTokenMap());
            modelBuilder.ApplyConfiguration(new TblUserPasswordMap());
            modelBuilder.ApplyConfiguration(new TblEvoucherMap());
            modelBuilder.ApplyConfiguration(new TblBuyTypeMap());
            modelBuilder.ApplyConfiguration(new TblPaymentMethodMap());
            modelBuilder.ApplyConfiguration(new TblGeneratedEvoucherMap());
            modelBuilder.ApplyConfiguration(new TblPurchaseHistoryMap());
            modelBuilder.ApplyConfiguration(new TblPurchaseOrderMap());
        }
    }
}

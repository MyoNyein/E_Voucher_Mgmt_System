using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace E_Voucher.Entities.Database.EVoucherSystem.DBModelMap
{
    public class TemplateMap : IEntityTypeConfiguration<TblTemplate>
    {
        public void Configure(EntityTypeBuilder<TblTemplate> entity)
        {

        }
    }
}

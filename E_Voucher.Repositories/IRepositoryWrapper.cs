using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Repositories
{
    public interface IRepositoryWrapper
    {
        IRefreshTokenRepository RefreshToken { get; }
        IUserRepository User { get; }
        IEVoucherRepository EVoucher { get; }
        IPromoCodeRepository PromoCode { get; }
        IEStoreRepository Estore { get; }

    }
}

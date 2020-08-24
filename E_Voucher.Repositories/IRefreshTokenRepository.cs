using E_Voucher.Entities.DTO;
using E_Voucher.Entities.Request_Models;
using E_Voucher.Entities.Response_Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Repositories
{
    public interface IRefreshTokenRepository
    {
        public void SaveRefreshToken(SaveRefreshTokenDTO tokenDto);
        public RefreshTokenResponse RefreshToken(RefreshTokenRequest _request, string token);
    }
}

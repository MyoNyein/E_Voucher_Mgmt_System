using E_Voucher.Entities.Request_Models;
using E_Voucher.Entities.Response_Models;

namespace E_Voucher.Repositories
{
    public interface IUserRepository 
    {
        public LoginResponse Login(LoginRequest _request);
        public RefreshTokenResponse RefreshToken(RefreshTokenRequest _request,string token);
    }
}

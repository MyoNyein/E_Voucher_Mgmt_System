using E_Voucher.Entities.DTO;

namespace E_Voucher.Entities.Response_Models
{
    public class RefreshTokenResponse : ResponseBase
    {
        public string AccessToken { get; set; }
        public int AccessTokenExpireHours { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshTokenExpireHours { get; set; }
    }
}

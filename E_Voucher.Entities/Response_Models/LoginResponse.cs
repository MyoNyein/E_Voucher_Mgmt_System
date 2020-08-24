using E_Voucher.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.Response_Models
{
    public class LoginResponse 
    {
        public string AccessToken { get; set; }
        public int AccessTokenExpireMinutes { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshTokenExpireMinutes { get; set; }

        public string ErrorStatus;
        public int UserId;
    }
}

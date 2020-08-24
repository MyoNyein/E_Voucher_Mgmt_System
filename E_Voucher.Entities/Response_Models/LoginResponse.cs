using E_Voucher.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.Response_Models
{
    public class LoginResponse 
    {
        public string AccessToken { get; set; }
        public int AccessTokenExpireHours { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshTokenExpireHours { get; set; }

        public string ErrorStatus;
    }
}

using E_Voucher.Entities.Response_Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.DTO
{
    public class LoginResponseDTO : ResponseBase
    {
        public string AccessToken { get; set; }
        public int AccessTokenExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshTokenExpiresIn { get; set; }
    }
}

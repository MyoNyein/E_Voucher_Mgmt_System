using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.DTO
{
    public class CheckValidateTokenDTO
    {
        public string Token { get; set; }
        public bool IsValidateExpiry { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string PrivateKey { get; set; }
    }
}

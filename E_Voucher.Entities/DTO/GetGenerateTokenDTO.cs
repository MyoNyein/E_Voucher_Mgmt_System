using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace E_Voucher.Entities.DTO
{
    public class GetGenerateTokenDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PrivateKey { get; set; }
        public int TokenExpiryMinute { get; set; }
        public int RefreshTokenExpiryMinute { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}

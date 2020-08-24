using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace E_Voucher.Entities.Request_Models
{
    public class RefreshTokenRequest 
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}

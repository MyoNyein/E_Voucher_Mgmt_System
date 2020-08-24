using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace E_Voucher.Entities.Request_Models
{
    public class LoginRequest
    {
        [Required]
        public string LoginId { get; set; }
        [Required]
        public string Password { get; set; }

    }
}

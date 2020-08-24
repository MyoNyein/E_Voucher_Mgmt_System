using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace E_Voucher.Entities.Request_Models
{
    public class UpdateEVoucherStatusRequest
    {
        [Required]
        public string VoucherNo { get; set; }
        [Required]
        public short Status { get; set; }
    }
}

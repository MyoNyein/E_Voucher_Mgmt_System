using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace E_Voucher.Entities.Request_Models
{
    public class CheckStockAvaliableRequest
    {
        [Required]
        public string VoucherNo { get; set; }
    }
}

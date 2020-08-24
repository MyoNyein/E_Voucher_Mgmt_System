using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace E_Voucher.Entities.Request_Models
{
    public class BuyEVoucherRequest
    {
        [Required]
        public string VoucherNo { get; set; }
        [Required]
        public string BuyerName { get; set; }
        [Required]
        public string BuyerPhone { get; set; }
        [Required]
        public string BuyType { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        [Range(1, int.MaxValue,
        ErrorMessage = "Value for {0} must be greater than 0.")]
        public int Quantity { get; set; }
        
    }
}

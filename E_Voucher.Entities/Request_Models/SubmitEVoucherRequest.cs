using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace E_Voucher.Entities.Request_Models
{
    public class SubmitEVoucherRequest
    {
        public string VoucherNo { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        public DateTime ExpiryDate { get; set; }
        [Required]
        public string BuyType { get; set; }
        [Required]
        [Range(0.1, double.MaxValue,
        ErrorMessage = "Value for {0} must be greater than 0.")]
        public decimal VoucherAmount { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        [Range(0.1, double.MaxValue,
        ErrorMessage = "Value for {0} must be greater than 0.")]
        public decimal SellingPrice { get; set; }
        [Range(1, 100,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public short? SellingDiscount { get; set; }
        [Required]
        public int MaxLimit { get; set; }
        [Required]
        public int GiftPerUserLimit { get; set; }
        [Required]
        public short Status { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        [Range(0, double.MaxValue,
        ErrorMessage = "Value for {0} must be greater than {1}.")]
        public int Quantity { get; set; } 
    }
}

using E_Voucher.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.Response_Models
{
    public class GetEVoucherListingResponse 
    {
        public string VoucherNo { get; set; }
        public string Title { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal VoucherAmount { get; set; }
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
    }
}

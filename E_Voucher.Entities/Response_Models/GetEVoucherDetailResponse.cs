using E_Voucher.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.Response_Models
{
    public class GetEVoucherDetailResponse:ResponseBase
    {
        public string VoucherNo { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string BuyType { get; set; }
        public decimal VoucherAmount { get; set; }
        public string PaymentMethod { get; set; }
        public decimal SellingPrice { get; set; }
        public short? SellingDiscount { get; set; }
        public int Quantity { get; set; }
        public int MaxLimit { get; set; }
        public int GiftPerUserLimit { get; set; }
        public int Status { get; set; }
        public string Image { get; set; }
    }
}

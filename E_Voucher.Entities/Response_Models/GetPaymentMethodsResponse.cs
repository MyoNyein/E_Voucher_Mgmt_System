using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.Response_Models
{
    public class GetPaymentMethodListResponse
    {
        public string PaymentMethod { get; set;}
        public string Description { get; set; }
        public bool? HasDiscount { get; set; }
        public int DiscountPercentage { get; set; }
        public short Status { get; set; }
    }
}

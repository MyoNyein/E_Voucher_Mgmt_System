using E_Voucher.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.Response_Models
{
    public class BuyEVoucherResponse : ResponseBase
    {
        public string OrderNo { get; set; }
        public bool IsPurchaseSuccess { get; set; }
        public string ErrorResponse { get; set; }
        
    }
}

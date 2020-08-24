using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.Request_Models
{
    public class CheckPromoCodeRequest
    {
        public string PromoCode { get; set; }
        public string Phone { get; set; }
    }
}

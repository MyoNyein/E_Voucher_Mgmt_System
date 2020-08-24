using E_Voucher.Entities.DTO;
using E_Voucher.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.Response_Models
{
    public class CheckPromoCodeResponse : ResponseBase
    {
        public PromoCodeStatus Status { get; set; }
        public decimal PromoAmount { get; set; }
    }
}

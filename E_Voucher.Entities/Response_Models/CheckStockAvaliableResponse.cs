using E_Voucher.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.Response_Models
{
    public class CheckStockAvaliableResponse :ResponseBase
    {
        public bool isAvaliable { get; set; }
        public int RemainingQuantity { get; set; }
    }
}

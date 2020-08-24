using E_Voucher.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.Response_Models
{
    public class GetPurchaseHistoryResponse
    {
        public int PurchaseHistoryId { get; set; }
        public bool IsUsed { get; set; }
        public string PromoCode { get; set; }
        public string QR_Image_Path { get; set; }
    }
}

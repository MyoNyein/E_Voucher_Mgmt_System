using E_Voucher.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.Response_Models
{
    public class UpdateEVoucherStatusResponse : ResponseBase
    {
        public bool Updated { get; set; }
        public string VoucherNo { get; set; }
    }
}

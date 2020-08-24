using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace E_Voucher.Entities.Request_Models
{
    public class GetPurchaseHistoryRequest
    {
        public DateTime? PurchaseFromDate { get; set; }
        public DateTime? PurchaseToDate { get; set; }
        [Required]
        [Range(1,1000)]
        public int PageNumber { get; set; }
        [Required]
        [Range(1, 1000)]
        public int PageSize { get; set; }
    }
}

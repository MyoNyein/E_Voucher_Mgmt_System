using E_Voucher.Entities.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace E_Voucher.Entities.Request_Models
{
    public class GetEVoucherListingRequest
    {
        public short? Status { get; set; }
        [Required]
        [Range(1,1000)]
        public int PageNumber { get; set; }
        [Required]
        [Range(1, 1000)]
        public int PageSize { get; set; }
    }
}

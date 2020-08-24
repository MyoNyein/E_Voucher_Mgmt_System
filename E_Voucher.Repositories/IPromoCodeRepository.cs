using E_Voucher.Entities.Response_Models;
using E_Voucher.Entities.Request_Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Repositories
{
    public interface IPromoCodeRepository
    {
        public GeneratePromoCodeResponse GeneratePromoCode(GeneratePromoCodeRequest _request);
    }
}

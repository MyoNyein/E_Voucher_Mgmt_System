using E_Voucher.Entities.Request_Models;
using E_Voucher.Entities.Response_Models;
using E_Voucher.Repositories.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Repositories
{
    public interface IEStoreRepository
    {
        public EstoreAccessTokenResponse GetAccessToken(EstoreAccessTokenRequest _request);
        public PagedList<GetEVoucherListingResponse> GetStoreEvoucherList(GetEVoucherListingRequest _request);
        public GetEVoucherDetailResponse GetEvoucherDetail(GetEVoucherDetailRequest _request);
        public List<GetPaymentMethodListResponse> GetPaymentMethodList();
        public BuyEVoucherResponse BuyEVoucher(BuyEVoucherRequest _request);
        public CheckStockAvaliableResponse CheckStockAvaliable(CheckStockAvaliableRequest _request);
        public CheckPromoCodeResponse CheckPromoCode(CheckPromoCodeRequest _request);
        public PagedList<GetPurchaseHistoryResponse> GetPurchaseHistory(GetPurchaseHistoryRequest _request);
        public void ScheduleGeneratePromoCode(GeneratePromoCodeRequest _requestData);
        public void ScheduleUpdatePaymentHistory();
        public void MyJob();
    }
}

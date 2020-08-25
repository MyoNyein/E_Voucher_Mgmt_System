using E_Voucher.Entities.Database.EVoucherSystem;
using E_Voucher.Entities.Request_Models;
using E_Voucher.Entities.Response_Models;
using E_Voucher.Repositories.Helper;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using E_Voucher.Entities.Enum;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System;
using E_Voucher.Entities.Constant;
using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using E_Voucher.Entities.DTO;
using RestSharp;
using Microsoft.EntityFrameworkCore;

namespace E_Voucher.Repositories
{
    public class EStoreRepository : IEStoreRepository
    {
        private readonly EVoucher_System_DBContex db_Evoucher;
        private readonly IConfiguration configuration;

        public EstoreAccessTokenResponse GetAccessToken(EstoreAccessTokenRequest _request)
        {
            EstoreAccessTokenResponse response = new EstoreAccessTokenResponse();

            if (configuration["EStoreClientID"] == _request.ClientID)
            {
                GetGenerateTokenDTO generateTokenDto = new GetGenerateTokenDTO
                {
                    Audience = configuration["Audience"],
                    Issuer = configuration["Issuer"],
                    PrivateKey = configuration["PrivateKey"],
                    TokenExpiryMinute = Int32.Parse(configuration["TokenExpiryMinute"]),
                    RefreshTokenExpiryMinute = Int32.Parse(configuration["RefreshTokenExpiryMinute"]),
                    UserId = 0,
                    UserName = "EstoreClient"
                };
                TokenGeneratedDTO generatedToken = JWTHelper.GenerateToken(generateTokenDto);
                if (generatedToken != null && string.IsNullOrEmpty(generatedToken.ErrorStatus))
                {
                    response.AccessToken = generatedToken.AccessToken;
                    response.AccessTokenExpireMinutes = generatedToken.TokenExpiresMinute;
                    response.RefreshToken = generatedToken.RefreshToken;
                    response.RefreshTokenExpireMinutes = generatedToken.TokenExpiresMinute;
                }
                else
                {
                    response.StatusCode = 401;
                    response.ErrorType = "Unauthorized Request";
                    response.ErrorMessage = "Unable to generate Token.";
                }
            }
            else
            {
                response.StatusCode = 401;
                response.ErrorType = "Unauthorized Request";
                response.ErrorMessage = "Invalid Client ID.";
            }
            return response;
        }
        public EStoreRepository(EVoucher_System_DBContex _EVoucher_DBContex, IConfiguration _configuration)
        {
            db_Evoucher = _EVoucher_DBContex;
            configuration = _configuration;
        }
        public PagedList<GetEVoucherListingResponse> GetStoreEvoucherList(GetEVoucherListingRequest _request)
        {
            var evoucherList = (from e in db_Evoucher.TblEvouchers
                                where (_request.Status == null || e.Status == _request.Status)
                                && e.Status == (int)RecordStatus.Active && e.Quantity > 0
                                select new GetEVoucherListingResponse
                                {
                                    ExpiryDate = e.ExpiryDate,
                                    Quantity = e.Quantity ,
                                    Status = e.Status,
                                    //Image = Path.Combine(configuration["BaseURL"], e.ImagePath),
                                    SellingPrice = e.SellingPrice,
                                    Title = e.Title,
                                    VoucherAmount = e.VoucherAmount,
                                    VoucherNo = e.VoucherNo
                                }
                                ).AsQueryable();
            return PagedList<GetEVoucherListingResponse>.ToPagedList(evoucherList,
         _request.PageNumber,
         _request.PageSize);
        }
        public GetEVoucherDetailResponse GetEvoucherDetail(GetEVoucherDetailRequest _request)
        {
            GetEVoucherDetailResponse response = new GetEVoucherDetailResponse();
            response = (from e in db_Evoucher.TblEvouchers
                        where e.VoucherNo == _request.VoucherNo
                        select new GetEVoucherDetailResponse
                        {
                            BuyType = e.BuyType,
                            Description = e.Description,
                            ExpiryDate = e.ExpiryDate,
                            GiftPerUserLimit = e.GiftPerUserLimit,
                            Image = Path.Combine(configuration["BaseURL"], e.ImagePath),
                            MaxLimit = e.MaxLimit,
                            PaymentMethod = e.PaymentMethod,
                            Quantity = e.Quantity,
                            SellingDiscount = e.SellingDiscount,
                            SellingPrice = e.SellingPrice,
                            Status = e.Status,
                            Title = e.Title,
                            VoucherAmount = e.VoucherAmount,
                            VoucherNo = e.VoucherNo
                        }
                        ).FirstOrDefault();

            return response;
        }
        public List<GetPaymentMethodListResponse> GetPaymentMethodList()
        {
            List<GetPaymentMethodListResponse> response = new List<GetPaymentMethodListResponse>();
            response = (from p in db_Evoucher.TblPaymentMethods
                        where p.Status == (int)RecordStatus.Active
                        select new GetPaymentMethodListResponse
                        {
                            PaymentMethod = p.PaymentMethod,
                            Description = p.Description,
                            DiscountPercentage = p.DiscountPercentage,
                            HasDiscount = p.IsDiscount,
                            Status = p.Status
                        }

                       ).ToList();
            return response;
        }
        public CheckStockAvaliableResponse CheckStockAvaliable(CheckStockAvaliableRequest _request)
        {
            var response = (from p in db_Evoucher.TblEvouchers
                            where p.Status == (int)RecordStatus.Active
                            && p.Quantity > 0
                            && p.ExpiryDate > DateTime.Now
                            select new CheckStockAvaliableResponse
                            {
                                isAvaliable = true,
                                RemainingQuantity = p.Quantity
                            }
                        ).FirstOrDefault();

            if (response == null)
            {
                response = new CheckStockAvaliableResponse
                {
                    isAvaliable = false,
                    RemainingQuantity = 0
                };
            }
            return response;
        }
        public CheckPromoCodeResponse CheckPromoCode(CheckPromoCodeRequest _request)
        {
            var response = (from ge in db_Evoucher.TblGeneratedEvouchers
                            where ge.PromoCode == _request.PromoCode
                            && ge.ExpiryDate > DateTime.Now
                            && ge.OwnerPhone == _request.Phone
                            select new CheckPromoCodeResponse
                            {
                                Status = (PromoCodeStatus)ge.Status,
                                PromoAmount = ge.VoncherAmount
                            }
                        ).FirstOrDefault();
            if (response == null)
            {
                response = new CheckPromoCodeResponse
                {
                    Status = PromoCodeStatus.InValid,
                    PromoAmount = 0
                };
            }

            return response;
        }
        public PagedList<GetPurchaseHistoryResponse> GetPurchaseHistory(GetPurchaseHistoryRequest _request)
        {
            GetPurchaseHistoryResponse response = new GetPurchaseHistoryResponse();
            var eVoucherList = (from p in db_Evoucher.TblPurchaseHistories
                                join ge in db_Evoucher.TblGeneratedEvouchers
                                on p.PromoCode equals ge.PromoCode
                                where ge.Status >= (int)PromoCodeStatus.Used
                                && p.Status == (int)RecordStatus.Active
                                && _request.PurchaseFromDate == null ? true : p.PurchaseDate >= _request.PurchaseFromDate
                                && _request.PurchaseToDate == null ? true : p.PurchaseDate <= _request.PurchaseToDate
                                select new GetPurchaseHistoryResponse
                                {
                                    PromoCode = p.PromoCode,
                                    QR_Image_Path = ge.Status != (int)PromoCodeStatus.Used ? ge.QrImagePath : "",
                                    IsUsed = ge.Status == (int)PromoCodeStatus.Used,
                                    PurchaseHistoryId = p.PurchaseHistoryId,
                                }
                                ).AsQueryable();
            return PagedList<GetPurchaseHistoryResponse>.ToPagedList(eVoucherList,
        _request.PageNumber,
        _request.PageSize);
        }
        public BuyEVoucherResponse BuyEVoucher(BuyEVoucherRequest _request)
        {
            BuyEVoucherResponse response = new BuyEVoucherResponse();
            string validateMsg = "";
            validateMsg = ValidateBuyEVoucher(_request);
            if (string.IsNullOrEmpty(validateMsg))
            {
                using (var dbContextTransaction = db_Evoucher.Database.BeginTransaction())
                {
                    Thread.Sleep(60000);

                    try
                    {
                        var tblEvoucher = (from v in db_Evoucher.TblEvouchers
                                           where v.VoucherNo == _request.VoucherNo
                                           select v
                                   ).FirstOrDefault();
                        if (tblEvoucher == null)
                        {
                            validateMsg = "No Voucher Found";
                        }
                        else
                        {
                            if (tblEvoucher.ExpiryDate < DateTime.Now && tblEvoucher.Status != (int)RecordStatus.Active)
                            {
                                validateMsg = $"{validateMsg}\r\nVoucher has been expired or out of stock.";
                            }
                            else if (tblEvoucher.Quantity <= 0)
                            {
                                validateMsg = $"{validateMsg}\r\nOut of stock.";
                            }
                            else if (tblEvoucher.Quantity < _request.Quantity)
                            {
                                validateMsg = $"{validateMsg}\r\nOrder quantity exceed the avaliable stock.";
                            }
                            else
                            {
                                var previousOrderList = (from p in db_Evoucher.TblPurchaseOrders
                                                         where p.VoucherNo == _request.VoucherNo
                                                         && p.BuyerPhone == _request.BuyerPhone
                                                         select new
                                                         {
                                                             p.BuyType,
                                                             p.Quantity
                                                         }
                                                       ).ToList();

                                if (previousOrderList == null || previousOrderList.Count <= 0)
                                {
                                    if (_request.BuyType == Constant.EVOUCHER_BUY_TYPE_ONLYME
                                        && _request.Quantity > tblEvoucher.MaxLimit
                                        )
                                    {
                                        validateMsg = $"{validateMsg}\r\nReach Limitted Quantity,You can't buy anymore.";
                                    }
                                    else if (_request.Quantity > tblEvoucher.GiftPerUserLimit)
                                    {
                                        validateMsg = $"{validateMsg}\r\nReach Limitted Gift Quantity,You can't buy anymore.";
                                    }
                                }
                                else
                                {
                                    var buyGroup = previousOrderList.GroupBy(x => x.BuyType)
                                                            .Select(x => new
                                                            {
                                                                BuyType = x.First().BuyType,
                                                                Quantity = x.Sum(x => x.Quantity)
                                                            }).ToList();
                                    var OwnUsageQuantity = buyGroup.Where(x => x.BuyType == Constant.EVOUCHER_BUY_TYPE_ONLYME).Select(x => x.Quantity).FirstOrDefault();
                                    var GiftUsageQuantity = buyGroup.Where(x => x.BuyType == Constant.EVOUCHER_BUY_TYPE_GIFT).Select(x => x.Quantity).FirstOrDefault();
                                    var totalUsage = OwnUsageQuantity + GiftUsageQuantity;

                                    if (_request.Quantity + totalUsage > tblEvoucher.MaxLimit)
                                    {
                                        if (totalUsage > tblEvoucher.MaxLimit)
                                            validateMsg = $"{validateMsg}\r\nReach Limitted Quantity,You can buy anymore.";
                                        else
                                            validateMsg = $"{validateMsg}\r\nReach Limitted Quantity,You can buy only {tblEvoucher.MaxLimit - totalUsage} voucher.";

                                    }
                                    else if (_request.BuyType == Constant.EVOUCHER_BUY_TYPE_ONLYME
                                       && _request.Quantity + OwnUsageQuantity > tblEvoucher.MaxLimit
                                       )
                                    {
                                        if (OwnUsageQuantity > tblEvoucher.MaxLimit)
                                            validateMsg = $"{validateMsg}\r\nOwn Usage Reach Limitted Quantity,You can't buy anymore.";
                                        else
                                            validateMsg = $"{validateMsg}\r\nOwn Usage Reach Limitted Quantity,You can buy only {tblEvoucher.MaxLimit - OwnUsageQuantity} voucher.";
                                    }
                                    else if (_request.Quantity + GiftUsageQuantity > tblEvoucher.GiftPerUserLimit)
                                    {
                                        if (GiftUsageQuantity > tblEvoucher.GiftPerUserLimit)
                                            validateMsg = $"{validateMsg}\r\nGift Usage Reach Limitted Quantity,You can't buy anymore.";
                                        else
                                            validateMsg = $"{validateMsg}\r\nGift Usage Reach Limitted Quantity,You can buy only {tblEvoucher.MaxLimit - GiftUsageQuantity} voucher.";
                                    }

                                }

                                if (validateMsg == "")
                                {
                                    var UpdatetblEvoucher = (from v in db_Evoucher.TblEvouchers
                                                             where v.VoucherNo == _request.VoucherNo
                                                             select v
                                                                ).FirstOrDefault();
                                    UpdatetblEvoucher.Quantity = UpdatetblEvoucher.Quantity - _request.Quantity;
                                    decimal totalPrice = UpdatetblEvoucher.SellingPrice;
                                    short sellingDiscount;
                                    if (_request.PaymentMethod == UpdatetblEvoucher.PaymentMethod
                                       && UpdatetblEvoucher.SellingDiscount != null)
                                    {
                                        var discountAmount = totalPrice * (decimal)((UpdatetblEvoucher.SellingDiscount ?? 0) / 100.0);
                                        totalPrice = totalPrice - discountAmount;
                                        if (totalPrice < 0)
                                            totalPrice = 0;
                                        sellingDiscount = UpdatetblEvoucher.SellingDiscount ?? 0;
                                    }
                                    else
                                    {
                                        sellingDiscount = 0;
                                    }

                                    var pOrderList = (from v in db_Evoucher.TblPurchaseOrders
                                                      select new
                                                      {
                                                          v.Id
                                                      }
                                     ).ToList();

                                    int maxNo = 1;
                                    if (pOrderList != null && pOrderList.Count > 0)
                                    {
                                        maxNo = pOrderList.Max(x => x.Id);
                                        maxNo++;
                                    }

                                    TblPurchaseOrder order = new TblPurchaseOrder
                                    {
                                        PurchaseOrderNo = "PO-" + maxNo.ToString().PadLeft(6, '0'),
                                        BuyerName = _request.BuyerName,
                                        BuyerPhone = _request.BuyerPhone,
                                        BuyType = _request.BuyType,
                                        OrderDate = DateTime.Now,
                                        PaymentMethod = _request.PaymentMethod,
                                        SellingDiscount = sellingDiscount,
                                        Quantity = _request.Quantity,
                                        Status = (int)RecordStatus.Active,
                                        TotalSellingAmount = totalPrice,
                                        SellingPrice = UpdatetblEvoucher.SellingPrice,
                                        ExpiryDate = UpdatetblEvoucher.ExpiryDate,
                                        ImagePath = UpdatetblEvoucher.ImagePath,
                                        VoncherAmount = UpdatetblEvoucher.VoucherAmount,
                                        VoucherNo = UpdatetblEvoucher.VoucherNo,
                                        VoucherGenerated = false,
                                    };

                                    db_Evoucher.TblPurchaseOrders.Add(order);
                                    db_Evoucher.SaveChanges();

                                    dbContextTransaction.Commit();
                                    response.OrderNo = order.PurchaseOrderNo;
                                    response.IsPurchaseSuccess = true;
                                }

                            }
                        }
                    }catch(Exception e)
                    {
                        response.StatusCode = 500;
                        response.ErrorType = "validation-error";
                        response.ErrorMessage = e.Message;

                        dbContextTransaction.Rollback();
                    }
                }
            }
            else
            {
                response.StatusCode = 400;
                response.ErrorType = "validation-error";
                response.ErrorMessage = validateMsg;
            }
            if (validateMsg != "")
            {
                response.StatusCode = 400;
                response.ErrorType = "validation-error";
                response.ErrorMessage = validateMsg;
            }

            return response;
        }
        public string ValidateBuyEVoucher(BuyEVoucherRequest _request)
        {
            string validationMsg = "";

            var isValidPaymentMethod = (from p in db_Evoucher.TblPaymentMethods
                                        where p.PaymentMethod == _request.PaymentMethod
                                        select true).FirstOrDefault();
            if (!isValidPaymentMethod)
            {
                validationMsg = "Invalid Payment Method.";
            }
            //var isValidBuyType = (from b in db_Evoucher.TblBuyTypes
            //                      where b.BuyType == _request.BuyType
            //                      select true).FirstOrDefault();
            //if (!isValidBuyType)
            //    validationMsg = $"{validationMsg} /r/n Invalid Buy Type.";

            return validationMsg;
        }

        public void ScheduleGeneratePromoCode(GeneratePromoCodeRequest _requestData)
        {
            var serviceURL = configuration["PromoCodeServiceURL"];
            var client = new RestClient(serviceURL);
            var request = new RestRequest(serviceURL + "/v1/PromoService/GeneratePromoCode");
            request.Method = Method.POST;
            request.AddJsonBody(_requestData);
            var resp = client.Execute(request);

        }

        public void ScheduleUpdatePaymentHistory()
        {
            var noHistoryOrder = (from o in db_Evoucher.TblPurchaseOrders
                                  join gp in db_Evoucher.TblGeneratedEvouchers
                                  on o.PurchaseOrderNo equals gp.PurchaseOrderNo
                                  join h in db_Evoucher.TblPurchaseHistories
                                  on o.PurchaseOrderNo equals h.PurchaseOrderNo
                                  into lh
                                  from jlh in lh.DefaultIfEmpty()
                                  where jlh == null && o.Status == (int)RecordStatus.Active
                                  select new TblPurchaseHistory
                                  {
                                      PurchaseOrderNo = gp.PurchaseOrderNo,
                                      PromoCode = gp.PromoCode,
                                      VoucherNo = gp.VoucherNo,
                                      Status = gp.Status,
                                      PurchaseDate = o.OrderDate,

                                  }).ToList();

            if (noHistoryOrder != null && noHistoryOrder.Count > 0)
            {
                db_Evoucher.TblPurchaseHistories.AddRange(noHistoryOrder);
                db_Evoucher.SaveChanges();
            }
        }

        public void MyJob()
        {
            Thread.Sleep(20000);
        }
    }
}

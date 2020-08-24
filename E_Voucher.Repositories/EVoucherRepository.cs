using E_Voucher.Entities;
using E_Voucher.Entities.Constant;
using E_Voucher.Entities.Database.EVoucherSystem;
using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using E_Voucher.Entities.DTO;
using E_Voucher.Entities.Enum;
using E_Voucher.Entities.Request_Models;
using E_Voucher.Entities.Response_Models;
using E_Voucher.Repositories.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace E_Voucher.Repositories
{
    public class EVoucherRepository : IEVoucherRepository
    {
        private readonly EVoucher_System_DBContex db_Evoucher;
        private readonly IConfiguration configuration;

        public EVoucherRepository(EVoucher_System_DBContex _EVoucher_DBContex, IConfiguration _configuration)
        {
            db_Evoucher = _EVoucher_DBContex;
            configuration = _configuration;
        }

        public SubmitEVoucherResponse SubmitEVoucher(SubmitEVoucherRequest _request)
        {
            SubmitEVoucherResponse response = new SubmitEVoucherResponse();
            string validateMsg = ValidateSubmitEVoucher(_request);

            if(string.IsNullOrEmpty(validateMsg))
            {
                var savedImage = ImageHelper.SaveBase64AsFile(_request.Image);
                if (!string.IsNullOrEmpty(savedImage.ErrorStatus) || string.IsNullOrEmpty(savedImage.SaveFilePath))
                {
                    response.StatusCode = 500;
                    response.ErrorType = "filesave-error";
                    response.ErrorMessage = savedImage.ErrorStatus;
                    return response;
                }

                if (string.IsNullOrEmpty(_request.VoucherNo))
                {
                    //New
                    var newResponse = NewEVoucher(_request,savedImage);
                    return newResponse;
                    
                }
                else
                {
                    //Update
                    var updateResponse = UpdateEVoucher(_request,savedImage);
                    return updateResponse;
                }
               
            }
            else
            {
                response.StatusCode = 400;
                response.ErrorType = "validation-error";
                response.ErrorMessage = validateMsg;
            }

            return response;
        }

        public SubmitEVoucherResponse NewEVoucher(SubmitEVoucherRequest _request,SaveImageDTO savedImage)
        {
            SubmitEVoucherResponse response = new SubmitEVoucherResponse();
            var vList = (from v in db_Evoucher.TblEvouchers
                         select new
                         {
                             v.Id
                         }
                             ).ToList();

            int maxNo = 1;
            if (vList != null && vList.Count > 0)
            {
                maxNo = vList.Max(x => x.Id);
                maxNo++;
            }

            TblEvoucher evoucher = new TblEvoucher
            {
                BuyType = _request.BuyType,
                CreatedBy = LoginInformation.UserName,
                CreatedOn = DateTime.Now,
                Description = _request.Description,
                ExpiryDate = _request.ExpiryDate,
                GiftPerUserLimit = _request.GiftPerUserLimit,
                MaxLimit = _request.MaxLimit,
                PaymentMethod = _request.PaymentMethod,
                SellingDiscount = _request.SellingDiscount ?? 0,
                SellingPrice = _request.SellingPrice,
                Status = _request.Status,
                VoucherAmount = _request.VoucherAmount,
                Title = _request.Title,
                VoucherNo = "EV-" + maxNo.ToString().PadLeft(4, '0'),
                ImagePath = savedImage.SaveFilePath,
                Quantity = _request.Quantity
            };

            db_Evoucher.TblEvouchers.Add(evoucher);
            db_Evoucher.SaveChanges();
            response.EVoucherNo = evoucher.VoucherNo;
            return response;
        }

        public SubmitEVoucherResponse UpdateEVoucher(SubmitEVoucherRequest _request,SaveImageDTO savedImage)
        {
            SubmitEVoucherResponse response = new SubmitEVoucherResponse();
            var tblEvoucher = (from v in db_Evoucher.TblEvouchers
                               where v.VoucherNo == _request.VoucherNo
                               select v
                               ).FirstOrDefault();
            if (tblEvoucher == null)
            {
                response.StatusCode = 404;
                response.ErrorType = "Record-Not Found";
                response.ErrorMessage = "No Voucher Found.";
                return response;
            }

            tblEvoucher.BuyType = _request.BuyType;
            tblEvoucher.CreatedBy = LoginInformation.UserName;
            tblEvoucher.CreatedOn = DateTime.Now;
            tblEvoucher.Description = _request.Description;
            tblEvoucher.ExpiryDate = _request.ExpiryDate;
            tblEvoucher.GiftPerUserLimit = _request.GiftPerUserLimit;
            tblEvoucher.MaxLimit = _request.MaxLimit;
            tblEvoucher.PaymentMethod = _request.PaymentMethod;
            tblEvoucher.SellingDiscount = _request.SellingDiscount ?? 0;
            tblEvoucher.SellingPrice = _request.SellingPrice;
            tblEvoucher.Status = _request.Status;
            tblEvoucher.VoucherAmount = _request.VoucherAmount;
            tblEvoucher.Title = _request.Title;
            tblEvoucher.ImagePath = savedImage.SaveFilePath;
            tblEvoucher.Quantity = _request.Quantity;

            db_Evoucher.SaveChanges();
            response.EVoucherNo = tblEvoucher.VoucherNo;

            return response;
        }

        public UpdateEVoucherStatusResponse UpdateEVoucherStatus(UpdateEVoucherStatusRequest _request)
        {
            UpdateEVoucherStatusResponse response = new UpdateEVoucherStatusResponse();
            var tblEvoucher = (from v in db_Evoucher.TblEvouchers
                               where v.VoucherNo == _request.VoucherNo
                               select v
                               ).FirstOrDefault();
            if (tblEvoucher == null)
            {
                response.StatusCode = 404;
                response.ErrorType = "Record-Not Found";
                response.ErrorMessage = "No Voucher Found.";
                return response;
            }
            tblEvoucher.Status = _request.Status;

            db_Evoucher.SaveChanges();

            response.Updated = true;
            response.VoucherNo = tblEvoucher.VoucherNo;

            return response;
        }
        private string ValidateSubmitEVoucher(SubmitEVoucherRequest _request)
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


            if (!ImageHelper.IsBase64(_request.Image))
            {
                validationMsg = $"{validationMsg} /r/n Invalid Image String.";
            }
            return validationMsg;
        }

        public PagedList<GetEVoucherListingResponse> GetEvoucherList(GetEVoucherListingRequest _request)
        {
            var evoucherList = (from e in db_Evoucher.TblEvouchers
                                where (_request.Status == null || e.Status == _request.Status)
                                select new GetEVoucherListingResponse
                                {
                                    ExpiryDate = e.ExpiryDate,
                                    Image = Path.Combine(configuration["BaseURL"], e.ImagePath),
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

        public PagedList<GetEVoucherListingResponse> GetStoreEvoucherList(GetEVoucherListingRequest _request)
        {
            var evoucherList = (from e in db_Evoucher.TblEvouchers
                                where (_request.Status == null || e.Status == _request.Status)
                                && e.Status == (int)RecordStatus.Active && e.Quantity > 0
                                select new GetEVoucherListingResponse
                                {
                                    ExpiryDate = e.ExpiryDate,
                                    Image = Path.Combine(configuration["BaseURL"], e.ImagePath),
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
                        && p.Quantity>0
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
                    isAvaliable= false,
                    RemainingQuantity=0
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
                    PromoAmount=0
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
                                where ge.Status>=(int)PromoCodeStatus.Used
                                && p.Status==(int)RecordStatus.Active
                                && _request.PurchaseFromDate==null?true:p.PurchaseDate>=_request.PurchaseFromDate
                                && _request.PurchaseToDate==null?true:p.PurchaseDate<=_request.PurchaseToDate
                                select new GetPurchaseHistoryResponse
                                {
                                    PromoCode = p.PromoCode,
                                    QR_Image_Path = ge.Status != (int)PromoCodeStatus.Used?ge.QrImagePath:"",
                                    IsUsed = ge.Status==(int)PromoCodeStatus.Used,
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
                                if(totalUsage>tblEvoucher.MaxLimit)
                                    validateMsg = $"{validateMsg}\r\nReach Limitted Quantity,You can buy anymore.";
                                else
                                    validateMsg = $"{validateMsg}\r\nReach Limitted Quantity,You can buy only {tblEvoucher.MaxLimit-totalUsage} voucher.";

                            } else if (_request.BuyType == Constant.EVOUCHER_BUY_TYPE_ONLYME
                                 && _request.Quantity + OwnUsageQuantity > tblEvoucher.MaxLimit
                                 )
                            {
                                if(OwnUsageQuantity > tblEvoucher.MaxLimit)
                                    validateMsg = $"{validateMsg}\r\nOwn Usage Reach Limitted Quantity,You can't buy anymore.";
                                else
                                    validateMsg = $"{validateMsg}\r\nOwn Usage Reach Limitted Quantity,You can buy only {tblEvoucher.MaxLimit-OwnUsageQuantity} voucher.";
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
                                PurchaseOrderNo = "PO-"+maxNo.ToString().PadLeft(6,'0'),
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
                            response.OrderNo = order.PurchaseOrderNo;
                            response.IsPurchaseSuccess = true;
                        }
                        
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
    }
}

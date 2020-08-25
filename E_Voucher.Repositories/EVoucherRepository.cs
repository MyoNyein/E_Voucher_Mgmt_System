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
using System.Threading;

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
            _request.BuyType = "";
            if (_request.Image != "")
            {
                var rawBase64 = _request.Image.Split(',');
                if (rawBase64.Length >= 2)
                    _request.Image = rawBase64[1];
            }
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
       
        public PagedList<GetEVoucherListingResponse> GetEvoucherList(GetEVoucherListingRequest _request)
        {
            var evoucherList = (from e in db_Evoucher.TblEvouchers
                                where (_request.Status == null || e.Status == _request.Status)
                                select new GetEVoucherListingResponse
                                {
                                    id = e.Id,
                                    ExpiryDate = e.ExpiryDate,
                                    Quantity = e.Quantity,
                                    //Image = Path.Combine(configuration["BaseURL"], e.ImagePath),
                                    Status = e.Status,
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

            //if(_request.Image)
            if (!ImageHelper.IsBase64(_request.Image))
            {
                validationMsg = $"{validationMsg} /r/n Invalid Image String.";
            }
            return validationMsg;
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

    }
}

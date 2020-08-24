using E_Voucher.Entities.Database.EVoucherSystem;
using E_Voucher.Entities.Request_Models;
using E_Voucher.Entities.Response_Models;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Linq;
using E_Voucher.Repositories.Helper;
using System;
using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using E_Voucher.Entities.Enum;

namespace E_Voucher.Repositories
{
    public class PromoCodeRepository :IPromoCodeRepository
    {
        private readonly EVoucher_System_DBContex db_Evoucher;
        private readonly IConfiguration configuration;

        public PromoCodeRepository(EVoucher_System_DBContex _EVoucher_DBContex, IConfiguration _configuration)
        {
            db_Evoucher = _EVoucher_DBContex;
            configuration = _configuration;
        }

        public GeneratePromoCodeResponse GeneratePromoCode(GeneratePromoCodeRequest _request)
        {
            GeneratePromoCodeResponse response = new GeneratePromoCodeResponse();
            var order = (from o in db_Evoucher.TblPurchaseOrders
                             where o.PurchaseOrderNo == _request.PurchaseOrder_No
                             && o.VoucherGenerated==false
                             select o
                             ).FirstOrDefault();
            if(order!=null)
            {
                for (int i = 0; i < order.Quantity; i++)
                {
                    bool isUnique = false;
                    string promoCode = StringHelper.GeneatePromo();
                    string qrCodePath;
                    do
                    {
                        isUnique = !(from gp in db_Evoucher.TblGeneratedEvouchers
                                     where gp.PromoCode == promoCode
                                     select true
                                   ).FirstOrDefault();
                    } while (!isUnique);

                    qrCodePath = QRCodeHelper.GenerateQRCode(promoCode);

                    if (qrCodePath != "")
                    {
                        TblGeneratedEvoucher generatedEvoucher = new TblGeneratedEvoucher
                        {
                            ExpiryDate = order.ExpiryDate,
                            OwnerName = order.BuyerName,
                            OwnerPhone = order.BuyerPhone,
                            PromoCode = promoCode,
                            PurchaseOrderNo = order.PurchaseOrderNo,
                            QrImagePath = qrCodePath,
                            Status = (int)RecordStatus.Active,
                            VoncherAmount = order.VoncherAmount,
                            VoucherImagePath = order.ImagePath,
                            VoucherNo = order.VoucherNo
                        };
                        
                        db_Evoucher.TblGeneratedEvouchers.Add(generatedEvoucher);

                    }
                }
                order.VoucherGenerated = true;

            }
            else
            {
                response.StatusCode = 404;
                response.ErrorType = "Not-Found";
                response.ErrorMessage = "Record Not Found";
            }
            db_Evoucher.SaveChanges();
            response.PromoCodeGenerated = true;
            
            return response;
        }
    }
}

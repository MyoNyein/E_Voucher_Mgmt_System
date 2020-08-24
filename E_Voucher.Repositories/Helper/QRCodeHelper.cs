using QRCoder;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System;
using System.Drawing.Imaging;

namespace E_Voucher.Repositories.Helper
{
    public static class QRCodeHelper
    {
        public static string GenerateQRCode(string contentString)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(contentString, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            string saveFilePath = "";

            using(var qrCodeImage = qrCode.GetGraphic(20))
            {
                var fileName = Guid.NewGuid().ToString() + ".jpg";
                var filePath = FileHelper.GetFullFilePath("Images", fileName);
                saveFilePath = Path.Combine("Images", fileName);
                qrCodeImage.Save(filePath, ImageFormat.Png);
            }
            return saveFilePath;
        }
       
    }
}

using E_Voucher.Entities.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace E_Voucher.Repositories.Helper
{
    public static class ImageHelper
    {
        public static SaveImageDTO SaveBase64AsFile(string base64str)
        {
            SaveImageDTO saveDto = new SaveImageDTO();
            var fileName = Guid.NewGuid().ToString() + ".jpg";
            var filePath = FileHelper.GetFullFilePath("Images",fileName);
            var bytes = Convert.FromBase64String(base64str);

            
            if (base64str.Length > 0 && filePath!="")
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
                saveDto.SaveFilePath = Path.Combine("Images", fileName);
            }
            else
            {
                saveDto.ErrorStatus = "Failed to save:base64 string is empty or file path not found.";
            }
            return saveDto;
        }

        public static bool IsBase64(this string base64String)
        {
        
        if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
        || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
                return false;
            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch 
            {
                
            }
            return false;
        }

    }
}

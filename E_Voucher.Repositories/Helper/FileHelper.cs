using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace E_Voucher.Repositories.Helper
{
    public static class FileHelper
    {
        public static string GetRootPath()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images");
            return filePath;
        }

        public static string GetFullFilePath(string folder,string fileName)
        {
            if (string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(fileName))
                return "";

            var saveFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\",folder);
            var fullFilePath = Path.Combine(saveFilePath, fileName);

            if (!Directory.Exists(saveFilePath))
            {
                Directory.CreateDirectory(saveFilePath);
            }

            return fullFilePath;
        }

        public static string DeleteOldFile(string folder,string fileName)
        {
            var filePath = GetFullFilePath(folder, fileName);
            File.Delete(filePath);
            return "Ok";
        }
    }
}

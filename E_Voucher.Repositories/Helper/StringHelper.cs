using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MiniGuids;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace E_Voucher.Repositories.Helper
{
    public static class StringHelper
    {
        public static string GenerateHash(string plainText)
        {
            byte[] salt = new byte[128 / 8];

            string hashedString = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: plainText,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

            return hashedString;
        }

        public static string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string GeneatePromo()
        {
            var miniGuid = MiniGuid.NewGuid();

            //implicit conversions (and vice-versa too)
            string someString = miniGuid;
            Guid someGuid = miniGuid;

            //explicit stringifying and parsing
            var str = miniGuid.ToString();
            var sameMiniGuid = str.Substring(0, 5);
            var intGuid = codeFromCoupon(str);
            var PromoCode = sameMiniGuid + intGuid.ToString().Substring(0, 6);
            Random num = new Random();

            PromoCode = new string(PromoCode.ToCharArray().
                            OrderBy(s => (num.Next(2) % 2) == 0).ToArray());
            return PromoCode;
        }

        const string ALPHABET = "A1G4FO6LEWV3TC7P8Y3ZHNI7UDBX8SM0QKzawemckdjeufysoeui";
        public static uint codeFromCoupon(string coupon)
        {
            uint n = 0;
            for (int i = 0; i < 6; ++i)
                n = n | (((uint)ALPHABET.IndexOf(coupon[i])) << (5 * i));
            return n;
        }
        

    }
}

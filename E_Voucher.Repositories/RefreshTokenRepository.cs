using E_Voucher.Entities.Database.EVoucherSystem;
using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using E_Voucher.Entities.DTO;
using E_Voucher.Entities.Request_Models;
using E_Voucher.Entities.Response_Models;
using E_Voucher.Repositories.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace E_Voucher.Repositories
{
    public class RefreshTokenRepository: IRefreshTokenRepository
    {
        private readonly EVoucher_System_DBContex db_Evoucher;
        private readonly IConfiguration configuration;

        public RefreshTokenRepository(EVoucher_System_DBContex _EVoucher_DBContex, IConfiguration _configuration)
        {
            db_Evoucher = _EVoucher_DBContex;
            configuration = _configuration;
        }

        public void DeleteExpiryRefreshToken()
        {
            var deleteLoginToken = (from rt in db_Evoucher.TblRefreshToken
                                    where rt.ExpiryDate < DateTime.Now
                                    select rt).ToList();
            foreach (var item in deleteLoginToken)
            {
                db_Evoucher.TblRefreshToken.Remove(item);
            }
        }

        public void DeleteRefreshToken(string refreshToken)
        {
            var deleteLoginToken = (from rt in db_Evoucher.TblRefreshToken
                                    where rt.RefreshToken == refreshToken
                                    select rt).FirstOrDefault();
            db_Evoucher.TblRefreshToken.Remove(deleteLoginToken);
        }

        public void SaveRefreshToken(SaveRefreshTokenDTO tokenDto)
        {
            TblRefreshToken tblRefreshToken = new TblRefreshToken
            {
                ExpiryDate = DateTime.Now.AddMinutes(tokenDto.ExpiryMinute),
                RefreshToken = tokenDto.RefreshToken,
                UserId = tokenDto.UserId
            };
            db_Evoucher.TblRefreshToken.Add(tblRefreshToken);
            DeleteExpiryRefreshToken();
            db_Evoucher.SaveChanges();
        }

        public RefreshTokenResponse RefreshToken(RefreshTokenRequest _request, string token)
        {
            RefreshTokenResponse response = new RefreshTokenResponse();
            CheckValidateTokenDTO validateDto = new CheckValidateTokenDTO
            {
                Audience = configuration["Audience"],
                Issuer = configuration["Issuer"],
                PrivateKey = configuration["PrivateKey"],
                IsValidateExpiry = false,
                Token = token
            };

            var validatedToken = JWTHelper.CheckValidToken(validateDto);
            if (validatedToken.IsValid)
            {
                var tblRefreshToken = (from rt in db_Evoucher.TblRefreshToken
                                       where rt.RefreshToken == _request.RefreshToken
                                       && rt.UserId == validatedToken.UserID
                                       && rt.ExpiryDate > DateTime.Now
                                       select rt).FirstOrDefault();
                if (tblRefreshToken != null && tblRefreshToken.RefreshToken != "")
                {
                    GetGenerateTokenDTO getGenerateToken = new GetGenerateTokenDTO
                    {
                        Audience = configuration["Audience"],
                        Issuer = configuration["Issuer"],
                        PrivateKey = configuration["PrivateKey"],
                        TokenExpiryMinute = Int32.Parse(configuration["TokenExpiryMinute"]),
                        RefreshTokenExpiryMinute = Int32.Parse(configuration["RefreshTokenExpiryMinute"]),
                        UserId = validatedToken.UserID,
                        UserName = validatedToken.UserName
                    };

                    var generatedToken = JWTHelper.GenerateToken(getGenerateToken);
                    if (generatedToken != null && string.IsNullOrEmpty(generatedToken.ErrorStatus))
                    {
                        response.AccessToken = generatedToken.AccessToken;
                        response.AccessTokenExpireMinutes = generatedToken.TokenExpiresMinute;
                        response.RefreshToken = generatedToken.RefreshToken;
                        response.RefreshTokenExpireMinutes = Int32.Parse(configuration["RefreshTokenExpiryMinute"]);
                        SaveRefreshToken(new SaveRefreshTokenDTO 
                        {
                            ExpiryMinute = generatedToken.RefreshTokenExpiresMinute,
                            RefreshToken = generatedToken.RefreshToken,
                            UserId = generatedToken.UserId
                        });
                        DeleteRefreshToken(_request.RefreshToken);
                    }
                    else
                    {
                        response.StatusCode = 500;
                        response.ErrorType = "Token-Generation Fail.";
                        response.ErrorMessage = "Unable to generate Access Token.";
                    }



                    DeleteExpiryRefreshToken();

                    db_Evoucher.SaveChanges();
                }
                else
                {
                    response.StatusCode = 401;
                    response.ErrorType = "Unauthorized Request";
                    response.ErrorMessage = "Invalid or Expired Refresh Token.";
                }

            }
            else
            {
                response.StatusCode = 401;
                response.ErrorType = "Unauthorized Request";
                response.ErrorMessage = "Invalid or Expired Access Token.";
            }

            return response;
        }

    }
}

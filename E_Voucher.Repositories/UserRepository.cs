using E_Voucher.Entities.Database.EVoucherSystem;
using E_Voucher.Entities.Database.EVoucherSystem.DbModel;
using E_Voucher.Entities.DTO;
using E_Voucher.Entities.Request_Models;
using E_Voucher.Entities.Response_Models;
using E_Voucher.Repositories.Helper;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace E_Voucher.Repositories
{
    public class UserRepository : IUserRepository
    {
        private EVoucher_System_DBContex db_Evoucher;
        private readonly IConfiguration configuration;

        public UserRepository(EVoucher_System_DBContex _EVoucher_DBContex, IConfiguration _configuration) 
        {
            db_Evoucher = _EVoucher_DBContex;
            configuration = _configuration;
        }


        public LoginResponse Login(LoginRequest _request)
        {
            LoginResponse response = new LoginResponse();
            string hashedPassword = StringHelper.GenerateHash(_request.Password);

            var user = (from u in db_Evoucher.TblUsers
                        join up in db_Evoucher.TblUserPassword
                        on u.UserId equals up.UserId
                        where u.LoginId == _request.LoginId && up.Password1==hashedPassword
                        select u).FirstOrDefault();
            if (user != null)
            {
                GetGenerateTokenDTO getGenerateToken = new GetGenerateTokenDTO
                {
                    Audience = configuration["Audience"],
                    Issuer = configuration["Issuer"],
                    PrivateKey = configuration["PrivateKey"],
                    TokenExpiryMinute = Int32.Parse(configuration["TokenExpiryMinute"]),
                    RefreshTokenExpiryMinute = Int32.Parse(configuration["RefreshTokenExpiryMinute"]),
                    UserId = user.UserId,
                    UserName = user.DisplayName
                };
                TokenGeneratedDTO generatedToken = JWTHelper.GenerateToken(getGenerateToken);
                if (String.IsNullOrEmpty(generatedToken.ErrorStatus))
                {
                    response.AccessToken = generatedToken.AccessToken;
                    response.AccessTokenExpireHours = generatedToken.TokenExpiresMinute;
                    response.RefreshToken = generatedToken.RefreshToken;
                    response.RefreshTokenExpireHours = Int32.Parse(configuration["RefreshTokenExpiryMinute"]);
                    SaveRefreshToken(generatedToken);
                }
                else
                {
                    response.ErrorStatus = generatedToken.ErrorStatus;
                }
            }
            else
            {
                response.ErrorStatus = "Invalid user name or password!";
            }
            DeleteExpiryToken();
            db_Evoucher.SaveChanges();
            return response;
        }

        public void DeleteExpiryToken()
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
                                    where rt.RefreshToken==refreshToken
                                    select rt).FirstOrDefault();
            db_Evoucher.TblRefreshToken.Remove(deleteLoginToken);
        }
        public void SaveRefreshToken(TokenGeneratedDTO tokenDto)
        {
            TblRefreshToken tblRefreshToken = new TblRefreshToken
            {
                ExpiryDate = DateTime.Now.AddMinutes(tokenDto.RefreshTokenExpiresMinute),
                RefreshToken = tokenDto.RefreshToken,
                UserId = tokenDto.UserId
            };
            db_Evoucher.TblRefreshToken.Add(tblRefreshToken);
            
        }

        public RefreshTokenResponse RefreshToken(RefreshTokenRequest _request,string token)
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
                                        && rt.UserId==validatedToken.UserID
                                        && rt.ExpiryDate > DateTime.Now
                                        select rt).FirstOrDefault();
                if(tblRefreshToken!=null && tblRefreshToken.RefreshToken != "")
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
                    if(generatedToken != null && string.IsNullOrEmpty(generatedToken.ErrorStatus))
                    {
                        response.AccessToken = generatedToken.AccessToken;
                        response.AccessTokenExpireHours = generatedToken.TokenExpiresMinute;
                        response.RefreshToken = generatedToken.RefreshToken;
                        response.RefreshTokenExpireHours = Int32.Parse(configuration["RefreshTokenExpiryMinute"]);
                        SaveRefreshToken(generatedToken);
                        DeleteRefreshToken(_request.RefreshToken);
                    }
                    else
                    {
                        response.StatusCode = 500;
                        response.ErrorType = "Token-Generation Fail.";
                        response.ErrorMessage = "Unable to generate Access Token.";
                    }

                    
                    
                    DeleteExpiryToken();

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

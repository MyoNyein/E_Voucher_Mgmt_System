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
                    response.AccessTokenExpireMinutes = generatedToken.TokenExpiresMinute;
                    response.RefreshToken = generatedToken.RefreshToken;
                    response.RefreshTokenExpireMinutes = Int32.Parse(configuration["RefreshTokenExpiryMinute"]);
                    response.UserId = user.UserId;
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
            db_Evoucher.SaveChanges();
            return response;
        }

    }
}

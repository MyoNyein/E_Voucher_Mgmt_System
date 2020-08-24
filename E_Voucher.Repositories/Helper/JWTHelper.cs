using E_Voucher.Entities.DTO;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_Voucher.Repositories.Helper
{
    public static class JWTHelper
    {
        public static TokenGeneratedDTO GenerateToken(GetGenerateTokenDTO tokenDTO)
        {
            TokenGeneratedDTO _tokendata = new TokenGeneratedDTO();


            var privateKey = tokenDTO.PrivateKey;
            if (privateKey != "")
            {
                try
                {
                    RSACryptoServiceProvider rsaService = new RSACryptoServiceProvider();
                    rsaService.FromXmlString(privateKey);
                    var ExpiryDate = DateTime.Now.AddMinutes(tokenDTO.TokenExpiryMinute);
                    var refreshToken = Guid.NewGuid().ToString();
                   

                    var encryptedRefreshToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(refreshToken));
                    //var dec = Encoding.UTF8.GetString(Convert.FromBase64String(enc));

                    var authClaims = new[]
                    {
                        new Claim("UserID", tokenDTO.UserId.ToString()),
                        new Claim("UserName", tokenDTO.UserName)
                    };



                    var jwttoken = new JwtSecurityToken(
                        issuer: tokenDTO.Issuer,
                        audience: tokenDTO.Audience,
                        expires: ExpiryDate,
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(new RsaSecurityKey(rsaService), SecurityAlgorithms.RsaSha256)
                        );
                    _tokendata.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwttoken);
                    _tokendata.RefreshToken = encryptedRefreshToken;
                    _tokendata.TokenExpiresMinute = tokenDTO.TokenExpiryMinute;
                    _tokendata.RefreshTokenExpiresMinute = tokenDTO.RefreshTokenExpiryMinute;
                    _tokendata.ErrorStatus = "";
                    _tokendata.UserId = tokenDTO.UserId;
                    //var uinfo = LoginUserInfo(_userId, _coid);
                    //if (uinfo != null)
                    //{
                    //    _tokendata.UserName = uinfo.UserName;
                    //    _tokendata.Designation = uinfo.Designation;
                    //}
                }
                catch (Exception e)
                {
                    _tokendata.ErrorStatus = "Error occur while generate token." + e.Message;
                }
            }
            else
            {
                _tokendata.ErrorStatus = "Private Key can't be empty.";
            }

            return _tokendata;
        }

        public static ValidateTokenDTO CheckValidToken(CheckValidateTokenDTO tokenDTO)
        {
            ValidateTokenDTO validToken = new ValidateTokenDTO();
            RSACryptoServiceProvider privateKey = new RSACryptoServiceProvider();
            privateKey.FromXmlString(tokenDTO.PrivateKey);

            tokenDTO.Token = tokenDTO.Token.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

            TokenValidationParameters validationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = tokenDTO.Issuer,
                ValidAudience = tokenDTO.Audience,
                IssuerSigningKey = new RsaSecurityKey(privateKey),
                ValidateLifetime = tokenDTO.IsValidateExpiry, 
                ClockSkew = TimeSpan.FromMinutes(0) //0 minute tolerance for the expiration date
            };
            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            try
            {
                var payload = handler.ValidateToken(tokenDTO.Token, validationParameters, out validatedToken);
                Int32.TryParse(payload.Claims.Where(c => c.Type == "UserID").Select(c => c.Value).SingleOrDefault(), out int userId);
                var userName = payload.Claims.Where(c => c.Type == "UserName").Select(c => c.Value).SingleOrDefault();

                validToken.UserID = userId;
                validToken.UserName = userName;
                validToken.IsValid = true;


            }
            catch (Exception e)
            {
                validToken.IsValid = false;
                validToken.ErrorMessage= e.Message;
            }
            try
            {
                //DeleteExpiryToken();
            }
            catch(Exception e)
            {
                validToken.ErrorMessage = "Unable to delete expiry access tokens " + e.Message;
            }
            return validToken;
        }
    }
}

using AutoMapper;
using E_Voucher.CMS_API.Helper;
using E_Voucher.Entities.DTO;
using E_Voucher.Entities.Request_Models;
using E_Voucher.Entities.Response_Models;
using E_Voucher.Repositories;
using E_Voucher.Repositories.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace E_Voucher.CMS_API.Controller
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepositoryWrapper repo;
        private readonly ILogger log;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserController(IRepositoryWrapper _repo,ILogger<UserController> _log,IHttpContextAccessor _httpContextAccessor)
        {
            repo = _repo;
            log = _log;
            httpContextAccessor = _httpContextAccessor;
        }

        [Route("v1/User/Login")]
        [HttpPost]
        public IActionResult Login(LoginRequest _request)
        {
            try
            {
                log.LogInformation($"Login\r\n");
                var response = repo.User.Login(_request);
                if (String.IsNullOrEmpty(response.ErrorStatus))
                {
                    repo.RefreshToken.SaveRefreshToken(new SaveRefreshTokenDTO
                    {
                        ExpiryMinute = response.RefreshTokenExpireMinutes,
                        RefreshToken = response.RefreshToken,
                        UserId  =response.UserId
                    });
                    log.LogInformation($"Login Success");
                    return Ok(response);
                }
                else
                {
                    log.LogError($"Login Error\r\n{response.ErrorStatus}");
                    return NotFound(new Error("Un-authorized",response.ErrorStatus));
                }
            }
            catch (Exception e)
            {
                log.LogError($"Login Error\r\n{e}");
                return StatusCode(500, new Error("internal_error", e.Message));
            }
        }

        [Route("v1/User/RefreshToken")]
        [HttpPost]
        public IActionResult RefreshToken(RefreshTokenRequest _request)
        {
            var APIName = "RefreshToken";
            try
            {
                log.LogInformation($"{APIName}\r\n");
                var requestHeader = httpContextAccessor.HttpContext.Request.Headers;
                string accessToken = requestHeader["Authorization"];
                var response = repo.RefreshToken.RefreshToken(_request,accessToken);

                if (response.StatusCode==200)
                {
                    log.LogInformation($"{APIName} Success");
                    return Ok(response);
                }
                else
                {
                    log.LogError($"{APIName}\r\nStautsCode:{response.StatusCode}\r\nErrorType:{response.ErrorType}" +
                                 $"\r\nErrorMsg:{response.ErrorMessage}");
                    return StatusCode(response.StatusCode, response.GetError());
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName} Error\r\n{e}");
                return StatusCode(500, new Error("internal_error", e.Message));
            }
        }



        //[Route("v1/User/Test")]
        //[HttpPost]
       
        //public IActionResult Test()
        //{
        //    var tmp = StringHelper.codeFromCoupon("ABCDEFG");
        //    //var tmp2 = StringHelper.couponCode(tmp);
        //   // string promoCode = "ABCDE";
        //   // StringHelper.GeneatePromo();
        //    //string qrCodePath = QRCodeHelper.GenerateQRCode(promoCode);
        //    return Ok("Hello"); 
        //}

        //[Route("v1/User/GenerateHash")]
        //[HttpGet]
        //public IActionResult GenerateHash(string password)
        //{
        //    try
        //    {
        //        return Ok(repository.GenerateHash(password));
        //        //return Ok("");
        //    }
        //    catch(Exception e)
        //    {
        //        return StatusCode(500, new Error("internal_error", e.Message));
        //    }

       // }
    }
}
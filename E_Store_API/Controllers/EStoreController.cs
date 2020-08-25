using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E_Voucher.Entities.DTO;
using E_Voucher.Entities.Request_Models;
using E_Voucher.Entities.Response_Models;
using E_Voucher.Repositories;
using E_Voucher.Repositories.Helper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace E_Store_API.Controllers
{

    [ApiController]
    public class EStoreController : ControllerBase
    {
        private readonly IRepositoryWrapper repo;
        private readonly ILogger log;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IDistributedCache distributedCache;

        public EStoreController(IRepositoryWrapper _repo, ILogger<EStoreController> _log, IHttpContextAccessor _httpContextAccessor,IDistributedCache _distributedCache)
        {
            repo = _repo;
            log = _log;
            httpContextAccessor = _httpContextAccessor;
            distributedCache = _distributedCache;
        }

        [Route("v1/EStore/GetAccessToken")]
        [HttpPost]
        public IActionResult GetAccessToken(EstoreAccessTokenRequest _request)
        {
            string APIName = "GetAccessToken";
            try
            {
                log.LogInformation($"{APIName}\r\n");
                var response = repo.Estore.GetAccessToken(_request);
                if (response.StatusCode==200)
                {
                    repo.RefreshToken.SaveRefreshToken(new SaveRefreshTokenDTO
                    {
                        ExpiryMinute = response.RefreshTokenExpireMinutes,
                        RefreshToken = response.RefreshToken,
                        UserId = 0
                    });
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

        [Route("v1/EStore/RefreshToken")]
        [HttpPost]
        public IActionResult RefreshToken(RefreshTokenRequest _request)
        {
            var APIName = "RefreshToken";
            try
            {
                log.LogInformation($"{APIName}\r\n");
                var requestHeader = httpContextAccessor.HttpContext.Request.Headers;
                string accessToken = requestHeader["Authorization"];
                var response = repo.RefreshToken.RefreshToken(_request, accessToken);

                if (response.StatusCode == 200)
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

        [Route("v1/EStore/GetStoreEvoucherList")]
        [HttpPost]
        [Authorize()]
        public IActionResult GetStoreEvoucherList(GetEVoucherListingRequest _request)
        {
            string APIName = "GetStoreEvoucherList";
            log.LogInformation($"{APIName}\r\nStatus:{_request.Status}\r\nPageNum:{_request.PageNumber}\r\nPageSize{_request.PageSize}");
            try
            {

                var response = repo.Estore.GetStoreEvoucherList(_request);
                
                if (response != null && response.Count > 0)
                {
                    Response.Headers.Add("X-Pagination", PageListHelper.GetPagingMetadata(response));
                    log.LogInformation($"{APIName}\r\n Get List Count :{response.Count}");
                    return Ok(response);
                }
                else
                {
                    log.LogError($"{APIName}\r\nNo Record Found");
                    return NotFound(new Error("Not-Found","No Record Found"));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName}\r\n{e}");
                return StatusCode(500, new Error("internal-error", e.Message));
            }
        }

        [Route("v1/EStore/GetEvoucherDetail")]
        [HttpPost]
        [Authorize()]
        public IActionResult GetEvoucherDetail(GetEVoucherDetailRequest _request)
        {
            string APIName = "SubmitEVoucher";
            log.LogInformation($"{APIName}\r\njson={StringHelper.SerializeObject(_request)}");
            try
            {
                var response = repo.EVoucher.GetEvoucherDetail(_request);
                if (response!=null)
                {
                    log.LogInformation($"{APIName}\r\n Submit Success ");
                    return Ok(response);
                }
                else
                {
                    log.LogError($"{APIName}\r\nStautsCode:404\r\nErrorType:Record Not Found");
                    return NotFound(new Error("RecordNotFound", "Record Not Found"));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName}\r\n{e}");
                return StatusCode(500, new Error("internal-error", e.Message));
            }
        }

        [Route("v1/EStore/GetPaymentMethodList")]
        [HttpPost]
        [Authorize()]
        public IActionResult GetPaymentMethodList()
        {
            string APIName = "GetPaymentMethodList";
            log.LogInformation($"{APIName}");
            try
            {
                var paymentListCatch = distributedCache.GetString("PaymentMethodList");
                if (string.IsNullOrEmpty(paymentListCatch))
                {
                    var response = repo.Estore.GetPaymentMethodList();
                    //Response.Headers.Add("X-Pagination", PageListHelper.GetPagingMetadata(response));
                    if (response != null && response.Count > 0)
                    {
                        distributedCache.SetString("PaymentMethodList", JsonConvert.SerializeObject(response));
                        log.LogInformation($"{APIName}\r\n Get List Count :{response.Count}");
                        return Ok(response);
                    }
                    else
                    {
                        log.LogError($"{APIName}\r\nNo Record Found");
                        return NotFound(new Error("Not-Found", "No Record Found"));
                    }
                }
                else
                {

                    var response = JsonConvert.DeserializeObject<List<GetPaymentMethodListResponse>>(paymentListCatch);

                    log.LogInformation($"{APIName}\r\n Get List from Catch Count :{response.Count}");
                    return Ok(response);
                }
               
            }
            catch (Exception e)
            {
                log.LogError($"{APIName}\r\n{e}");
                return StatusCode(500, new Error("internal-error", e.Message));
            }
        }

        [Route("v1/EStore/BuyEVoucher")]
        [HttpPost]
        [Authorize()]
        public IActionResult BuyEVoucher(BuyEVoucherRequest _request)
        {
            string APIName = "BuyEVoucher";
            log.LogInformation($"{APIName}\r\njson={StringHelper.SerializeObject(_request)}");
            try
            {
                var response = repo.Estore.BuyEVoucher(_request);
                if (response.StatusCode == 200)
                {
                    //First generate promocode when order success
                    var generatePromoJobId = BackgroundJob.Enqueue(() => repo.Estore.ScheduleGeneratePromoCode(new GeneratePromoCodeRequest
                    {
                        PurchaseOrder_No = response.OrderNo
                    })) ;
                    //after promocode was generated add all to purchase history
                    BackgroundJob.ContinueJobWith(generatePromoJobId, () => repo.Estore.ScheduleUpdatePaymentHistory());

                    log.LogInformation($"{APIName}\r\n Buy Success ");
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
                log.LogError($"{APIName}\r\n{e}");
                return StatusCode(500, new Error("internal-error", e.Message));
            }
        }

        [Route("v1/EStore/CheckStockAvaliable")]
        [HttpPost]
        [Authorize()]
        public IActionResult CheckStockAvaliable(CheckStockAvaliableRequest _request)
        {
            string APIName = "CheckStockAvaliable";
            log.LogInformation($"{APIName}\r\njson={StringHelper.SerializeObject(_request)}");
            try
            {
                var response = repo.Estore.CheckStockAvaliable(_request);
                if (response != null)
                {
                    log.LogInformation($"{APIName}\r\n Check Stock Success.\r\nRemaining Amount:{response.RemainingQuantity}");
                    return Ok(response);
                }
                else
                {
                    log.LogError($"{APIName}\r\nStautsCode:404\r\nErrorType:Record Not Found");
                    return NotFound(new Error("RecordNotFound", "Record Not Found"));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName}\r\n{e}");
                return StatusCode(500, new Error("internal-error", e.Message));
            }
        }

        [Route("v1/EStore/CheckPromoCode")]
        [HttpPost]
        [Authorize()]
        public IActionResult CheckPromoCode(CheckPromoCodeRequest _request)
        {
            string APIName = "CheckPromoCode";
            log.LogInformation($"{APIName}\r\njson={StringHelper.SerializeObject(_request)}");
            try
            {
                var response = repo.Estore.CheckPromoCode(_request);
                if (response != null)
                {
                    log.LogInformation($"{APIName}\r\n Check PromoCode Success ");
                    return Ok(response);
                }
                else
                {
                    log.LogError($"{APIName}\r\nStautsCode:404\r\nErrorType:Record Not Found");
                    return NotFound(new Error("RecordNotFound", "Record Not Found"));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName}\r\n{e}");
                return StatusCode(500, new Error("internal-error", e.Message));
            }
        }

        [Route("v1/EStore/GetPurchaseHistoryList")]
        [HttpPost]
        [Authorize()]
        public IActionResult GetPurchaseHistoryList(GetPurchaseHistoryRequest _request)
        {
            string APIName = "GetPurchaseHistoryList";
            log.LogInformation($"{APIName}\r\nPageNum:{_request.PageNumber}\r\nPageSize{_request.PageSize}");
            try
            {
                var response = repo.Estore.GetPurchaseHistory(_request);
               
                if (response != null && response.Count > 0)
                {
                    Response.Headers.Add("X-Pagination", PageListHelper.GetPagingMetadata(response));
                    log.LogInformation($"{APIName}\r\n Get List Count :{response.Count}");
                    return Ok(response);
                }
                else
                {
                    log.LogError($"{APIName}\r\nNo Record Found");
                    return NotFound(new Error("Not-Found", "No Record Found"));
                }
            }
            catch (Exception e)
            {
                log.LogError($"{APIName}\r\n{e}");
                return StatusCode(500, new Error("internal-error", e.Message));
            }
        }

        [Route("v1/EStore/TestFire")]
        [HttpPost]
        public IActionResult TestFire()
        {
            //repo.Estore.ScheduleGeneratePromoCode(new GeneratePromoCodeRequest
            //{
            //    PurchaseOrder_No = "PO-000003"
            //});
            var generatePromoJobId = BackgroundJob.Enqueue(() => repo.Estore.ScheduleGeneratePromoCode(new GeneratePromoCodeRequest
            {
                PurchaseOrder_No = "PO-000003"
            }));

            BackgroundJob.ContinueJobWith(generatePromoJobId,() => repo.Estore.ScheduleUpdatePaymentHistory());
       

            Console.WriteLine("Caller Finished");
            return Ok($"");
        }

    }
}
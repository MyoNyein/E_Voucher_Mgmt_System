using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Voucher.Entities.Request_Models;
using E_Voucher.Entities.Response_Models;
using E_Voucher.Repositories;
using E_Voucher.Repositories.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace E_Store_API.Controllers
{

    [ApiController]
    public class EVoucherController : ControllerBase
    {
        private readonly IRepositoryWrapper repo;
        private readonly ILogger log;

        public EVoucherController(IRepositoryWrapper _repo, ILogger<EVoucherController> _log)
        {
            repo = _repo;
            log = _log;
        }

        [Route("v1/EVoucher/GetStoreEvoucherList")]
        [HttpPost]
        [Authorize()]
        public IActionResult GetStoreEvoucherList(GetEVoucherListingRequest _request)
        {
            string APIName = "GetStoreEvoucherList";
            log.LogInformation($"{APIName}\r\nStatus:{_request.Status}\r\nPageNum:{_request.PageNumber}\r\nPageSize{_request.PageSize}");
            try
            {

                var response = repo.EVoucher.GetStoreEvoucherList(_request);
                
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

        [Route("v1/EVoucher/GetEvoucherDetail")]
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

        [Route("v1/EVoucher/GetPaymentMethodList")]
        [HttpPost]
        [Authorize()]
        public IActionResult GetPaymentMethodList()
        {
            string APIName = "GetPaymentMethodList";
            log.LogInformation($"{APIName}");
            try
            {

                var response = repo.EVoucher.GetPaymentMethodList();
                //Response.Headers.Add("X-Pagination", PageListHelper.GetPagingMetadata(response));
                if (response != null && response.Count > 0)
                {

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

        [Route("v1/EVoucher/BuyEVoucher")]
        [HttpPost]
        [Authorize()]
        public IActionResult BuyEVoucher(BuyEVoucherRequest _request)
        {
            string APIName = "BuyEVoucher";
            log.LogInformation($"{APIName}\r\njson={StringHelper.SerializeObject(_request)}");
            try
            {
                var response = repo.EVoucher.BuyEVoucher(_request);
                if (response.StatusCode == 200)
                {
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

        [Route("v1/EVoucher/CheckStockAvaliable")]
        [HttpPost]
        [Authorize()]
        public IActionResult CheckStockAvaliable(CheckStockAvaliableRequest _request)
        {
            string APIName = "CheckStockAvaliable";
            log.LogInformation($"{APIName}\r\njson={StringHelper.SerializeObject(_request)}");
            try
            {
                var response = repo.EVoucher.CheckStockAvaliable(_request);
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

        [Route("v1/EVoucher/CheckPromoCode")]
        [HttpPost]
        [Authorize()]
        public IActionResult CheckPromoCode(CheckPromoCodeRequest _request)
        {
            string APIName = "CheckPromoCode";
            log.LogInformation($"{APIName}\r\njson={StringHelper.SerializeObject(_request)}");
            try
            {
                var response = repo.EVoucher.CheckPromoCode(_request);
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

        [Route("v1/EVoucher/GetPurchaseHistoryList")]
        [HttpPost]
        [Authorize()]
        public IActionResult GetPurchaseHistoryList(GetPurchaseHistoryRequest _request)
        {
            string APIName = "GetPurchaseHistoryList";
            log.LogInformation($"{APIName}\r\nPageNum:{_request.PageNumber}\r\nPageSize{_request.PageSize}");
            try
            {
                var response = repo.EVoucher.GetPurchaseHistory(_request);
               
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

    }
}
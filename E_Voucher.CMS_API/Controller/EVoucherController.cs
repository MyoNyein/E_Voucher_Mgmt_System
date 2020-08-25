using E_Voucher.Entities.Request_Models;
using E_Voucher.Entities.Response_Models;
using E_Voucher.Repositories;
using E_Voucher.Repositories.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace E_Voucher.CMS_API.Controller
{
    [ApiController]
    public class EVoucherController : ControllerBase
    {
        private readonly IRepositoryWrapper repo;
        private readonly ILogger log;
        private readonly IDistributedCache distributedCache;

        public EVoucherController(IRepositoryWrapper _repo, ILogger<EVoucherController> _log, IDistributedCache _distributedCache)
        {
            repo = _repo;
            log = _log;
            distributedCache = _distributedCache;
        }

        [Route("v1/EVoucher/SubmitEvoucher")]
        [HttpPost]
        [Authorize()]
        public IActionResult SubmitEVoucher(SubmitEVoucherRequest _request)
        {
            string APIName = "SubmitEVoucher";
            log.LogInformation($"{APIName}\r\njson={StringHelper.SerializeObject(_request)}");
            try
            {
                var response = repo.EVoucher.SubmitEVoucher(_request);
                if (response.StatusCode == 200)
                {
                    log.LogInformation($"{APIName}\r\n Submit Success ");
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

        
        [Route("v1/EVoucher/GetEvoucherList")]
        [HttpGet]
        [Authorize()]
        public IActionResult GetEvoucherList([FromQuery]GetEVoucherListingRequest _request)
        {
            string APIName = "GetEvoucherList";
            var tmp = LoginInformation.UserName;
            var tmpe = LoginInformation.UserID;
            log.LogInformation($"{APIName}\r\nStatus:{_request.Status}\r\nPageNum:{_request.PageNumber}\r\nPageSize{_request.PageSize}");
            try
            {
               
                var response = repo.EVoucher.GetEvoucherList(_request);
                Response.Headers.Add("X-Pagination", PageListHelper.GetPagingMetadata(response));
                
                if (response!=null && response.Count>0)
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

        [Route("v1/EVoucher/UpdateEVoucherStatus")]
        [HttpPost]
        [Authorize()]
        public IActionResult UpdateEVoucherStatus(UpdateEVoucherStatusRequest _request)
        {
            string APIName = "UpdateEVoucherStatus";
            log.LogInformation($"{APIName}\r\njson={StringHelper.SerializeObject(_request)}");
            try
            {
                var response = repo.EVoucher.UpdateEVoucherStatus(_request);
                if (response.StatusCode == 200)
                {
                    log.LogInformation($"{APIName}\r\n Update Status Success ");
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

        [Route("v1/EVoucher/GetEvoucherDetail")]
        [HttpGet]
        [Authorize()]
        public IActionResult GetEvoucherDetail([FromQuery]GetEVoucherDetailRequest _request)
        {
            string APIName = "SubmitEVoucher";
            log.LogInformation($"{APIName}\r\njson={StringHelper.SerializeObject(_request)}");
            try
            {
                var response = repo.EVoucher.GetEvoucherDetail(_request);
                if (response != null)
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

    }
}
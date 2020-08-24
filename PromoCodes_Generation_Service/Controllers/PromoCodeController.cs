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

namespace PromoCodes_Generation_Service.Controllers
{
    [ApiController]
    public class PromoCodeController : ControllerBase
    {
        private readonly IRepositoryWrapper repo;
        private readonly ILogger log;
        //private readonly IDistributedCache distributedCache;

        public PromoCodeController(IRepositoryWrapper _repo, ILogger<PromoCodeController> _log)
        {
            repo = _repo;
            log = _log;
           // distributedCache = _distributedCache;
        }

        [Route("v1/EVoucher/GeneratePromoCode")]
        [HttpPost]
        [Authorize()]
        public IActionResult GeneratePromoCode(GeneratePromoCodeRequest _request)
        {
            string APIName = "GeneratePromoCode";
            log.LogInformation($"{APIName}\r\njson={StringHelper.SerializeObject(_request)}");
            try
            {
                var response = repo.PromoCode.GeneratePromoCode(_request);
                if (response.StatusCode == 200)
                {
                    log.LogInformation($"{APIName}\r\n Success ");
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
    }
}
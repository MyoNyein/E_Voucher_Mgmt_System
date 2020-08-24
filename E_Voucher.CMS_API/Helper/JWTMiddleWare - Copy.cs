using E_Voucher.Entities.DTO;
using E_Voucher.Entities.Response_Models;
using E_Voucher.Repositories;
using E_Voucher.Repositories.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Voucher.CMS_API.Helper
{
    public class AccessServiceAuthorize : AuthorizeAttribute
    {
       
        public AccessServiceAuthorize()
        {
           
        }

    }
    public class AccessServiceRequirement : IAuthorizationRequirement
    {
        public AccessServiceRequirement()
        {
        }
    }
    public class AccessServicePolicy : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider defaultPolicyProvider { get; }
        public AccessServicePolicy(IOptions<AuthorizationOptions> options)
        {
            defaultPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return defaultPolicyProvider.GetDefaultPolicyAsync();
        }
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return defaultPolicyProvider.GetFallbackPolicyAsync();
        }
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            string[] subStringPolicy = policyName.Split(new char[] { '.' });

            //if (subStringPolicy[0] == "AccessLevel" && Int32.TryParse(subStringPolicy[1], out var access))
            //{
            //    var policy = new AuthorizationPolicyBuilder();
            //    policy.AddRequirements(new AccessServiceRequirement());
            //    return Task.FromResult(policy.Build());
            //}
            //else
            //{
            //   // AccessLevel = AccessLevel.NO_ACCESS;
            //}
            //var policy = new AuthorizationPolicyBuilder();
            //policy.AddRequirements(new AccessServiceRequirement());
            //return Task.FromResult(policy.Build());
            return defaultPolicyProvider.GetPolicyAsync("JwtBearer");
        }


    }
    public class AccessServiceHandler : AuthorizationHandler<AccessServiceRequirement>
    {
        private IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration configuration;
       // private static readonly ILog log = log4net.LogManager.GetLogger(typeof(AccessServiceHandler));

        public AccessServiceHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessServiceRequirement requirement)
        {
            var requestHeader = httpContextAccessor.HttpContext.Request.Headers;
            string accessToken = requestHeader["Authorization"];
            if (String.IsNullOrEmpty(accessToken))
                accessToken = "";

            accessToken = accessToken.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
            string errorStatus="";
            var tmp = requestHeader.Keys;
            var route = httpContextAccessor.HttpContext.Request.RouteValues;

            CheckValidateTokenDTO checkValidate= new CheckValidateTokenDTO 
            {
                Issuser = configuration["Issuer"],
                Audience= configuration["Audiance"],
                PrivateKey = configuration["PrivateKey"],
                IsValidateExpiry = true,
                Token = accessToken
            };
            var validToken = JWTHelper.CheckValidToken(checkValidate);

            if (!validToken.IsValid )
            {
                //log.Error($"Invalid or Expire Access Token");
                errorStatus = "Invalid or Expire Access Token";
            }

            if (errorStatus == "")
            {
                context.Succeed(requirement);

                if (requestHeader.ContainsKey("UserID")) requestHeader.Remove("UserID");
                
                requestHeader.Add("UserID", validToken.UserID);
            }
            else
            {
                context.Fail();
                Error err = new Error("Unauthorized Request", errorStatus);
                var Response = httpContextAccessor.HttpContext.Response;
                var message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(err));
                httpContextAccessor.HttpContext.Response.StatusCode = 401;
                httpContextAccessor.HttpContext.Response.ContentType = "application/json;";
                Response.Body.WriteAsync(message, 0, message.Length);
            }
            return Task.CompletedTask;
        }

    }

}

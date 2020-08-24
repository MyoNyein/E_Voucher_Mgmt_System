using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using E_Voucher.Repositories;
using E_Voucher.Entities.Database.EVoucherSystem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using E_Voucher.Entities.Response_Models;
using System.Text;
using Newtonsoft.Json;
using PromoCodes_Generation_Service.Helper;

namespace PromoCodes_Generation_Service.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDbContex(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<EVoucher_System_DBContex>(options => options.UseSqlServer(config.GetConnectionString("EVoucher_SystemCon")));
        }
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }
        public static void ConfigureAuthorization(this IServiceCollection services, IConfiguration config)
        {

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";

            }).AddJwtBearer("JwtBearer", jwtBearerOptions =>
            {
                jwtBearerOptions.SecurityTokenValidators.Clear();
                jwtBearerOptions.SecurityTokenValidators.Add(new JwtValidationHandler(config));
                jwtBearerOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        string errorStatus = "";
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                            errorStatus = "Token is Expired.";
                        }
                        else
                        {
                            errorStatus = "Invalid Token!";
                        }
                        Error err = new Error("Unauthorized Request", errorStatus);

                        var message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(err));
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json;";
                        context.Response.Body.WriteAsync(message, 0, message.Length);
                        return Task.CompletedTask;
                    }
                };
            });
        }
        public static void ConfigureValidateModel(this IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelAttribute));
            });
            services.Configure<ApiBehaviorOptions>(options => {
                options.SuppressModelStateInvalidFilter = true;
            });
        }
        //public static void ConfigureRadis(this IServiceCollection services, IConfiguration config)
        //{
        //    services.AddDistributedRedisCache(option =>
        //    {
        //        option.Configuration = config["RedisURL"];
        //        option.InstanceName = "master";
        //    });
        //}
    }
}

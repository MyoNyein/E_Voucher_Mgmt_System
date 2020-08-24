using E_Voucher.Entities.Response_Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PromoCodes_Generation_Service.Helper
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                if (context.ModelState != null && context.ModelState.Any(m => m.Value.Errors.Count > 0))
                {
                    string message = "Please correct the specified errors and try again." + Environment.NewLine;

                    foreach (var state in context.ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            string errorMessage = string.IsNullOrEmpty(error.ErrorMessage) ? error.Exception.Message : error.ErrorMessage;
                            message = string.Format("{0}{1}{2}", message, errorMessage, Environment.NewLine);
                        }
                    }

                    context.Result = new BadRequestObjectResult(new Error("application_validation_error", message));
                }
            }
        }
    }
}

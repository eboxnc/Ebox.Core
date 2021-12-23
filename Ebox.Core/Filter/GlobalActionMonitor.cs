using Ebox.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Ebox.Core.Filter
{
    public class GlobalActionMonitor : Attribute, IActionFilter
    {
        public GlobalActionMonitor()
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            #region 模型验证

            if (context.ModelState.IsValid) return;
            var response = Result.Fail(System.Net.HttpStatusCode.InternalServerError, "");


            foreach (var item in context.ModelState.Values)
            {
                foreach (var error in item.Errors)
                {
                    if (!string.IsNullOrEmpty(response.Msg))
                    {
                        response.Msg += " | ";
                    }

                    response.Msg += error.ErrorMessage;
                }
            }

            context.Result = new JsonResult(response);
            #endregion
        }
    }
}

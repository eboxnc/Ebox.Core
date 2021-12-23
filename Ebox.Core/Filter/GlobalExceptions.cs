using Ebox.Core.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Ebox.Core.Filter
{
    public class GlobalExceptions : Attribute, IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GlobalExceptions> _logger;

        public GlobalExceptions(IWebHostEnvironment env, ILogger<GlobalExceptions> logger)
        {
            _env = env;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {

            var response = Result.Fail(System.Net.HttpStatusCode.InternalServerError, context.Exception.Message, "");
            if (_env.IsDevelopment())
            {
                response.Data = context.Exception.StackTrace;//堆栈信息
            }

            context.Result = new JsonResult(response) { StatusCode = 200 };

            //采用log4net 进行错误日志记录
            _logger.LogError(WriteLog(response.Msg, context.Exception));

        }

        /// <summary>
        /// 自定义返回格式
        /// </summary>
        /// <param name="throwMsg"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public string WriteLog(string throwMsg, Exception ex)
        {
            return $"\r\n【自定义错误】：{throwMsg} \r\n【异常类型】：{ex.GetType().Name} \r\n【异常信息】：{ex.Message} \r\n【堆栈调用】：{ex.StackTrace }\r\n";
        }

    }
}

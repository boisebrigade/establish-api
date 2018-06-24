using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CFABB.SelfRescue.Services
{
    public class ExceptionMiddleware {
        private readonly RequestDelegate next;
        public ExceptionMiddleware(RequestDelegate next) {
            this.next = next;
        }

        public async Task Invoke(HttpContext context) {
            try {
                await next(context);
            } catch (Exception ex) {
                await HandleException(context, ex);
            }
        }

        private async Task HandleException(HttpContext context, Exception ex) {
            if (ex is WebException && (HttpWebResponse)((WebException)ex).Response != null) {
                var error = ex.Message;
                var response = (HttpWebResponse)((WebException)ex).Response;
                string responseBody = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
                var result = JsonConvert.SerializeObject(new { HttpError = error, ResponseBody = responseBody });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync(result);
            } else {
                var result = JsonConvert.SerializeObject(new { HttpError = ex.Message });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(result);
            }
        }
    }
}

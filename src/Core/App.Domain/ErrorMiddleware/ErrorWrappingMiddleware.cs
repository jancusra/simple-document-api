using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using App.Domain.Exceptions;
using App.Domain.Responses;

namespace App.Domain.ErrorMiddleware
{
    /// <summary>
    /// Error wrapping middleware
    /// </summary>
    public partial class ErrorWrappingMiddleware : BaseErrorMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorWrappingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Method to catch api exceptions
        /// </summary>
        /// <param name="context">HTTP context</param>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BaseResponseException baseResponseException)
            {
                await SendResponseIfNotStarted(context, baseResponseException.ResponseState);
            }
            catch
            {
                await SendResponseIfNotStarted(context, 
                new ResponseState { 
                    Code = 10000, 
                    HttpStatusCode = (int)HttpStatusCode.InternalServerError, 
                    Message = "Internal Server Error"
                });
            }
        }
    }
}

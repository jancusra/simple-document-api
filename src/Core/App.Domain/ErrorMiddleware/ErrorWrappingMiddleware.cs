using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<ErrorWrappingMiddleware> _logger;

        public ErrorWrappingMiddleware(RequestDelegate next, ILogger<ErrorWrappingMiddleware> logger)
        {
            this.next = next;
            _logger = logger;
        }

        /// <summary>
        /// Method to catch API exceptions
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
                // Expected business error (e.g. 400/404/409) - logged for visibility, not as a failure.
                _logger.LogInformation(
                    "Handled API exception ({StatusCode}): {Message}",
                    baseResponseException.ResponseState.HttpStatusCode,
                    baseResponseException.Message);

                await SendResponseIfNotStarted(context, baseResponseException.ResponseState);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception while processing {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                await SendResponseIfNotStarted(context,
                new ResponseState
                {
                    Code = 10000,
                    HttpStatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Internal Server Error"
                });
            }
        }
    }
}

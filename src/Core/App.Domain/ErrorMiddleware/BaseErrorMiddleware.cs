using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using MessagePack;
using App.Domain.Responses;

namespace App.Domain.ErrorMiddleware
{
    /// <summary>
    /// Represents base error middleware
    /// </summary>
    public abstract class BaseErrorMiddleware
    {
        /// <summary>
        /// Write the potential issue to the response 
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="responseState">response state</param>
        protected virtual async Task SendResponseIfNotStarted(HttpContext context, ResponseState responseState)
        {
            if (!context.Response.HasStarted)
            {
                var response = new BaseResponse(responseState.Code, responseState.Message);
                var responseBody = string.Empty;

                switch (context.Request.Headers[HttpDefaults.HeaderAcceptKey])
                {
                    case HttpDefaults.ContentTypeXml:
                        {
                            context.Response.ContentType = HttpDefaults.ContentTypeXml;

                            var serializer = new DataContractSerializer(typeof(BaseResponse));
                            using (var output = new StringWriter())

                            using (var writer = new XmlTextWriter(output) { Formatting = Formatting.Indented })
                            {
                                serializer.WriteObject(writer, response);
                                responseBody = output.GetStringBuilder().ToString();
                            }

                            break;
                        }
                    case HttpDefaults.ContentTypeMessagePack:
                        {
                            context.Response.ContentType = HttpDefaults.ContentTypeMessagePack;

                            byte[] bytes = MessagePackSerializer.Serialize(response);
                            responseBody = Encoding.UTF8.GetString(bytes);

                            break;
                        }
                    default:
                        {
                            context.Response.ContentType = HttpDefaults.ContentTypeJson;
                            responseBody = JsonSerializer.Serialize(response);

                            break;
                        }
                }

                context.Response.StatusCode = responseState.HttpStatusCode;
                await context.Response.WriteAsync(responseBody);
            }
        }
    }
}

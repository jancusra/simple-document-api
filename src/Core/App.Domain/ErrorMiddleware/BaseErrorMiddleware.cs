using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
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
            if (context.Response.HasStarted)
            {
                return;
            }

            var response = new BaseResponse(responseState.Code, responseState.Message);
            context.Response.StatusCode = responseState.HttpStatusCode;

            switch (ResolveContentType(context))
            {
                case HttpDefaults.ContentTypeXml:
                    {
                        context.Response.ContentType = HttpDefaults.ContentTypeXml;

                        var serializer = new DataContractSerializer(typeof(BaseResponse));
                        using (var output = new StringWriter())

                        using (var writer = new XmlTextWriter(output) { Formatting = Formatting.Indented })
                        {
                            serializer.WriteObject(writer, response);
                            await context.Response.WriteAsync(output.GetStringBuilder().ToString());
                        }

                        break;
                    }
                case HttpDefaults.ContentTypeMessagePack:
                    {
                        context.Response.ContentType = HttpDefaults.ContentTypeMessagePack;

                        byte[] bytes = MessagePackSerializer.Serialize(response);
                        await context.Response.Body.WriteAsync(bytes);

                        break;
                    }
                default:
                    {
                        context.Response.ContentType = HttpDefaults.ContentTypeJson;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));

                        break;
                    }
            }
        }

        /// <summary>
        /// Resolve the best supported response content type from the request Accept header,
        /// honoring multiple media types and their quality (q) values
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns>a supported content type; falls back to JSON</returns>
        private static string ResolveContentType(HttpContext context)
        {
            var acceptValues = context.Request.Headers[HttpDefaults.HeaderAcceptKey];

            if (MediaTypeHeaderValue.TryParseList(acceptValues, out var mediaTypes))
            {
                foreach (var mediaType in mediaTypes.OrderByDescending(x => x.Quality ?? 1.0))
                {
                    var value = mediaType.MediaType.Value;

                    if (value == HttpDefaults.ContentTypeXml
                        || value == HttpDefaults.ContentTypeMessagePack
                        || value == HttpDefaults.ContentTypeJson)
                    {
                        return value;
                    }
                }
            }

            return HttpDefaults.ContentTypeJson;
        }
    }
}

using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using MessagePack;
using App.Contracts.Models;
using App.Domain;
using App.Domain.Responses;

namespace App.Web.Tests
{
    /// <summary>
    /// Partial class representing integration tests for API error responses
    /// </summary>
    public partial class ApiIntegrationTests
    {
        private static StringContent JsonDocument(DocumentDto document)
        {
            return new StringContent(JsonSerializer.Serialize(document), Encoding.UTF8, HttpDefaults.ContentTypeJson);
        }

        private static DocumentDto NewDocument()
        {
            return new DocumentDto
            {
                id = Guid.NewGuid(),
                tags = ["error", "integration-test"],
                data = new JsonObject { ["field"] = "value" }
            };
        }

        [Fact]
        public async Task GetNonExistingDocument_ReturnsNotFoundAsJson()
        {
            var client = _testServer.CreateClient();
            client.DefaultRequestHeaders.Add(HttpDefaults.HeaderAcceptKey, HttpDefaults.ContentTypeJson);

            var response = await client.GetAsync($"{SharedDefaults.ApplicationUrl}/documents/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var error = JsonSerializer.Deserialize<BaseResponse>(body);

            Assert.NotNull(error);
            Assert.NotEqual(0, error.ResultCode);
        }

        [Fact]
        public async Task PostDuplicateDocument_ReturnsConflict()
        {
            var client = _testServer.CreateClient();
            var document = NewDocument();

            var first = await client.PostAsync($"{SharedDefaults.ApplicationUrl}/documents", JsonDocument(document));
            Assert.Equal(HttpStatusCode.OK, first.StatusCode);

            var second = await client.PostAsync($"{SharedDefaults.ApplicationUrl}/documents", JsonDocument(document));
            Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        }

        [Fact]
        public async Task PutNonExistingDocument_ReturnsNotFound()
        {
            var client = _testServer.CreateClient();

            var response = await client.PutAsync($"{SharedDefaults.ApplicationUrl}/documents", JsonDocument(NewDocument()));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetNonExistingDocument_ReturnsNotFoundAsXml()
        {
            var client = _testServer.CreateClient();
            client.DefaultRequestHeaders.Add(HttpDefaults.HeaderAcceptKey, HttpDefaults.ContentTypeXml);

            var response = await client.GetAsync($"{SharedDefaults.ApplicationUrl}/documents/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal(HttpDefaults.ContentTypeXml, response.Content.Headers.ContentType?.MediaType);

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var serializer = new DataContractSerializer(typeof(BaseResponse));
            var reader = XmlDictionaryReader.CreateTextReader(bytes, new XmlDictionaryReaderQuotas());
            var error = serializer.ReadObject(reader) as BaseResponse;
            reader.Close();

            Assert.NotNull(error);
            Assert.NotEqual(0, error.ResultCode);
        }

        [Fact]
        public async Task GetNonExistingDocument_ReturnsNotFoundAsMessagePack()
        {
            var client = _testServer.CreateClient();
            client.DefaultRequestHeaders.Add(HttpDefaults.HeaderAcceptKey, HttpDefaults.ContentTypeMessagePack);

            var response = await client.GetAsync($"{SharedDefaults.ApplicationUrl}/documents/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // Validates that the error body is written as raw MessagePack bytes (not mangled through UTF-8).
            var bytes = await response.Content.ReadAsByteArrayAsync();
            var error = MessagePackSerializer.Deserialize<BaseResponse>(bytes);

            Assert.NotNull(error);
            Assert.NotEqual(0, error.ResultCode);
        }
    }
}

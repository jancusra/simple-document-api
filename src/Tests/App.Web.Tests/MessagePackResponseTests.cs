using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MessagePack;
using App.Contracts.Models;

namespace App.Web.Tests
{
    /// <summary>
    /// Partial class representing the integration test (API response in MessagePack format)
    /// </summary>
    public partial class ApiIntegrationTests
    {
        [Fact]
        public async Task SaveAndGetDocumentInMessagePackFormat()
        {
            var client = _testServer.CreateClient();

            Guid documentId = Guid.NewGuid();
            var document = new DocumentDto
            {
                id = documentId,
                tags = ["x-msgpack", "integration-test"],
                data = new Dictionary<string, string>() {
                    { "field1", "x-msgpack" },
                    { "field2", "integration" }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(document), Encoding.UTF8, SharedDefaults.ContentTypeJson);
            var saveResponse = await client.PostAsync($"{SharedDefaults.ApplicationUrl}/documents", content);

            Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);

            client.DefaultRequestHeaders.Add(SharedDefaults.HeaderAcceptKey, SharedDefaults.ContentTypeMessagePack);
            var getResponse = await client.GetAsync($"{SharedDefaults.ApplicationUrl}/documents/{documentId}");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getDataBytes = await getResponse.Content.ReadAsByteArrayAsync();
            var model = MessagePackSerializer.Deserialize<DocumentDto>(getDataBytes);

            Assert.Equal(documentId.ToString().ToLower(), model?.id.ToString().ToLower());
            Assert.Equal(document.tags[0], model?.tags[0]);
            Assert.Equal(document.data["field2"], model?.data["field2"]);
        }
    }
}
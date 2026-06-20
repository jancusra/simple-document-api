using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using App.Contracts.Models;
using App.Domain;

namespace App.Web.Tests
{
    /// <summary>
    /// Partial class representing integration tests for the arbitrary (schema-free) data field
    /// </summary>
    public partial class ApiIntegrationTests
    {
        private static DocumentDto ArbitraryDocument(Guid id)
        {
            // Mirrors the assignment example and adds nested objects, arrays, numbers and booleans
            // to prove the data schema is truly arbitrary.
            return new DocumentDto
            {
                id = id,
                tags = ["important", ".net"],
                data = new JsonObject
                {
                    ["some"] = "data",
                    ["optional"] = "fields",
                    ["nested"] = new JsonObject
                    {
                        ["author"] = new JsonObject { ["name"] = "X", ["age"] = 30 },
                        ["scores"] = new JsonArray(1, 2, 3)
                    },
                    ["active"] = true
                }
            };
        }

        [Fact]
        public async Task SaveAndGetArbitraryNestedData_InJsonFormat()
        {
            var client = _testServer.CreateClient();
            var documentId = Guid.NewGuid();
            var document = ArbitraryDocument(documentId);

            var content = new StringContent(JsonSerializer.Serialize(document), Encoding.UTF8, HttpDefaults.ContentTypeJson);
            var saveResponse = await client.PostAsync($"{SharedDefaults.ApplicationUrl}/documents", content);
            Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);

            client.DefaultRequestHeaders.Add(HttpDefaults.HeaderAcceptKey, HttpDefaults.ContentTypeJson);
            var getResponse = await client.GetAsync($"{SharedDefaults.ApplicationUrl}/documents/{documentId}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getData = await getResponse.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<DocumentDto>(getData);

            Assert.NotNull(model?.data);
            Assert.Equal("data", model.data["some"].GetValue<string>());
            Assert.Equal("X", model.data["nested"]["author"]["name"].GetValue<string>());
            Assert.Equal(30, model.data["nested"]["author"]["age"].GetValue<int>());
            Assert.Equal(3, model.data["nested"]["scores"].AsArray().Count);
            Assert.True(model.data["active"].GetValue<bool>());
        }

        [Fact]
        public async Task SaveAndGetArbitraryNestedData_InXmlFormat()
        {
            var client = _testServer.CreateClient();
            var documentId = Guid.NewGuid();
            var document = ArbitraryDocument(documentId);

            var content = new StringContent(JsonSerializer.Serialize(document), Encoding.UTF8, HttpDefaults.ContentTypeJson);
            var saveResponse = await client.PostAsync($"{SharedDefaults.ApplicationUrl}/documents", content);
            Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);

            client.DefaultRequestHeaders.Add(HttpDefaults.HeaderAcceptKey, HttpDefaults.ContentTypeXml);
            var getResponse = await client.GetAsync($"{SharedDefaults.ApplicationUrl}/documents/{documentId}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getDataBytes = await getResponse.Content.ReadAsByteArrayAsync();
            var serializer = new DataContractSerializer(typeof(DocumentDto));
            var reader = XmlDictionaryReader.CreateTextReader(getDataBytes, new XmlDictionaryReaderQuotas());
            var model = serializer.ReadObject(reader) as DocumentDto;
            reader.Close();

            // Across XML the arbitrary data survives as a JSON-encoded value; the structure is intact.
            Assert.NotNull(model?.data);
            Assert.Equal(30, model.data["nested"]["author"]["age"].GetValue<int>());
            Assert.True(model.data["active"].GetValue<bool>());
        }

        [Fact]
        public async Task PostDocumentWithoutMandatoryData_ReturnsBadRequest()
        {
            var client = _testServer.CreateClient();

            // Missing the mandatory "data" field.
            var payload = new JsonObject
            {
                ["id"] = Guid.NewGuid().ToString(),
                ["tags"] = new JsonArray("only-tags")
            };

            var content = new StringContent(payload.ToJsonString(), Encoding.UTF8, HttpDefaults.ContentTypeJson);
            var response = await client.PostAsync($"{SharedDefaults.ApplicationUrl}/documents", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

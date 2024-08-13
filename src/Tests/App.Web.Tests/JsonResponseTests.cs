using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using App.Contracts.Models;
using App.Domain;

namespace App.Web.Tests
{
    /// <summary>
    /// Partial class representing the integration test (API response in JSON format)
    /// </summary>
    public partial class ApiIntegrationTests : IDisposable
    {
        private readonly TestServer _testServer;

        public ApiIntegrationTests()
        {
            var webBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder.AddJsonFile(SharedDefaults.ConfigurationFile);
                })
                .UseStartup<Startup>();

            _testServer = new TestServer(webBuilder);
        }

        [Fact]
        public async Task SaveAndGetDocumentInJsonFormat()
        {
            var client = _testServer.CreateClient();

            // prepare a test identifier and document
            Guid documentId = Guid.NewGuid();
            var document = new DocumentDto
            {
                id = documentId,
                tags = ["json", "integration-test"],
                data = new Dictionary<string, string>() { 
                    { "field1", "json" }, 
                    { "field2", "integration" }
                }
            };

            // create a test document using the POST api method
            var content = new StringContent(JsonSerializer.Serialize(document), Encoding.UTF8, HttpDefaults.ContentTypeJson);
            var saveResponse = await client.PostAsync($"{SharedDefaults.ApplicationUrl}/documents", content);

            Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);

            // retrieve a test document by a known identifier
            client.DefaultRequestHeaders.Add(HttpDefaults.HeaderAcceptKey, HttpDefaults.ContentTypeJson);
            var getResponse = await client.GetAsync($"{SharedDefaults.ApplicationUrl}/documents/{documentId}");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            // parse the GET response and check the results
            var getData = await getResponse.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<DocumentDto>(getData);

            Assert.Equal(documentId.ToString().ToLower(), model?.id.ToString().ToLower());
            Assert.Equal(document.tags[0], model?.tags[0]);
            Assert.Equal(document.data["field2"], model?.data["field2"]);
        }

        public void Dispose()
        {
            _testServer.Dispose();
        }
    }
}
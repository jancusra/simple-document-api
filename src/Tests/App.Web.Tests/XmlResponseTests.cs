using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using App.Contracts.Models;

namespace App.Web.Tests
{
    public partial class ApiIntegrationTests
    {
        [Fact]
        public async Task SaveAndGetDocumentInXmlFormat()
        {
            var client = _testServer.CreateClient();

            Guid documentId = Guid.NewGuid();
            var document = new DocumentDto
            {
                id = documentId,
                tags = ["xml", "integration-test"],
                data = new Dictionary<string, string>() { 
                    { "field1", "xml" }, 
                    { "field2", "integration" }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(document), Encoding.UTF8, SharedDefaults.ContentTypeJson);
            var saveResponse = await client.PostAsync($"{SharedDefaults.ApplicationUrl}/documents", content);

            Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);

            client.DefaultRequestHeaders.Add(SharedDefaults.HeaderAcceptKey, SharedDefaults.ContentTypeXml);
            var getResponse = await client.GetAsync($"{SharedDefaults.ApplicationUrl}/documents/{documentId}");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getDataBytes = await getResponse.Content.ReadAsByteArrayAsync();

            DataContractSerializer dcs = new DataContractSerializer(typeof(DocumentDto));
            var reader = XmlDictionaryReader.CreateTextReader(getDataBytes, new XmlDictionaryReaderQuotas());

            DocumentDto? model = dcs.ReadObject(reader) as DocumentDto;
            reader.Close();

            Assert.Equal(documentId.ToString().ToLower(), model?.id.ToString().ToLower());
            Assert.Equal(document.tags[0], model?.tags[0]);
            Assert.Equal(document.data["field2"], model?.data["field2"]);
        }
    }
}
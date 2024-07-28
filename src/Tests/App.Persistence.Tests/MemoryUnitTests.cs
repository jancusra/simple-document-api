using System;
using System.Threading.Tasks;
using MemoryCache.Testing.Moq;
using Moq;
using App.Domain.Entities;
using App.Persistence.Memory;

namespace App.Persistence.Tests
{
    public class MemoryUnitTests
    {
        [Fact]
        public async Task SaveAndGetDocumentFromMemory()
        {
            Guid documentId = Guid.NewGuid();

            var document = new Document
            {
                Id = documentId,
                Value = "Some string text value"
            };

            var mockedCache = Create.MockedMemoryCache();
            var memoryProvider = new Mock<MemoryDataProvider>(mockedCache)
            {
                CallBase = true
            };

            await memoryProvider.Object.InsertDocumentAsync(document);

            var result = await memoryProvider.Object.GetDocumentByIdAsync<Document>(documentId);

            Assert.Equal(result.Id, document.Id);
            Assert.Equal(result.Value, document.Value);
        }

        [Fact]
        public async Task SaveUpdateAndGetDocumentFromMemory()
        {
            Guid documentId = Guid.NewGuid();

            var document = new Document
            {
                Id = documentId,
                Value = "Some new document"
            };

            var mockedCache = Create.MockedMemoryCache();
            var memoryProvider = new Mock<MemoryDataProvider>(mockedCache)
            {
                CallBase = true
            };

            await memoryProvider.Object.InsertDocumentAsync(document);

            document.Value = "Updated document value";

            await memoryProvider.Object.UpdateDocumentAsync(document);

            var result = await memoryProvider.Object.GetDocumentByIdAsync<Document>(documentId);

            Assert.Equal(result.Id, document.Id);
            Assert.Equal(result.Value, document.Value);
        }
    }
}
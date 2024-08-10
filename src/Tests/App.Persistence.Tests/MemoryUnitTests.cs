using System;
using Microsoft.Extensions.Caching.Memory;
using App.Domain.Entities;

namespace App.Persistence.Tests
{
    /// <summary>
    /// Examples of unit tests for an memory cache
    /// </summary>
    public class MemoryUnitTests : MemoryTestsBase
    {
        [Fact]
        public void SaveAndGetDocumentFromMemory()
        {
            Guid documentId = Guid.NewGuid();

            var document = new Document
            {
                Id = documentId,
                Value = "Some string text value"
            };

            MemoryCache.Set(documentId, document);

            var result = MemoryCache.Get<Document>(documentId);

            Assert.Equal(result.Id, document.Id);
            Assert.Equal(result.Value, document.Value);
        }

        [Fact]
        public void SaveUpdateAndGetDocumentFromMemory()
        {
            Guid documentId = Guid.NewGuid();

            var document = new Document
            {
                Id = documentId,
                Value = "Some new document"
            };

            MemoryCache.Set(documentId, document);

            document.Value = "Updated document value";

            MemoryCache.Set(documentId, document);

            var result = MemoryCache.Get<Document>(documentId);

            Assert.Equal(result.Id, document.Id);
            Assert.Equal(result.Value, document.Value);
        }
    }
}
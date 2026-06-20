using System;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Entities;
using App.Domain.Exceptions;
using App.Persistence.Memory;

namespace App.Persistence.Tests
{
    /// <summary>
    /// Unit tests for the in-memory data provider (add/get/update guards)
    /// </summary>
    public class MemoryDataProviderTests : MemoryTestsBase
    {
        private readonly MemoryDataProvider _provider;

        public MemoryDataProviderTests()
        {
            _provider = new MemoryDataProvider(MemoryCache);
        }

        [Fact]
        public async Task AddThenGet_ReturnsStoredDocument()
        {
            var document = new Document { Id = Guid.NewGuid(), Value = "stored value" };

            await _provider.AddDocumentAsync(document);
            var result = await _provider.GetDocumentByIdAsync<Document>(document.Id);

            Assert.NotNull(result);
            Assert.Equal(document.Id, result.Id);
            Assert.Equal(document.Value, result.Value);
        }

        [Fact]
        public async Task Get_NonExistingDocument_ReturnsNull()
        {
            var result = await _provider.GetDocumentByIdAsync<Document>(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task Add_DuplicateDocument_ThrowsAlreadyExists()
        {
            var document = new Document { Id = Guid.NewGuid(), Value = "first" };

            await _provider.AddDocumentAsync(document);

            await Assert.ThrowsAsync<EntityEntryAlreadyExistsException>(
                () => _provider.AddDocumentAsync(document));
        }

        [Fact]
        public async Task Update_ExistingDocument_ModifiesValue()
        {
            var document = new Document { Id = Guid.NewGuid(), Value = "original" };
            await _provider.AddDocumentAsync(document);

            document.Value = "updated";
            await _provider.UpdateDocumentAsync(document);

            var result = await _provider.GetDocumentByIdAsync<Document>(document.Id);

            Assert.Equal("updated", result.Value);
        }

        [Fact]
        public async Task Update_NonExistingDocument_ThrowsNonExisting()
        {
            var document = new Document { Id = Guid.NewGuid(), Value = "ghost" };

            await Assert.ThrowsAsync<NonExistingEntityEntryException>(
                () => _provider.UpdateDocumentAsync(document));
        }

        [Fact]
        public async Task ConcurrentAdd_SameId_OnlyOneSucceeds()
        {
            var id = Guid.NewGuid();
            const int attempts = 20;

            var tasks = Enumerable.Range(0, attempts).Select(_ => Task.Run(async () =>
            {
                try
                {
                    await _provider.AddDocumentAsync(new Document { Id = id, Value = "value" });
                    return true;
                }
                catch (EntityEntryAlreadyExistsException)
                {
                    return false;
                }
            }));

            var results = await Task.WhenAll(tasks);

            // Exactly one concurrent add must win; the rest must be rejected as duplicates.
            Assert.Equal(1, results.Count(succeeded => succeeded));
        }
    }
}

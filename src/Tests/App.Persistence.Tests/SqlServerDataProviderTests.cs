using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using App.Domain.Entities;
using App.Domain.Exceptions;
using App.Persistence.Database;

namespace App.Persistence.Tests
{
    /// <summary>
    /// Unit tests for the SQL Server data provider, backed by the EF Core in-memory provider
    /// so the provider logic (insert/get/update guards) can be verified without a real database.
    /// Each operation uses a fresh context over a shared store to mimic per-request scoped contexts.
    /// </summary>
    public class SqlServerDataProviderTests
    {
        private readonly InMemoryDatabaseRoot _databaseRoot = new InMemoryDatabaseRoot();

        private readonly string _databaseName = Guid.NewGuid().ToString();

        private SqlServerDataProvider CreateProvider()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(_databaseName, _databaseRoot)
                .Options;

            return new SqlServerDataProvider(new AppDbContext(options));
        }

        [Fact]
        public async Task AddThenGet_ReturnsStoredDocument()
        {
            var document = new Document { Id = Guid.NewGuid(), Value = "stored value" };

            await CreateProvider().AddDocumentAsync(document);
            var result = await CreateProvider().GetDocumentByIdAsync<Document>(document.Id);

            Assert.NotNull(result);
            Assert.Equal(document.Id, result.Id);
            Assert.Equal(document.Value, result.Value);
        }

        [Fact]
        public async Task Get_NonExistingDocument_ReturnsNull()
        {
            var result = await CreateProvider().GetDocumentByIdAsync<Document>(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task Add_DuplicateDocument_ThrowsAlreadyExists()
        {
            var document = new Document { Id = Guid.NewGuid(), Value = "first" };

            await CreateProvider().AddDocumentAsync(document);

            await Assert.ThrowsAsync<EntityEntryAlreadyExistsException>(
                () => CreateProvider().AddDocumentAsync(document));
        }

        [Fact]
        public async Task Update_ExistingDocument_ModifiesValue()
        {
            var document = new Document { Id = Guid.NewGuid(), Value = "original" };
            await CreateProvider().AddDocumentAsync(document);

            await CreateProvider().UpdateDocumentAsync(new Document { Id = document.Id, Value = "updated" });

            var result = await CreateProvider().GetDocumentByIdAsync<Document>(document.Id);

            Assert.Equal("updated", result.Value);
        }

        [Fact]
        public async Task Update_NonExistingDocument_ThrowsNonExisting()
        {
            var document = new Document { Id = Guid.NewGuid(), Value = "ghost" };

            await Assert.ThrowsAsync<NonExistingEntityEntryException>(
                () => CreateProvider().UpdateDocumentAsync(document));
        }
    }
}

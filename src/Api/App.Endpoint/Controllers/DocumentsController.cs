using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App.Contracts.Models;
using App.Domain.Entities;
using App.Domain.Exceptions;
using App.Mapper;
using App.Persistence.DataProvider;

namespace Rat.Endpoint.Controllers
{
    [ApiController]
    public partial class DocumentsController : ControllerBase
    {
        private readonly IDataProvider _dataProvider;

        public DocumentsController(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Get document by ID
        /// </summary>
        /// <param name="id">document ID</param>
        /// <returns>final document in DTO model</returns>
        [Route("[controller]/{id}")]
        [HttpGet]
        public virtual async Task<IActionResult> Get(Guid id)
        {
            var result = await _dataProvider.GetDocumentByIdAsync<Document>(id);

            if (result == null)
            {
                throw new NonExistingEntityEntryException(nameof(Document), id);
            }

            return Ok(result.ToDtoModel());
        }

        /// <summary>
        /// Add new document
        /// </summary>
        /// <param name="model">document to add in DTO</param>
        /// <returns>OK result</returns>
        [Route("[controller]")]
        [HttpPost]
        public virtual async Task<IActionResult> AddDocument([FromBody]DocumentDto model)
        {
            await _dataProvider.AddDocumentAsync(model.ToEntity());

            return Ok();
        }

        /// <summary>
        /// Update existing document
        /// </summary>
        /// <param name="model">document to update in DTO</param>
        /// <returns>OK result</returns>
        [Route("[controller]")]
        [HttpPut]
        public virtual async Task<IActionResult> UpdateDocument([FromBody]DocumentDto model)
        {
            await _dataProvider.UpdateDocumentAsync(model.ToEntity());

            return Ok();
        }
    }
}

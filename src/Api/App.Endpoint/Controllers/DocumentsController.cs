using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App.Contracts.Models;
using App.Domain.Entities;
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

        [Route("[controller]/{id}")]
        [HttpGet]
        public virtual async Task<IActionResult> Get(Guid id)
        {
            var result = await _dataProvider.GetDocumentByIdAsync<Document>(id);

            return Ok(result.ToDtoModel());
        }

        [Route("[controller]")]
        [HttpPost]
        public virtual async Task<IActionResult> AddDocument([FromBody]DocumentDto model)
        {
            await _dataProvider.AddDocumentAsync(model.ToEntity());

            return Ok();
        }

        [Route("[controller]")]
        [HttpPut]
        public virtual async Task<IActionResult> UpdateDocument([FromBody]DocumentDto model)
        {
            await _dataProvider.UpdateDocumentAsync(model.ToEntity());

            return Ok();
        }
    }
}

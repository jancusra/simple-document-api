using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App.Domain.Entities;
using App.Persistence;

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

            return Ok(JsonSerializer.Deserialize<DocumentDto>(result.Value));
        }

        [Route("[controller]")]
        [HttpPost]
        public virtual async Task<IActionResult> AddDocument([FromBody]DocumentDto model)
        {
            await _dataProvider.AddDocumentAsync(new Document { Id = model.id, Value = JsonSerializer.Serialize(model) });

            return Ok();
        }

        [Route("[controller]")]
        [HttpPut]
        public virtual async Task<IActionResult> UpdateDocument([FromBody] DocumentDto model)
        {
            await _dataProvider.UpdateDocumentAsync(new Document { Id = model.id, Value = JsonSerializer.Serialize(model) });

            return Ok();
        }

        public class DocumentDto
        {
            public Guid id { get; set; }

            public string[] tags { get; set; }

            public object data { get; set; }
        }
    }
}

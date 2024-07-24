using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Rat.Endpoint.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class HomeController : ControllerBase
    {
        /// <summary>
        /// Default starting api method (verify running app)
        /// </summary>
        /// <returns>array of strings</returns>
        [HttpGet]
        public virtual IEnumerable<string> Get()
        {
            return new string[] { "API", "app", "is", "running" };
        }
    }
}

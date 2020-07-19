using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SwaggerUsage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("Working too.");
        }
    }
}

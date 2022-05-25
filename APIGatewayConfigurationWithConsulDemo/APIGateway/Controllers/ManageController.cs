using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
    /// <summary>
    /// Ocelot Manager
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ManageController : ControllerBase
    {
        [HttpGet("Test")]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }
    }
}

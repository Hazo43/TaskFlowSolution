using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
 
    public class TestController : BaseApiController
    {
        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("JWT is working");
        }

    }
}

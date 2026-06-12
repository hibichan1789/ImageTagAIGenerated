using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImageTagApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        [HttpGet("public")]
        public ActionResult Public()
        {
            return Ok("Hello World (Public)");
        }

        [Authorize]
        [HttpGet("private")]
        public ActionResult Private()
        {
            return Ok("Hello World (Private)");
        }
    }
}

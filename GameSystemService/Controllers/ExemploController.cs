using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameSystemService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExemploController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public ActionResult GetExemplo(){
            return Ok("asdasdasd");
        }
    }
}
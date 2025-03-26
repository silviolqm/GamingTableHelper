using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameSystemService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExemploController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetExemplo(){
            return Ok("asdasdasd");
        }
    }
}
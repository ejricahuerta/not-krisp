using Microsoft.AspNetCore.Mvc;

namespace NotKrisp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleError(Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
} 
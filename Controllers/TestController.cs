using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace projfour.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok(new
            {
                message = "Anyone can access this endpoint"
            });
        }


        [Authorize]
        [HttpGet("authorized")]
        public IActionResult Authorized()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                message = "You are authenticated",
                username,
                role
            });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult Admin()
        {
            return Ok(new
            {
                message = "Only Admin can access this endpoint"
            });
        }



        [Authorize(Roles = "Employee")]
        [HttpGet("employee")]
        public IActionResult Employee()
        {
            return Ok(new
            {
                message = "Only Employee can access this endpoint"
            });
        }



    }
}

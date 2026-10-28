using Microsoft.AspNetCore.Mvc;
using projfour.Models;
using projfour.Service;

namespace projfour.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }



        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            if (request == null) 
            {
                return BadRequest(new {
                    message = "This request body cannot be null"
                });
            }

            var result = _authService.Login(request);

            if (result == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password"
                });
            }

            return Ok(result);
        }



        [HttpPost("refresh")]
        public IActionResult Refresh(RefreshRequest request)
        {
            var result = _authService.RefreshToken(request.RefreshToken);

            if (result == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid refresh token"
                });
            }

            return Ok(result);
        } 







        public IActionResult Index()
        {
            return View();
        }
    }
}

using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    // ==========================
    // Users Controller
    // ==========================

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // ==========================
        // Register
        // ==========================

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _userService.RegisterAsync(dto);

            if (result == "EMAIL_EXISTS")
                return BadRequest(new
                {
                    message = "An account with this email already exists."
                });

            return Ok(new
            {
                message = "Registration successful."
            });
        }

        // ==========================
        // Login
        // ==========================

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userService.LoginAsync(dto);

            if (user == null)
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.IsAdmin
            });
        }
    }
}
using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;

namespace BookApp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthBL _authBL;
        private readonly IForgotPasswordService _forgotPasswordService;

        public AuthController(IAuthBL authBL, IForgotPasswordService forgotPasswordService)
        {
            _authBL = authBL;
            _forgotPasswordService = forgotPasswordService;
        }

        // POST /api/auth/register → Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            var result = await _authBL.RegisterAsync(registerDto);
            if (result == "User already exists")
                return BadRequest(new { message = result });

            return Ok(new { token = result });
        }

        // POST /api/auth/login → Login user
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            var token = await _authBL.LoginAsync(loginDto);
            if (token == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { token });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO dto)
        {
            var result = await _forgotPasswordService.ForgotPasswordAsync(dto);
            if (!result) return BadRequest(new { message = "Email not found" });

            return Ok(new { message = "Reset password email sent" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            var result = await _forgotPasswordService.ResetPasswordAsync(dto);
            if (!result) return BadRequest(new { message = "Invalid token or expired" });

            return Ok(new { message = "Password reset successful" });
        }
    }
}

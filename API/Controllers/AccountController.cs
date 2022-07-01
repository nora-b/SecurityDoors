using System.Security.Claims;
using API.DTOs;
using API.Services;
using API.Services.Commands.Users;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountController(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, TokenService tokenService) : base(httpContextAccessor, tokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            return HandleResult(await Mediator.Send(new Login.Command { User = loginDto }));
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            return HandleResult(await Mediator.Send(new Register.Command { User = registerDto }));
        }

        [Authorize]
        [HttpPost("currentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            return HandleResult(await Mediator.Send(new GetCurrentUserInfo.Query { Username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email) != null ? _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value :string.Empty }));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {
            return HandleResult(await Mediator.Send(new Logout.Query { Token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "")}));
        }
    }
}
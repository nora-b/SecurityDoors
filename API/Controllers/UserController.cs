using System.Security.Claims;
using API.Services;
using API.Services.Commands.Office;
using API.Services.Commands.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseApiController
    {
        public UserController(IHttpContextAccessor httpContextAccessor, TokenService tokenService) : base(httpContextAccessor, tokenService)
        {
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpGet("userHistory")]
        public async Task<IActionResult> GetUsersHistory()
        {
            return HandleResult(await Mediator.Send(new GetUsersHistory.Query { }));
        }

    }
}
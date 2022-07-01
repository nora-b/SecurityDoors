using System.Security.Claims;
using API.DTOs;
using API.Services;
using API.Services.Commands.Office;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfficeController : BaseApiController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OfficeController(IHttpContextAccessor httpContextAccessor, TokenService tokenService) : base(httpContextAccessor, tokenService)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Policy = "IsAuthorizedInOffice")]
        [HttpPost("enter")]
        public async Task<IActionResult> EnterOffice()
        {
            return HandleResult(await Mediator.Send(new EnterOffice.Command { Username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value }));
        }

        [Authorize(Policy = "IsAuthorizedInOffice")]
        [HttpPost("leave")]
        public async Task<IActionResult> LeaveOffice()
        {
            return HandleResult(await Mediator.Send(new LeaveOffice.Command { Username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value }));
        }
    }
}
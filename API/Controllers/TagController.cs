using API.DTOs;
using API.Services;
using API.Services.Commands.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : BaseApiController
    {
        public TagController(IHttpContextAccessor httpContextAccessor, TokenService tokenService) : base(httpContextAccessor, tokenService)
        {
        }
        [Authorize(Policy = "IsAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            return HandleResult(await Mediator.Send(new RetrieveAllTags.Query()));
        }
        [Authorize(Policy ="IsAdmin")]
        [HttpPut]
        public async Task<IActionResult> UpdateTag(UpdateTagDto tagDto)
        {
            return HandleResult(await Mediator.Send(new UpdateTag.Command{Tag = tagDto, Username = tagDto.Username}));
        }
        
    }
}
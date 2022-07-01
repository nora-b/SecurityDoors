using API.Constants;
using API.DTOs;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Persistence;

namespace API.Services.Commands.Tags
{
    public class UpdateTag
    {
        public class Command : IRequest<Result<TagDto>>
        {
            public UpdateTagDto Tag { get; set; }
            public string Username { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<TagDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly UserManager<User> _userManager;
            public Handler(DataContext context, IMapper mapper, UserManager<User> userManager)
            {
                _userManager = userManager;
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<TagDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(request.Username.ToLower());
                
                if (user == null) return Result<TagDto>.Failure("The user you want to update doesn't exists");

                Tag userTag = await _context.Tags.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (userTag == null) return Result<TagDto>.Failure("Doesn't exists tag for this user");

                userTag.IsAuthorizedTunnel = request.Tag.AuthorizeInTunnel;

                userTag.IsAuthorizedOffice = request.Tag.AuthorizeInOffice;

                //the case for the tunnel
                if (request.Tag.AuthorizeInTunnel is true && TagStatus.Pending == userTag.StatusTunnel)
                {
                    userTag.StatusTunnel = TagStatus.Active;
                }
                //the case for the office
                if (request.Tag.AuthorizeInOffice is true && TagStatus.Pending == userTag.StatusOffice)
                {
                    userTag.StatusOffice = TagStatus.Active;
                    //If admin gives authorization to enter the office, by default he/she should have access also to enter the tunnel
                    userTag.StatusTunnel = TagStatus.Active;
                }

                if (userTag.TagTunnelExpiresAt <= DateTimeOffset.UtcNow)
                {
                    userTag.StatusTunnel = TagStatus.Expired;
                    //if the tag tunnel expires by default also the office tag will be expired
                    userTag.StatusOffice = TagStatus.Expired;
                }
                else
                {
                    userTag.StatusTunnel = request.Tag.TagStatusInTunnel ?? userTag.StatusTunnel;
                }

                //the case for the office
                if (userTag.TagOfficeExpiresAt <= DateTimeOffset.UtcNow)
                {
                    userTag.StatusOffice = TagStatus.Expired;
                }
                else
                {
                    userTag.StatusOffice = request.Tag.TagStatusInOffice ?? userTag.StatusOffice;
                }

                _context.Tags.Update(userTag);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<TagDto>.Failure("Failed to update tag");

                TagDto response = new TagDto();

                _mapper.Map(userTag, response);

                return Result<TagDto>.Success(response);
            }
        }

    }
}
using API.Constants;
using API.DTOs;
using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Services.Commands.Users
{
    public class Register
    {
        public class Command : IRequest<Result<UserDto>>
        {
            public RegisterDto User { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<UserDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly UserManager<User> _userManager;
            private readonly TokenService _tokenService;
            public Handler(DataContext context, IMapper mapper, UserManager<User> userManager, TokenService tokenService)
            {
                _tokenService = tokenService;
                _userManager = userManager;
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<UserDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await _userManager.Users.AnyAsync(x => x.UserName == request.User.Username))
                {
                    return Result<UserDto>.Failure("Username taken");
                }

                var user = new User
                {
                    FirstName = request.User.FirstName,
                    LastName = request.User.LastName,
                    UserName = request.User.Username.ToLower()
                };
                //user.NormalizedUserName = request.User.Username.ToUpper();

                var userAdded= await _userManager.CreateAsync(user, request.User.Password);

                // await _context.Users.AddAsync(user);
                
                await _context.SaveChangesAsync();
                //adding the role for the user
                await _userManager.AddToRoleAsync(user, request.User.Role);

                var tag = new TagDto();

                Tag newTag = AddNewTag(user.Id, request.User.Role);

                _mapper.Map(newTag, tag);

                await _context.Tags.AddAsync(newTag);

                string token = _tokenService.CreateToken(user, request.User.Role, tag);

                //adding the user history
                var userHistory = new UserHistory {
                    UserId = user.Id,
                    LastLoggedInOffice = null
                };
                
                _context.UserHistories.Add(userHistory);
                
                var result = await _context.SaveChangesAsync() > 0;

                var response = new UserDto 
                {
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    InOffice = user.InOffice,
                    Token = token
                };

                if (!result) return Result<UserDto>.Failure("Failed to create user");

                return Result<UserDto>.Success(response);
            }

            private static Tag AddNewTag(Guid userId, string userRole)
            {
                int expirationYears = 0;

                if (userRole == AppConstants.AdminRoleName)
                {
                    expirationYears = 3;
                }
                else if (userRole == AppConstants.EmployeeRoleName)
                {
                    expirationYears = 1;
                }
                var newTag = new Tag
                {
                    UserId = userId,
                    StatusOffice = TagStatus.Pending,
                    IsAuthorizedOffice = false,
                    StatusTunnel = TagStatus.Pending,
                    IsAuthorizedTunnel = false
                };
                //this is the condition for the expiration time if the user has guest role it will have a validity for 1 hour, else based on if condition above it will have the specified validity of users
                newTag.TagOfficeExpiresAt = expirationYears > 0 ? DateTimeOffset.UtcNow.AddYears(expirationYears) : DateTimeOffset.UtcNow.AddHours(1);
                newTag.TagTunnelExpiresAt = expirationYears > 0 ? DateTimeOffset.UtcNow.AddYears(expirationYears) : DateTimeOffset.UtcNow.AddHours(1);
                //the code will have a unique  value, it will be a combination of userId and a generated guid
                newTag.Code = $"{userId}-{Guid.NewGuid()}";

                return newTag;

            }


        }
    }
}
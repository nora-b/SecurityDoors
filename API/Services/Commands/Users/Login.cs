using API.DTOs;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Services.Commands.Users
{
    public class Login
    {
        public class Command : IRequest<Result<UserDto>>
        {
            public LoginDto User { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<UserDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly UserManager<User> _userManager;
            private readonly TokenService _tokenService;
            private readonly SignInManager<User> _signInManager;
            public Handler(DataContext context, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, TokenService tokenService)
            {
                _signInManager = signInManager;
                _tokenService = tokenService;
                _userManager = userManager;
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<UserDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(request.User.Username);

                if (user == null) return Result<UserDto>.Failure("Username is incorrect");

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.User.Password, false);

                if (!result.Succeeded) return Result<UserDto>.Failure("Password is incorrect");

                var roles = await _userManager.GetRolesAsync(user);

                var userRole = roles.Any() ? roles.First() : string.Empty;

                var tag = new TagDto();

                if (!string.IsNullOrWhiteSpace(userRole))
                {
                     var getTag = await _context.Tags.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    _mapper.Map(getTag, tag);
                }

                //update the userHistory table 
                var userHistory = await _context.UserHistories.FirstOrDefaultAsync(x => x.UserId == user.Id);
                if (userHistory != null){
                    userHistory.LastLoggedInOffice = DateTime.Now;
                    _context.UserHistories.Update(userHistory);
                    await _context.SaveChangesAsync();

                }
                
                if (result.Succeeded)
                {
                    var response = new UserDto
                    {
                        Username = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Token = _tokenService.CreateToken(user, userRole, tag)
                    };
                    return Result<UserDto>.Success(response);
                }

                return Result<UserDto>.Failure("Failed to login");
            }
        }
    
    }
}
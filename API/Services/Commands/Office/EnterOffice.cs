using API.DTOs;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence;

namespace API.Services.Commands.Office
{
    public class EnterOffice
    {
        public class Command : IRequest<Result<UserDto>>
        {
            public string Username { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<UserDto>>
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

            public async Task<Result<UserDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                
                if (user == null) return Result<UserDto>.Failure("The user does not exists!");

                user.InOffice = true;

                await _userManager.UpdateAsync(user);

                _context.Users.Update(user);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<UserDto>.Failure("Failed to enter the office");

                UserDto response = new UserDto();

                _mapper.Map(user, response);

                return Result<UserDto>.Success(response);
            }
        }

    }
}
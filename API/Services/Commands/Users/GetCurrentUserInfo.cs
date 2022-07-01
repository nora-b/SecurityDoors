using API.DTOs;
using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Services.Commands.Users
{
    public class GetCurrentUserInfo
    {
        public class Query : IRequest<Result<UserDto>> 
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<UserDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<UserDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                UserDto result = new UserDto();

                var user = await _context.Users.FirstOrDefaultAsync(x=> x.UserName == request.Username);

                if (user == null) return Result<UserDto>.Failure("The user doesn't exists or you have to login");
                
                _mapper.Map(user,result);

                return Result<UserDto>.Success(result);
            }
        }

    }
}
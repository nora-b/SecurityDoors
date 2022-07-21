using API.DTOs;
using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Services.Commands.Users
{
    public class GetUsersHistory
    {
         public class Query : IRequest<Result<List<UsersHistoryDto>>> 
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<UsersHistoryDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<List<UsersHistoryDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userHistory = await _context.UserHistories.Include(x => x.User).ToListAsync();
                var history = userHistory.Select(x => new UsersHistoryDto {Username= x.User.UserName, LastLoggedIn = x.LastLoggedInOffice}).ToList();

                List<UsersHistoryDto> result = new List<UsersHistoryDto>();
               _mapper.Map(history, result);

                return Result<List<UsersHistoryDto>>.Success(result);
            }
        }

    }
}
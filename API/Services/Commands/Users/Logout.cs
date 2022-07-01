using API.DTOs;
using Application.Core;
using MediatR;

namespace API.Services.Commands.Users
{
    public class Logout
    {
        public class Query : IRequest<Result<TokenDto>> 
        {
            public string Token { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<TokenDto>>
        {
            public async Task<Result<TokenDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                TokenDto result = new TokenDto();
                result.Token = request.Token;

                return Result<TokenDto>.Success(result);
            }
        }

    }
}
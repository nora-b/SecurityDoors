using API.DTOs;
using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Services.Commands.Tags
{
    public class RetrieveAllTags
    {
            public class Query : IRequest<Result<List<TagDto>>> { }

            public class Handler : IRequestHandler<Query, Result<List<TagDto>>>
            {
                private readonly DataContext _context;
                private readonly IMapper _mapper;
                public Handler(DataContext context, IMapper mapper)
                {
                    _mapper = mapper;
                    _context = context;
                }

                public async Task<Result<List<TagDto>>> Handle(Query request, CancellationToken cancellationToken)
                {
                    List<TagDto> result = new List<TagDto>();

                    var tags = await _context.Tags.ToListAsync();
                    
                    _mapper.Map(tags,result);

                    return Result<List<TagDto>>.Success(result);
                }
            }
    
    }
}
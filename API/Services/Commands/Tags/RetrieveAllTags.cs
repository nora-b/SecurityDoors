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
            public class Query : IRequest<Result<List<TagDto>>> {
                public TagFilterParams filter { get; set;}
             }

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
                    var tags = new List<TagDto>();
                    
                    if (request.filter != null){

                        if (request.filter.TunnelStatus.Count > 0 || request.filter.OfficeStatus.Count > 0){
                          result = await _context.Tags.Where(x => request.filter.TunnelStatus.Contains(x.StatusTunnel) || request.filter.OfficeStatus.Contains(x.StatusOffice)).Include(x=>x.User).Select(x=> new TagDto {
                                Username = x.User.UserName,
                                Code = x.Code,
                                StatusTunnel = x.StatusTunnel,
                                IsAuthorizedTunnel = x.IsAuthorizedTunnel,
                                StatusOffice = x.StatusOffice,
                                IsAuthorizedOffice = x.IsAuthorizedOffice,
                                TagTunnelExpiresAt = x.TagTunnelExpiresAt,
                                TagOfficeExpiresAt = x.TagOfficeExpiresAt,
                                UserId = x.UserId
                            }).ToListAsync();
                        }
                       
                     result = await _context.Tags.Include(x=> x.User).Select(x=> new TagDto{
                             Username = x.User.UserName,
                                Code = x.Code,
                                StatusTunnel = x.StatusTunnel,
                                IsAuthorizedTunnel = x.IsAuthorizedTunnel,
                                StatusOffice = x.StatusOffice,
                                IsAuthorizedOffice = x.IsAuthorizedOffice,
                                TagTunnelExpiresAt = x.TagTunnelExpiresAt,
                                TagOfficeExpiresAt = x.TagOfficeExpiresAt,
                                UserId = x.UserId
                        }).ToListAsync();
                    }

                    return Result<List<TagDto>>.Success(result);
                }
            }

    }
}
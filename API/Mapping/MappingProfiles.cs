using API.DTOs;
using AutoMapper;
using Domain;

namespace API.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Tag, TagDto>();
            CreateMap<User, UserDto>();
        }
    }
}
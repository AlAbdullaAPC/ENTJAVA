using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Entities;

namespace SampleWebApiAspNetCore.MappingProfiles
{
    public class PlayerMappings : Profile
    {
        public PlayerMappings()
        {
            CreateMap<PlayerEntity, PlayerDto>().ReverseMap();
            CreateMap<PlayerEntity, PlayerUpdateDto>().ReverseMap();
            CreateMap<PlayerEntity, PlayerCreateDto>().ReverseMap();
        }
    }
}

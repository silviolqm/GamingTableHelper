using AutoMapper;
using GameSystemService.Dtos;
using GameSystemService.Models;

namespace GameSystemService.Profiles
{
    public class GameSystemProfile : Profile
    {
        public GameSystemProfile()
        {
            CreateMap<GameSystem, GameSystemReadDto>();
            CreateMap<GameSystemCreateDto, GameSystem>();
            CreateMap<GameSystemReadDto, GameSystemPublishedDto>();
            /*CreateMap<GameSystem, GrpcGameSystemModel>()
                .ForMember(dest => dest.GameSystemId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));*/
        }
    }
}
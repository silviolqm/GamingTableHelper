using AutoMapper;
using GameSystemService;
using GameTableService.Dtos;
using GameTableService.Models;

namespace GameTableService.Profiles
{
    public class GameTableProfile : Profile
    {
        public GameTableProfile()
        {
            CreateMap<GameSystem, GameSystemReadDto>();
            CreateMap<GameTableCreateDto, GameTable>();
            CreateMap<GameTable, GameTableReadDto>();
            CreateMap<GameTableUpdateDto, GameTable>();
            CreateMap<GameSystemEventDto, GameSystem>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
            CreateMap<GrpcGameSystemModel, GameSystem>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
using AutoMapper;
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
            /*CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.PlatformId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Commands, opt => opt.Ignore());*/
        }
    }
}
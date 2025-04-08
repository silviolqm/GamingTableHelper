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
            CreateMap<GameSystem, GameSystemEventDto>();
            CreateMap<GameSystemCreateDto, GameSystem>();
            CreateMap<GameSystemUpdateDto, GameSystem>();
            CreateMap<GameSystemReadDto, GameSystemEventDto>();
            CreateMap<GameSystem, GrpcGameSystemModel>();
        }
    }
}
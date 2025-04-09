using AutoMapper;
using NotificationService.Dtos;
using NotificationService.Models;

namespace NotificationService.Profiles
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<UserEventDto, ApplicationUser>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
            CreateMap<ApplicationUser, ApplicationUserReadDto>();
        }
    }
}
using AutoMapper;
using AuthService.Models;
using AuthService.Dtos;

namespace AuthService.Profiles
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<ApplicationUser, GrpcUserModel>();
            CreateMap<ApplicationUser, UserEventDto>();
        }
    }
}
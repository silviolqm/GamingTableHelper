using AutoMapper;
using AuthService.Models;

namespace AuthService.Profiles
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<ApplicationUser, GrpcUserModel>();
        }
    }
}
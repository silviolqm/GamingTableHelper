using AutoMapper;
using Grpc.Core;
using AuthService.Data;
using Microsoft.AspNetCore.Identity;
using AuthService.Models;

namespace AuthService.SyncDataServices
{
    public class GrpcApplicationUserService : GrpcApplicationUser.GrpcApplicationUserBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GrpcApplicationUserService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public override Task<UserResponse> GetAllUsers(GetAllRequest request, ServerCallContext context)
        {
            var response = new UserResponse();
            var users = _userManager.Users.ToList();

            foreach(var user in users)
            {
                response.Users.Add(_mapper.Map<GrpcUserModel>(user));
            }

            return Task.FromResult(response);
        }
    }
}
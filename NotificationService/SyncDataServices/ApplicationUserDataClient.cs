using AutoMapper;
using AuthService;
using NotificationService.Models;
using Grpc.Net.Client;

namespace NotificationService.SyncDataServices
{
    public class ApplicationUserDataClient : IApplicationUserDataClient
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ApplicationUserDataClient(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }

        public IEnumerable<ApplicationUser>? ReturnAllUsers()
        {
            var channel = GrpcChannel.ForAddress(_configuration["GrpcAppUsersService"]!);
            var client = new GrpcApplicationUser.GrpcApplicationUserClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllUsers(request);
                return _mapper.Map<IEnumerable<ApplicationUser>>(reply.Users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection to gRPC server failed: {ex.Message}");
                return null;
            }
        }
    }
}
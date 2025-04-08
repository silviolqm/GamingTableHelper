using AutoMapper;
using GameSystemService;
using GameTableService.Models;
using Grpc.Net.Client;

namespace GameTableService.SyncDataServices
{
    public class GameSystemDataClient : IGameSystemDataClient
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GameSystemDataClient(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }

        public IEnumerable<GameSystem> ReturnAllGameSystems()
        {
            var channel = GrpcChannel.ForAddress(_configuration["GrpcGameSysService"]!);
            var client = new GrpcGameSystem.GrpcGameSystemClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllGameSystems(request);
                return _mapper.Map<IEnumerable<GameSystem>>(reply.Gamesystems);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection to gRPC server failed: {ex.Message}");
                return null;
            }
        }
    }
}
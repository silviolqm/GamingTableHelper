using AutoMapper;
using Grpc.Core;
using GameSystemService.Data;

namespace GameSystemService.SyncDataServices
{
    public class GrpcGameSystemService : GrpcGameSystem.GrpcGameSystemBase
    {
        private readonly IGameSystemRepo _repository;
        private readonly IMapper _mapper;

        public GrpcGameSystemService(IGameSystemRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public override Task<GameSystemResponse> GetAllGameSystems(GetAllRequest request, ServerCallContext context)
        {
            var response = new GameSystemResponse();
            var gamesystems = _repository.GetAllGameSystems();

            foreach(var gamesystem in gamesystems)
            {
                response.Gamesystems.Add(_mapper.Map<GrpcGameSystemModel>(gamesystem));
            }

            return Task.FromResult(response);
        }
    }
}
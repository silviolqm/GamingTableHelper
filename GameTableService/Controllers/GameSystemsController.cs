using AutoMapper;
using GameTableService.Data;
using GameTableService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace GameTableService.Controllers
{
    [ApiController]
    [Route("api/gametableservice/[controller]")]
    public class GameSystemsController : ControllerBase
    {
        private readonly IGameTableRepo _repo;
        private readonly IMapper _mapper;

        public GameSystemsController(IGameTableRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        //Only for local testing - not exposed in the gateway
        [HttpGet]
        public ActionResult<IEnumerable<GameSystemReadDto>> GetGameSystems()
        {
            var gameSystemsInRepo = _repo.GetGameSystems();
            return Ok(_mapper.Map<IEnumerable<GameSystemReadDto>>(gameSystemsInRepo));
        }
    }
}
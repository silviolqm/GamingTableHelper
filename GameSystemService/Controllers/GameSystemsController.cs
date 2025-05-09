using AutoMapper;
using GameSystemService.AsyncDataServices;
using GameSystemService.Data;
using GameSystemService.Dtos;
using GameSystemService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameSystemService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameSystemsController : ControllerBase
    {
        private readonly IGameSystemRepo _repo;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public GameSystemsController(IGameSystemRepo repo, IMapper mapper, IMessageBusClient messageBusClient)
        {
            _repo = repo;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<GameSystemReadDto>> GetAllGameSystems(){
            var gameSystemsInRepo = _repo.GetAllGameSystems();
            var gameSystems = _mapper.Map<IEnumerable<GameSystemReadDto>>(gameSystemsInRepo);
            return Ok(gameSystems);
        }

        [HttpGet("{id}", Name="GetGameSystemById")]
        public ActionResult<GameSystemReadDto> GetGameSystemById(Guid id){
            var gameSystemInRepo = _repo.GetGameSystemById(id);
            if (gameSystemInRepo == null){
                return NotFound();
            }
            var gameSystem = _mapper.Map<GameSystemReadDto>(gameSystemInRepo);
            return Ok(gameSystem);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<GameSystemReadDto>> CreateGameSystem(GameSystemCreateDto gameSystemCreateDto)
        {
            var gameSystemToCreate = _mapper.Map<GameSystem>(gameSystemCreateDto);
            _repo.CreateGameSystem(gameSystemToCreate);
            _repo.SaveChanges();

            // Send a message to the MessageBus
            // to notify other services about the new GameSystem creation
            try
            {
                var gameSystemEventDto = _mapper.Map<GameSystemEventDto>(gameSystemToCreate);
                gameSystemEventDto.Event = "GameSystem_Created";
                await _messageBusClient.PublishGameSystemEvent(gameSystemEventDto);
                Console.WriteLine($"Sent GameSystem_Created message to MessageBus.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send message to MessageBus: {ex.Message}");
            }

            var gameSystemReadDto = _mapper.Map<GameSystemReadDto>(gameSystemToCreate);
            return CreatedAtRoute(nameof(GetGameSystemById), new {Id = gameSystemReadDto.Id}, gameSystemReadDto);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGameSystem(Guid id)
        {
            var gameSystemInRepo = _repo.GetGameSystemById(id);
            if (gameSystemInRepo == null){
                return NotFound();
            }
            _repo.DeleteGameSystem(gameSystemInRepo);
            _repo.SaveChanges();

            // Send a message to the MessageBus
            // to notify other services about the GameSystem deletion
            try
            {
                var gameSystemEventDto = _mapper.Map<GameSystemEventDto>(gameSystemInRepo);
                gameSystemEventDto.Event = "GameSystem_Deleted";
                await _messageBusClient.PublishGameSystemEvent(gameSystemEventDto);
                Console.WriteLine($"Sent GameSystem_Deleted message to MessageBus.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send message to MessageBus: {ex.Message}");
            }
            return NoContent();
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGameSystem(Guid id, GameSystemUpdateDto gameSystemUpdateDto)
        {
            var existingGameSystem = _repo.GetGameSystemById(id);
            if (existingGameSystem == null)
            {
                return NotFound();
            }

            _mapper.Map(gameSystemUpdateDto, existingGameSystem);
            _repo.EditGameSystem(existingGameSystem);
            _repo.SaveChanges();

            // Send a message to the MessageBus
            // to notify other services about the updated GameSystem resource
            try
            {
                var gameSystemEventDto = _mapper.Map<GameSystemEventDto>(existingGameSystem);
                gameSystemEventDto.Event = "GameSystem_Updated";
                await _messageBusClient.PublishGameSystemEvent(gameSystemEventDto);
                Console.WriteLine($"Sent GameSystem_Updated message to MessageBus.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send message to MessageBus: {ex.Message}");
            }

            return Ok(existingGameSystem);
        }
    }
}
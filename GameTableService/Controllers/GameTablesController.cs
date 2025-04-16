using System.Security.Claims;
using AutoMapper;
using GameTableService.AsyncDataServices;
using GameTableService.Data;
using GameTableService.Dtos;
using GameTableService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameTableService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameTablesController : ControllerBase
    {
        private readonly IGameTableRepo _repo;
        private readonly IMapper _mapper;
        private readonly IMessageBusPublisher _messageBusPublisher;

        public GameTablesController(IGameTableRepo repo, IMapper mapper, IMessageBusPublisher messageBusPublisher)
        {
            _repo = repo;
            _mapper = mapper;
            _messageBusPublisher = messageBusPublisher;
        }

        [HttpGet]
        public ActionResult<IEnumerable<GameTableReadDto>> GetAllGameTables([FromQuery] Guid? gameSystemId)
        {
            if (gameSystemId.HasValue)
            {
                var filteredGameTables = _repo.GetAllGameTablesByGameSystem(gameSystemId);
                var response = _mapper.Map<IEnumerable<GameTableReadDto>>(filteredGameTables);
                return Ok(response);
            }
            else
            {
                var gameTablesInRepo = _repo.GetAllGameTables();
                var response = _mapper.Map<IEnumerable<GameTableReadDto>>(gameTablesInRepo);
                return Ok(response);
            }
        }

        [HttpGet("{id}", Name = "GetGameTableById")]
        public ActionResult<GameTableReadDto> GetGameTableById(Guid id)
        {
            var gameTableInRepo = _repo.GetGameTableById(id);
            if (gameTableInRepo == null){
                return NotFound();
            }
            var gameTable = _mapper.Map<GameTableReadDto>(gameTableInRepo);
            return Ok(gameTable);
        }

        [Authorize]
        [HttpPost]
        public ActionResult<GameTableReadDto> CreateGameTable(GameTableCreateDto gameTableCreateDto)
        {
            var gameTableToCreate = _mapper.Map<GameTable>(gameTableCreateDto);

            //Attaches the userId as the owner of this game table
            var appUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (appUserIdClaim == null)
            {
                return Problem(
                    type: "Unauthorized",
                    title: "Could not retrieve user id.",
                    detail: "User must be logged in to create a game table.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }
            var userId = Guid.Parse(appUserIdClaim.Value);
            gameTableToCreate.OwnerUserId = userId;

            //Validate GameSystemId
            if (!_repo.GameSystemExists(gameTableCreateDto.GameSystemId))
            {
                return Problem(
                    type: "Bad Request",
                    title: "Invalid Game System",
                    detail: "The Game System provided doesn't exist.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            //Validate Start Time for Game Table
            if (gameTableCreateDto.StartDateTime <= DateTimeOffset.UtcNow)
            {
                return Problem(
                    type: "Bad Request",
                    title: "Invalid StartDateTime",
                    detail: "The game table start date and time should be in the future.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            //Success
            _repo.CreateGameTable(gameTableToCreate);
            _repo.SaveChanges();

            var gameTableCreatedDto = _mapper.Map<GameTableReadDto>(gameTableToCreate);
            return CreatedAtRoute(nameof(GetGameTableById), 
                new {Id = gameTableCreatedDto.Id}, gameTableCreatedDto);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult DeleteGameTable(Guid id)
        {
            var gameTableInRepo = _repo.GetGameTableById(id);
            if (gameTableInRepo == null)
            {
                return NotFound();
            }

            //Get user Id from JWT token
            var appUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (appUserIdClaim == null)
            {
                return Problem(
                    type: "Unauthorized",
                    title: "Could not retrieve user id.",
                    detail: "User must be logged in to delete a game table.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }
            var userId = Guid.Parse(appUserIdClaim.Value);

            // Check if the user owns the GameTable before deletion
            if (gameTableInRepo.OwnerUserId == userId)
            {
                _repo.DeleteGameTable(gameTableInRepo);
                _repo.SaveChanges();
                return NoContent();
            }
            else
            {
                return Problem(
                    type: "Unauthorized",
                    title: "Game table belongs to another user.",
                    detail: "User can only delete his own game tables.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public ActionResult UpdateGameTable(Guid id, GameTableUpdateDto gameTableUpdateDto)
        {
            //Validate GameSystem
            if (!_repo.GameSystemExists(gameTableUpdateDto.GameSystemId))
            {
                return Problem(
                    type: "Bad Request",
                    title: "Invalid Game System",
                    detail: "The Game System provided doesn't exist.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            var existingGameTable = _repo.GetGameTableById(id);
            if (existingGameTable == null)
            {
                return NotFound();
            }

            // Get userId from the JWT token
            var appUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (appUserIdClaim == null)
            {
                return Problem(
                    type: "Unauthorized",
                    title: "Could not retrieve user id.",
                    detail: "User must be logged in to update a game table.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }
            var userId = Guid.Parse(appUserIdClaim.Value);

            // Check if the user owns the GameTable before updating
            if (existingGameTable.OwnerUserId == userId)
            {
                _mapper.Map(gameTableUpdateDto, existingGameTable);
                _repo.UpdateGameTable(existingGameTable);
                _repo.SaveChanges();

                return Ok(_mapper.Map<GameTableReadDto>(existingGameTable));
            }
            else
            {
                return Problem(
                    type: "Unauthorized",
                    title: "Game table belongs to another user.",
                    detail: "User can only update his own game tables.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult> JoinGameTable(Guid id)
        {
            var gameTable = _repo.GetGameTableById(id);
            if (gameTable == null)
            {
                return NotFound();
            }
            //Game Table at full capacity
            if (gameTable.Players.Count() >= gameTable.MaxPlayers)
            {
                return Problem(
                    type: "Conflict",
                    title: "Game Table is full.",
                    detail: "This Game Table has reached the maximum number of users.",
                    statusCode: StatusCodes.Status409Conflict);
            }

            //Get user Id from JWT token
            var appUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (appUserIdClaim == null)
            {
                return Problem(
                    type: "Unauthorized",
                    title: "Could not retrieve user id.",
                    detail: "User must be logged in to join a game table.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }
            var userId = Guid.Parse(appUserIdClaim.Value);

            if (_repo.AddPlayerToGameTable(id, userId))
            {
                if (gameTable.Players.Count() == gameTable.MaxPlayers)
                {
                    // Send a message to the MessageBus to notify other services that the game table is full
                    try
                    {
                        var gameTableFullEventDto = _mapper.Map<GameTableFullEventDto>(gameTable);
                        gameTableFullEventDto.Event = "GameTable_Full";
                        await _messageBusPublisher.PublishGameTableFullEvent(gameTableFullEventDto);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Could not send message to MessageBus: {ex.Message}");
                    }
                }

                var gameTableReadDto = _mapper.Map<GameTableReadDto>(gameTable);
                return Ok(gameTableReadDto);
            }
            else
            {
                return Problem(
                    type: "Conflict",
                    title: "Could not join this game table.",
                    detail: "User is already a player in this game table.",
                    statusCode: StatusCodes.Status409Conflict);
            }
        }
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Data;
using NotificationService.Dtos;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/notifications/[controller]")]
    public class ApplicationUsersController : ControllerBase
    {
        private readonly INotificationRepo _repo;
        private readonly IMapper _mapper;

        public ApplicationUsersController(INotificationRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ApplicationUserReadDto>> GetAllApplicationUsers()
        {
            var usersInRepo = _repo.GetAllUsers();
            var response = _mapper.Map<IEnumerable<ApplicationUserReadDto>>(usersInRepo);
            return Ok(response);
        }
    }
}
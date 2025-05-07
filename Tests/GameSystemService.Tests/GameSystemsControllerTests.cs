using AutoMapper;
using GameSystemService.AsyncDataServices;
using GameSystemService.Controllers;
using GameSystemService.Data;
using GameSystemService.Dtos;
using GameSystemService.Models;
using Moq;

namespace GameSystemService.Tests;

public class GameSystemsControllerTests
{
    [Fact]
    public void Creating_GameSystem_Should_Publish_Event()
    {
        // Arrange
        GameSystemCreateDto gameSystemCreateDto = new GameSystemCreateDto
        {
            Name = "Test Game System",
            Publisher = "Test Publisher",
        };

        var mockRepo = new Mock<IGameSystemRepo>();

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(x => x.Map<GameSystem>(It.IsAny<GameSystemCreateDto>()))
            .Returns(new GameSystem
            {
                Id = Guid.NewGuid(),
                Name = gameSystemCreateDto.Name,
                Publisher = gameSystemCreateDto.Publisher,
            });
        mockMapper.Setup(x => x.Map<GameSystemEventDto>(It.IsAny<GameSystem>()))
            .Returns(new GameSystemEventDto
            {
                Id = Guid.NewGuid(),
                Name = gameSystemCreateDto.Name,
                Event = "GameSystem_Created",
            });
        mockMapper.Setup(x => x.Map<GameSystemReadDto>(It.IsAny<GameSystem>()))
            .Returns(new GameSystemReadDto
            {
                Id = Guid.NewGuid(),
                Name = gameSystemCreateDto.Name,
                Publisher = gameSystemCreateDto.Publisher,
            });
        
        var mockMessageBusClient = new Mock<IMessageBusClient>();

        var sut = new GameSystemsController(
            mockRepo.Object, 
            mockMapper.Object, 
            mockMessageBusClient.Object
        );

        // Act
        var result = sut.CreateGameSystem(gameSystemCreateDto);

        // Assert
        mockMessageBusClient.Verify(x => x.PublishGameSystemEvent(It.IsAny<GameSystemEventDto>()), Times.Once);
    }

    [Fact]
    public void Deleting_GameSystem_Should_Publish_Event()
    {
        // Arrange
        var gameSystemId = Guid.NewGuid();

        var mockRepo = new Mock<IGameSystemRepo>();
        mockRepo.Setup(x => x.GetGameSystemById(It.IsAny<Guid>()))
            .Returns(new GameSystem
            {
                Id = gameSystemId,
                Name = "Test Game System",
                Publisher = "Test Publisher",
            });

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(x => x.Map<GameSystemEventDto>(It.IsAny<GameSystem>()))
            .Returns(new GameSystemEventDto
            {
                Id = gameSystemId,
                Name = "Test Game System",
                Event = "GameSystem_Deleted",
            });
        
        var mockMessageBusClient = new Mock<IMessageBusClient>();

        var sut = new GameSystemsController(
            mockRepo.Object, 
            mockMapper.Object, 
            mockMessageBusClient.Object
        );

        // Act
        var result = sut.DeleteGameSystem(gameSystemId);

        // Assert
        mockMessageBusClient.Verify(x => x.PublishGameSystemEvent(It.IsAny<GameSystemEventDto>()), Times.Once);
    }

        [Fact]
    public void Updating_GameSystem_Should_Publish_Event()
    {
        // Arrange
        var gameSystemId = Guid.NewGuid();
        var gameSystemUpdateDto = new GameSystemUpdateDto
        {
            Name = "Updated Game System",
            Publisher = "Updated Publisher",
        };

        var mockRepo = new Mock<IGameSystemRepo>();
        mockRepo.Setup(x => x.GetGameSystemById(It.IsAny<Guid>()))
            .Returns(new GameSystem
            {
                Id = gameSystemId,
                Name = "Test Game System",
                Publisher = "Test Publisher",
            });

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(x => x.Map<GameSystemEventDto>(It.IsAny<GameSystem>()))
            .Returns(new GameSystemEventDto
            {
                Id = gameSystemId,
                Name = "Test Game System",
                Event = "GameSystem_Updated",
            });
        
        var mockMessageBusClient = new Mock<IMessageBusClient>();

        var sut = new GameSystemsController(
            mockRepo.Object, 
            mockMapper.Object, 
            mockMessageBusClient.Object
        );

        // Act
        var result = sut.UpdateGameSystem(gameSystemId, gameSystemUpdateDto);

        // Assert
        mockMessageBusClient.Verify(x => x.PublishGameSystemEvent(It.IsAny<GameSystemEventDto>()), Times.Once);
    }
}

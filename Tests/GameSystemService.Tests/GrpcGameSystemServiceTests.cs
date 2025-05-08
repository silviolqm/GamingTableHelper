using Moq;
using AutoMapper;
using GameSystemService.Data;
using GameSystemService.SyncDataServices;
using GameSystemService.Models;
using FluentAssertions;

namespace GameSystemService.Tests;

public class GrpcGameSystemServiceTests
{
    [Fact]
    public async Task GetAllGameSystems_ReturnsAllGameSystems()
    {
        // Arrange
        var repoMock = new Mock<IGameSystemRepo>();
        var mapperMock = new Mock<IMapper>();
        var service = new GrpcGameSystemService(repoMock.Object, mapperMock.Object);
        var gameSystems = new List<GameSystem> { new GameSystem { Id = Guid.NewGuid(), Name = "Test", Publisher = "TestPublisher" } };
        GameSystemService.GrpcGameSystemModel grpcGameSystemModel = new GameSystemService.GrpcGameSystemModel { Id = "1", Name = "Test", Publisher = "TestPublisher" };
        repoMock.Setup(r => r.GetAllGameSystems()).Returns(gameSystems);
        mapperMock.Setup(m => m.Map<GrpcGameSystemModel>(It.IsAny<GameSystem>()))
                   .Returns(grpcGameSystemModel);

        // Act
        var response = await service.GetAllGameSystems(new GetAllRequest(), null);

        // Assert
        response.Should().NotBeNull();
        response.Gamesystems.Should().ContainSingle();
        response.Gamesystems[0].Should().BeEquivalentTo(grpcGameSystemModel);
    }
}
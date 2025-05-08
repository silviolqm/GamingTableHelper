using Microsoft.EntityFrameworkCore;
using Moq;
using GameSystemService.Data;
using GameSystemService.Models;

namespace GameSystemService.Tests;
public class GameSystemRepoTest
{
    private Mock<AppDbContext> _mockContext;
    private Mock<DbSet<GameSystem>> _mockSet;
    private List<GameSystem> _data;

    public GameSystemRepoTest()
    {
        _data = new List<GameSystem>
        {
            new GameSystem { Id = Guid.NewGuid(), Name = "System1", Publisher = "Publisher1" },
            new GameSystem { Id = Guid.NewGuid(), Name = "System2", Publisher = "Publisher2" }
        };

        var queryable = _data.AsQueryable();

        _mockSet = new Mock<DbSet<GameSystem>>();
        _mockSet.As<IQueryable<GameSystem>>().Setup(m => m.Provider).Returns(queryable.Provider);
        _mockSet.As<IQueryable<GameSystem>>().Setup(m => m.Expression).Returns(queryable.Expression);
        _mockSet.As<IQueryable<GameSystem>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        _mockSet.As<IQueryable<GameSystem>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        _mockSet.Setup(m => m.Add(It.IsAny<GameSystem>())).Callback<GameSystem>(_data.Add);
        _mockSet.Setup(m => m.Remove(It.IsAny<GameSystem>())).Callback<GameSystem>(gs => _data.Remove(gs));

        _mockContext = new Mock<AppDbContext>();
        _mockContext.Setup(c => c.GameSystems).Returns(_mockSet.Object);
        _mockContext.Setup(c => c.Set<GameSystem>()).Returns(_mockSet.Object);
        _mockContext.Setup(c => c.SaveChanges()).Returns(1);
        _mockContext.Setup(c => c.Entry(It.IsAny<GameSystem>())).Returns((GameSystem gs) =>
        {
            var mockEntry = new Mock<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<GameSystem>>();
            mockEntry.Setup(e => e.CurrentValues.SetValues(It.IsAny<GameSystem>())).Callback<GameSystem>(g =>
            {
                var idx = _data.FindIndex(x => x.Id == g.Id);
                if (idx >= 0) _data[idx] = g;
            });
            return mockEntry.Object;
        });
    }

    [Fact]
    public void CreateGameSystem_AddsGameSystem()
    {
        var repo = new GameSystemRepo(_mockContext.Object);
        var newSystem = new GameSystem { Id = Guid.NewGuid(), Name = "System3", Publisher = "Publisher3" };
        repo.CreateGameSystem(newSystem);
        Assert.Contains(newSystem, _data);
    }

    [Fact]
    public void CreateGameSystem_Null_Throws()
    {
        var repo = new GameSystemRepo(_mockContext.Object);
        Assert.Throws<ArgumentNullException>(() => repo.CreateGameSystem(null));
    }

    [Fact]
    public void DeleteGameSystem_RemovesGameSystem()
    {
        var repo = new GameSystemRepo(_mockContext.Object);
        var toRemove = _data[0];
        repo.DeleteGameSystem(toRemove);
        Assert.DoesNotContain(toRemove, _data);
    }

    [Fact]
    public void DeleteGameSystem_Null_Throws()
    {
        var repo = new GameSystemRepo(_mockContext.Object);
        Assert.Throws<ArgumentNullException>(() => repo.DeleteGameSystem(null));
    }

    [Fact]
    public void EditGameSystem_UpdatesGameSystem()
    {
        var repo = new GameSystemRepo(_mockContext.Object);
        var existing = _data[0];
        var updated = new GameSystem { Id = existing.Id, Name = "UpdatedName", Publisher = "UpdatedPublisher" };
        repo.EditGameSystem(updated);
        Assert.Equal("UpdatedName", _data[0].Name);
    }

    [Fact]
    public void EditGameSystem_Null_Throws()
    {
        var repo = new GameSystemRepo(_mockContext.Object);
        Assert.Throws<ArgumentNullException>(() => repo.EditGameSystem(null));
    }

    [Fact]
    public void EditGameSystem_NotFound_Throws()
    {
        var repo = new GameSystemRepo(_mockContext.Object);
        var notFound = new GameSystem { Id = Guid.NewGuid(), Name = "NotFound", Publisher = "NotFound" };
        Assert.Throws<KeyNotFoundException>(() => repo.EditGameSystem(notFound));
    }

    [Fact]
    public void GetAllGameSystems_ReturnsAll()
    {
        var repo = new GameSystemRepo(_mockContext.Object);
        var result = repo.GetAllGameSystems();
        Assert.Equal(_data.Count, result.Count());
    }

    [Fact]
    public void GetGameSystemById_ReturnsCorrect()
    {
        var repo = new GameSystemRepo(_mockContext.Object);
        var id = _data[0].Id;
        var result = repo.GetGameSystemById(id);
        Assert.Equal(_data[0], result);
    }

    [Fact]
    public void GetGameSystemById_NotFound_ReturnsNull()
    {
        var repo = new GameSystemRepo(_mockContext.Object);
        var result = repo.GetGameSystemById(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public void SaveChanges_CallsContext()
    {
        var repo = new GameSystemRepo(_mockContext.Object);
        var result = repo.SaveChanges();
        Assert.True(result);
        _mockContext.Verify(c => c.SaveChanges(), Times.Once);
    }
}
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using GameSystemService.Models;

namespace GameSystemService.Tests;

public class GameSystemTests
{
    [Fact]
    public void GameSystem_Should_Have_Required_Properties()
    {
        // Arrange
        var gameSystem = new GameSystem
        {
            Id = Guid.NewGuid(),
            Name = "Test Game System",
            Publisher = "Test Publisher",
        };

        // Act
        var validationResults = ValidateModel(gameSystem);

        // Assert
        Assert.Empty(validationResults);
    }

    
    [Theory]
    [InlineData(null, "Test Publisher")]
    [InlineData("Test Game System", null)]
    public void GameSystem_MissingRequiredProperty_ShouldFailValidation(string? name, string? publisher)
    {
        // Arrange
        var gameSystem = new GameSystem
        {
            Id = Guid.NewGuid(),
            Name = name!,
            Publisher = publisher!
        };

        // Act
        var validationResults = ValidateModel(gameSystem);

        // Assert
        validationResults.Should().ContainSingle(v => v.MemberNames.Contains("Name") || v.MemberNames.Contains("Publisher"));
    }

    private List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
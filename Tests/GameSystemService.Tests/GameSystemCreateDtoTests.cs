using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using GameSystemService.Dtos;

namespace GameSystemService.Tests;

public class GameSystemCreateDtoTests
{
    [Fact]
    public void GameSystemCreateDto_Should_Have_Required_Properties()
    {
        // Arrange
        var gameSystem = new GameSystemCreateDto
        {
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
    public void GameSystemCreateDto_MissingRequiredProperty_ShouldFailValidation(string? name, string? publisher)
    {
        // Arrange
        var gameSystem = new GameSystemCreateDto
        {
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
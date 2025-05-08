using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using FluentAssertions;
using GameSystemService.Dtos;

namespace GameSystemService.Tests;

public class GameSystemUpdateDtoTests
{
    [Fact]
    public void GameSystemUpdateDto_Should_Have_Required_Properties()
    {
        // Arrange
        var gameSystemUpdateDto = new GameSystemUpdateDto
        {
            Name = "Test Game System",
            Publisher = "Test Publisher",
        };

        // Act
        var validationResults = ValidateModel(gameSystemUpdateDto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    
    [Theory]
    [InlineData(null, "Test Publisher")]
    [InlineData("Test Game System", null)]
    public void GameSystemUpdateDto_MissingRequiredProperty_ShouldFailValidation(string? name, string? publisher)
    {
        // Arrange
        var gameSystemUpdateDto = new GameSystemUpdateDto
        {
            Name = name!,
            Publisher = publisher!
        };

        // Act
        var validationResults = ValidateModel(gameSystemUpdateDto);

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
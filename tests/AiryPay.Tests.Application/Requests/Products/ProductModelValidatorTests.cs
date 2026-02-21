using AiryPay.Application.Requests.Products;

namespace AiryPay.Tests.Application.Requests.Products;

public class ProductModelValidatorTests
{
    private readonly ProductModelValidator _validator = new();

    [Fact]
    public async Task ValidModel_ShouldNotHaveErrors()
    {
        var model = new ProductModel("😀", "Valid Product Name", 500, 123456789);

        var result = await _validator.ValidateAsync(model);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task DiscordEmoji_WhenNullOrEmpty_ShouldHaveError(string? emoji)
    {
        var model = new ProductModel(emoji!, "Valid Name", 500, 0);

        var result = await _validator.ValidateAsync(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DiscordEmoji");
    }

    [Fact]
    public async Task DiscordEmoji_WhenTooLong_ShouldHaveError()
    {
        var model = new ProductModel(new string('a', 65), "Valid Name", 500, 0);

        var result = await _validator.ValidateAsync(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DiscordEmoji");
    }

    [Fact]
    public async Task DiscordEmoji_WhenValidUnicodeEmoji_ShouldNotHaveError()
    {
        var model = new ProductModel("😀", "Valid Name", 500, 0);

        var result = await _validator.ValidateAsync(model);

        result.Errors.Where(e => e.PropertyName == "DiscordEmoji").Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ab")]
    public async Task Name_WhenTooShortOrEmpty_ShouldHaveError(string? name)
    {
        var model = new ProductModel("😀", name!, 500, 0);

        var result = await _validator.ValidateAsync(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Name_WhenTooLong_ShouldHaveError()
    {
        var model = new ProductModel("😀", new string('a', 33), 500, 0);

        var result = await _validator.ValidateAsync(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Name_WhenContainsInvalidCharacters_ShouldHaveError()
    {
        var model = new ProductModel("😀", "Product@Name", 500, 0);

        var result = await _validator.ValidateAsync(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Name_WhenValidLettersAndSpaces_ShouldNotHaveError()
    {
        var model = new ProductModel("😀", "Product Name 123", 500, 0);

        var result = await _validator.ValidateAsync(model);

        result.Errors.Where(e => e.PropertyName == "Name").Should().BeEmpty();
    }

    [Theory]
    [InlineData(99)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Price_WhenBelow100_ShouldHaveError(decimal price)
    {
        var model = new ProductModel("😀", "Valid Name", price, 0);

        var result = await _validator.ValidateAsync(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Theory]
    [InlineData(10001)]
    [InlineData(100000)]
    public async Task Price_WhenAbove10000_ShouldHaveError(decimal price)
    {
        var model = new ProductModel("😀", "Valid Name", price, 0);

        var result = await _validator.ValidateAsync(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Theory]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(10000)]
    public async Task Price_WhenInRange_ShouldNotHaveError(decimal price)
    {
        var model = new ProductModel("😀", "Valid Name", price, 0);

        var result = await _validator.ValidateAsync(model);

        result.Errors.Where(e => e.PropertyName == "Price").Should().BeEmpty();
    }
}

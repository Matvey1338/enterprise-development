using System.ComponentModel.DataAnnotations;
using DryCleaning.Application.Exceptions;
using DryCleaning.Contracts.Dto;
using DryCleaning.Domain.Enums;

namespace DryCleaning.Tests;

/// <summary>
/// CRUD-операции сервиса категорий: присвоение идентификаторов,
/// валидация данных и защита удаляемой категории от связей с изделиями
/// </summary>
public sealed class CategoryServiceTests
{
    private readonly ServiceSet _services = ServiceSetFactory.Create();

    [Fact]
    public async Task AddCategory_AssignsNextIdentifierAndReturnsDto()
    {
        // Arrange: эталон - максимальный занятый идентификатор 9, новый получит 10
        var upsert = new UpsertCategoryDto
        {
            Name = "Головные уборы",
            RecommendedCleaningType = CleaningType.Leather,
            CleaningPrice = 700m,
        };

        // Act
        var added = await _services.Categories.AddCategoryAsync(upsert);

        // Assert
        Assert.Equal(10, added.CategoryId);
        Assert.Equal("Головные уборы", added.Name);
        Assert.Equal(CleaningType.Leather, added.RecommendedCleaningType);
        Assert.Equal(700m, added.CleaningPrice);
    }

    [Fact]
    public async Task AddCategory_NegativePrice_ThrowsValidationException()
    {
        // Arrange
        var upsert = new UpsertCategoryDto
        {
            Name = "Головные уборы",
            RecommendedCleaningType = CleaningType.Dry,
            CleaningPrice = -1m,
        };

        // Act + Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _services.Categories.AddCategoryAsync(upsert));
    }

    [Fact]
    public async Task DeleteCategory_WithItems_ThrowsEntityInUseException()
    {
        // Arrange: с категорией 0 связаны изделия (например, пальто из заказа 0)
        const int categoryIdInUse = 0;

        // Act + Assert: категория остаётся в справочнике
        await Assert.ThrowsAsync<EntityInUseException>(
            () => _services.Categories.DeleteCategoryAsync(categoryIdInUse));
        Assert.NotNull(await _services.Categories.GetCategoryAsync(categoryIdInUse));
    }
}

using AutoMapper;
using DevNotes.Context;
using DevNotes.DTOs;
using DevNotes.Mapping;
using DevNotes.Repository;

namespace DevNotes.Tests.Services;

public class CategoryRepositoryTests : IAsyncLifetime
{
    private AppDatabase _database = null!;
    private CategoryRepository _repository = null!;
    private string _dbPath = null!;

    public async Task InitializeAsync()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"voicenotes_cat_test_{Guid.NewGuid()}.db3");
        _database = new AppDatabase(_dbPath);
        await _database.InitializeAsync();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<CategoryProfile>());
        var mapper = config.CreateMapper();
        _repository = new CategoryRepository(_database, mapper);
    }

    public async Task DisposeAsync()
    {
        await _database.Connection.CloseAsync();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    [Fact]
    public async Task SaveAsync_NewCategory_ShouldInsertAndReturnPositiveId()
    {
        var category = new CategoryInfo { Name = "Nova Categoria" };

        var result = await _repository.SaveAsync(category);

        Assert.True(result > 0);
    }

    [Fact]
    public async Task SaveAsync_ExistingCategory_ShouldUpdate()
    {
        await _repository.SaveAsync(new CategoryInfo { Name = "Original" });

        var saved = (await _repository.GetAllAsync()).First(c => c.Name == "Original");
        saved.Name = "Atualizado";
        await _repository.SaveAsync(saved);

        var updated = await _repository.GetByIdAsync(saved.Id);

        Assert.NotNull(updated);
        Assert.Equal("Atualizado", updated.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCategories_OrderedByName()
    {
        var categories = await _repository.GetAllAsync();

        Assert.True(categories.Count >= 6);
        for (int i = 1; i < categories.Count; i++)
        {
            Assert.True(
                string.Compare(categories[i - 1].Name, categories[i].Name, StringComparison.Ordinal) <= 0,
                $"Expected '{categories[i - 1].Name}' <= '{categories[i].Name}'");
        }
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ShouldReturnCategory()
    {
        var categories = await _repository.GetAllAsync();
        var first = categories.First();

        var found = await _repository.GetByIdAsync(first.Id);

        Assert.NotNull(found);
        Assert.Equal(first.Name, found.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ShouldReturnNull()
    {
        var found = await _repository.GetByIdAsync(99999);

        Assert.Null(found);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCategory()
    {
        await _repository.SaveAsync(new CategoryInfo { Name = "Para Excluir" });

        var all = await _repository.GetAllAsync();
        var toDelete = all.First(c => c.Name == "Para Excluir");

        await _repository.DeleteAsync(toDelete.Id);

        var afterDelete = await _repository.GetByIdAsync(toDelete.Id);
        Assert.Null(afterDelete);
    }
}

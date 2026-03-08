using AutoMapper;
using DevNote.Context;
using DevNote.DTOs;
using DevNote.Mapping;
using DevNote.Repository;

namespace DevNote.Tests.Services;

public class NoteRepositoryTests : IAsyncLifetime
{
    private AppDatabase _database = null!;
    private NoteRepository _repository = null!;
    private string _dbPath = null!;

    public async Task InitializeAsync()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"devnote_test_{Guid.NewGuid()}.db3");
        _database = new AppDatabase(_dbPath);
        await _database.InitializeAsync();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<NoteProfile>());
        var mapper = config.CreateMapper();
        _repository = new NoteRepository(_database, mapper);
    }

    public async Task DisposeAsync()
    {
        await _database.Connection.CloseAsync();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    [Fact]
    public async Task SaveAsync_NewNote_ShouldInsertAndReturnPositiveId()
    {
        var note = new NoteInfo { Title = "Nota teste", Description = "Descrição de teste", Category = "Tarefas" };

        var result = await _repository.SaveAsync(note);

        Assert.True(result > 0);
    }

    [Fact]
    public async Task SaveAsync_NewNote_ShouldSetCreatedAt()
    {
        var note = new NoteInfo { Title = "Nota com data", Description = "Teste", Category = "Ideias" };

        await _repository.SaveAsync(note);

        var saved = (await _repository.GetAllAsync()).First();
        Assert.True(saved.CreatedAt > DateTime.MinValue);
    }

    [Fact]
    public async Task SaveAsync_ExistingNote_ShouldUpdate()
    {
        var note = new NoteInfo { Title = "Original", Description = "Desc", Category = "Tarefas" };
        await _repository.SaveAsync(note);

        var saved = (await _repository.GetAllAsync()).First();
        saved.Title = "Atualizado";
        await _repository.SaveAsync(saved);

        var updated = await _repository.GetByIdAsync(saved.Id);

        Assert.NotNull(updated);
        Assert.Equal("Atualizado", updated.Title);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllNotes_OrderedByCreatedAtDesc()
    {
        await _repository.SaveAsync(new NoteInfo { Title = "Primeira", Description = "Conteúdo 1", Category = "Tarefas" });
        await Task.Delay(50);
        await _repository.SaveAsync(new NoteInfo { Title = "Segunda", Description = "Conteúdo 2", Category = "Ideias" });
        await Task.Delay(50);
        await _repository.SaveAsync(new NoteInfo { Title = "Terceira", Description = "Conteúdo 3", Category = "Lembretes" });

        var notes = await _repository.GetAllAsync();

        Assert.Equal(3, notes.Count);
        Assert.Equal("Terceira", notes[0].Title);
        Assert.Equal("Segunda", notes[1].Title);
        Assert.Equal("Primeira", notes[2].Title);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ShouldReturnNote()
    {
        await _repository.SaveAsync(new NoteInfo { Title = "Buscar por ID", Description = "Conteúdo", Category = "Pessoal" });

        var allNotes = await _repository.GetAllAsync();
        var found = await _repository.GetByIdAsync(allNotes.First().Id);

        Assert.NotNull(found);
        Assert.Equal("Buscar por ID", found.Title);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ShouldReturnNull()
    {
        var found = await _repository.GetByIdAsync(99999);

        Assert.Null(found);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveNote()
    {
        await _repository.SaveAsync(new NoteInfo { Title = "Para excluir", Description = "Conteúdo", Category = "Outros" });

        var allNotes = await _repository.GetAllAsync();
        Assert.Single(allNotes);

        await _repository.DeleteAsync(allNotes.First().Id);

        var remaining = await _repository.GetAllAsync();
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task GetByCategoryAsync_ShouldReturnOnlyMatchingCategory()
    {
        await _repository.SaveAsync(new NoteInfo { Title = "Tarefa 1", Description = "Conteúdo 1", Category = "Tarefas" });
        await _repository.SaveAsync(new NoteInfo { Title = "Ideia 1", Description = "Conteúdo 2", Category = "Ideias" });
        await _repository.SaveAsync(new NoteInfo { Title = "Tarefa 2", Description = "Conteúdo 3", Category = "Tarefas" });

        var tarefas = await _repository.GetByCategoryAsync("Tarefas");
        var ideias = await _repository.GetByCategoryAsync("Ideias");

        Assert.Equal(2, tarefas.Count);
        Assert.Single(ideias);
        Assert.All(tarefas, n => Assert.Equal("Tarefas", n.Category));
    }

    [Fact]
    public async Task GetByCategoryAsync_NoMatch_ShouldReturnEmpty()
    {
        await _repository.SaveAsync(new NoteInfo { Title = "Nota", Description = "Conteúdo", Category = "Tarefas" });

        var result = await _repository.GetByCategoryAsync("Inexistente");

        Assert.Empty(result);
    }
}

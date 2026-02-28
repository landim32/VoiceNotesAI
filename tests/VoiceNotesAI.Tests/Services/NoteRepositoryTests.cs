using VoiceNotesAI.Data;
using VoiceNotesAI.Models;
using VoiceNotesAI.Services;

namespace VoiceNotesAI.Tests.Services;

public class NoteRepositoryTests : IAsyncLifetime
{
    private AppDatabase _database = null!;
    private NoteRepository _repository = null!;
    private string _dbPath = null!;

    public async Task InitializeAsync()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"voicenotes_test_{Guid.NewGuid()}.db3");
        _database = new AppDatabase(_dbPath);
        await _database.InitializeAsync();
        _repository = new NoteRepository(_database);
    }

    public Task DisposeAsync()
    {
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task SaveAsync_NewNote_ShouldInsertAndReturnPositiveId()
    {
        var note = new Note
        {
            Title = "Nota teste",
            Description = "Descrição de teste",
            Category = "Tarefas"
        };

        var result = await _repository.SaveAsync(note);

        Assert.True(result > 0);
    }

    [Fact]
    public async Task SaveAsync_NewNote_ShouldSetCreatedAt()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        var note = new Note
        {
            Title = "Nota com data",
            Description = "Teste",
            Category = "Ideias"
        };

        await _repository.SaveAsync(note);

        Assert.True(note.CreatedAt >= before);
        Assert.True(note.UpdatedAt >= before);
    }

    [Fact]
    public async Task SaveAsync_ExistingNote_ShouldUpdate()
    {
        var note = new Note { Title = "Original", Description = "Desc", Category = "Tarefas" };
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
        await _repository.SaveAsync(new Note { Title = "Primeira", Category = "Tarefas" });
        await Task.Delay(50);
        await _repository.SaveAsync(new Note { Title = "Segunda", Category = "Ideias" });
        await Task.Delay(50);
        await _repository.SaveAsync(new Note { Title = "Terceira", Category = "Lembretes" });

        var notes = await _repository.GetAllAsync();

        Assert.Equal(3, notes.Count);
        Assert.Equal("Terceira", notes[0].Title);
        Assert.Equal("Segunda", notes[1].Title);
        Assert.Equal("Primeira", notes[2].Title);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ShouldReturnNote()
    {
        var note = new Note { Title = "Buscar por ID", Category = "Pessoal" };
        await _repository.SaveAsync(note);

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
        var note = new Note { Title = "Para excluir", Category = "Outros" };
        await _repository.SaveAsync(note);

        var allNotes = await _repository.GetAllAsync();
        Assert.Single(allNotes);

        await _repository.DeleteAsync(allNotes.First().Id);

        var remaining = await _repository.GetAllAsync();
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task GetByCategoryAsync_ShouldReturnOnlyMatchingCategory()
    {
        await _repository.SaveAsync(new Note { Title = "Tarefa 1", Category = "Tarefas" });
        await _repository.SaveAsync(new Note { Title = "Ideia 1", Category = "Ideias" });
        await _repository.SaveAsync(new Note { Title = "Tarefa 2", Category = "Tarefas" });

        var tarefas = await _repository.GetByCategoryAsync("Tarefas");
        var ideias = await _repository.GetByCategoryAsync("Ideias");

        Assert.Equal(2, tarefas.Count);
        Assert.Single(ideias);
        Assert.All(tarefas, n => Assert.Equal("Tarefas", n.Category));
    }

    [Fact]
    public async Task GetByCategoryAsync_NoMatch_ShouldReturnEmpty()
    {
        await _repository.SaveAsync(new Note { Title = "Nota", Category = "Tarefas" });

        var result = await _repository.GetByCategoryAsync("Inexistente");

        Assert.Empty(result);
    }
}

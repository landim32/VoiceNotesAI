using AutoMapper;
using DevNotes.Context;
using DevNotes.DTOs;
using DevNotes.Mapping;
using DevNotes.Repository;

namespace DevNotes.Tests.Services;

public class CommentRepositoryTests : IAsyncLifetime
{
    private AppDatabase _database = null!;
    private CommentRepository _repository = null!;
    private string _dbPath = null!;

    public async Task InitializeAsync()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"voicenotes_test_{Guid.NewGuid()}.db3");
        _database = new AppDatabase(_dbPath);
        await _database.InitializeAsync();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<CommentProfile>());
        var mapper = config.CreateMapper();
        _repository = new CommentRepository(_database, mapper);
    }

    public async Task DisposeAsync()
    {
        await _database.Connection.CloseAsync();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    [Fact]
    public async Task SaveAsync_NewComment_ShouldInsertAndReturnPositiveId()
    {
        var comment = new CommentInfo { NoteId = 1, Content = "Comentário de teste" };

        var result = await _repository.SaveAsync(comment);

        Assert.True(result > 0);
    }

    [Fact]
    public async Task SaveAsync_NewComment_ShouldSetCreatedAt()
    {
        var comment = new CommentInfo { NoteId = 1, Content = "Comentário com data" };

        await _repository.SaveAsync(comment);

        var saved = (await _repository.GetByNoteIdAsync(1)).First();
        Assert.True(saved.CreatedAt > DateTime.MinValue);
    }

    [Fact]
    public async Task GetByNoteIdAsync_ShouldReturnOnlyMatchingNoteId()
    {
        await _repository.SaveAsync(new CommentInfo { NoteId = 1, Content = "Comment A" });
        await _repository.SaveAsync(new CommentInfo { NoteId = 2, Content = "Comment B" });
        await _repository.SaveAsync(new CommentInfo { NoteId = 1, Content = "Comment C" });

        var comments = await _repository.GetByNoteIdAsync(1);

        Assert.Equal(2, comments.Count);
        Assert.All(comments, c => Assert.Equal(1, c.NoteId));
    }

    [Fact]
    public async Task GetByNoteIdAsync_ShouldReturnOrderedByCreatedAtDesc()
    {
        await _repository.SaveAsync(new CommentInfo { NoteId = 1, Content = "Primeiro" });
        await Task.Delay(50);
        await _repository.SaveAsync(new CommentInfo { NoteId = 1, Content = "Segundo" });
        await Task.Delay(50);
        await _repository.SaveAsync(new CommentInfo { NoteId = 1, Content = "Terceiro" });

        var comments = await _repository.GetByNoteIdAsync(1);

        Assert.Equal(3, comments.Count);
        Assert.Equal("Terceiro", comments[0].Content);
        Assert.Equal("Segundo", comments[1].Content);
        Assert.Equal("Primeiro", comments[2].Content);
    }

    [Fact]
    public async Task GetByNoteIdAsync_NoMatch_ShouldReturnEmpty()
    {
        await _repository.SaveAsync(new CommentInfo { NoteId = 1, Content = "Exists" });

        var comments = await _repository.GetByNoteIdAsync(999);

        Assert.Empty(comments);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveComment()
    {
        await _repository.SaveAsync(new CommentInfo { NoteId = 1, Content = "Para excluir" });

        var comments = await _repository.GetByNoteIdAsync(1);
        Assert.Single(comments);

        await _repository.DeleteAsync(comments.First().Id);

        var remaining = await _repository.GetByNoteIdAsync(1);
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task DeleteByNoteIdAsync_ShouldRemoveAllCommentsForNote()
    {
        await _repository.SaveAsync(new CommentInfo { NoteId = 1, Content = "A" });
        await _repository.SaveAsync(new CommentInfo { NoteId = 1, Content = "B" });
        await _repository.SaveAsync(new CommentInfo { NoteId = 2, Content = "C" });

        await _repository.DeleteByNoteIdAsync(1);

        var note1Comments = await _repository.GetByNoteIdAsync(1);
        var note2Comments = await _repository.GetByNoteIdAsync(2);

        Assert.Empty(note1Comments);
        Assert.Single(note2Comments);
    }
}

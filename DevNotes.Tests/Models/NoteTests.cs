using DevNotes.Models;

namespace DevNotes.Tests.Models;

public class NoteTests
{
    [Fact]
    public void Note_DefaultValues_ShouldBeInitialized()
    {
        var note = new Note();

        Assert.Equal(0, note.Id);
        Assert.Equal(string.Empty, note.Title);
        Assert.Equal(string.Empty, note.Description);
        Assert.Equal(string.Empty, note.Category);
        Assert.Equal(string.Empty, note.AudioFilePath);
        Assert.True(note.CreatedAt <= DateTime.UtcNow);
        Assert.True(note.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Note_SetProperties_ShouldRetainValues()
    {
        var now = DateTime.UtcNow;

        var note = new Note
        {
            Id = 42,
            Title = "Reunião amanhã",
            Description = "Lembrar de preparar a apresentação",
            Category = "Trabalho",
            AudioFilePath = "/data/audio/note_42.wav",
            CreatedAt = now,
            UpdatedAt = now
        };

        Assert.Equal(42, note.Id);
        Assert.Equal("Reunião amanhã", note.Title);
        Assert.Equal("Lembrar de preparar a apresentação", note.Description);
        Assert.Equal("Trabalho", note.Category);
        Assert.Equal("/data/audio/note_42.wav", note.AudioFilePath);
        Assert.Equal(now, note.CreatedAt);
        Assert.Equal(now, note.UpdatedAt);
    }

    [Fact]
    public void Validate_EmptyDescription_ShouldThrow()
    {
        var note = new Note { Title = "Título", Description = "" };

        var ex = Assert.Throws<InvalidOperationException>(() => note.Validate());
        Assert.Contains("conteúdo da nota", ex.Message);
    }

    [Fact]
    public void Validate_TitleTooLong_ShouldThrow()
    {
        var note = new Note
        {
            Title = new string('A', 201),
            Description = "Conteúdo válido"
        };

        var ex = Assert.Throws<InvalidOperationException>(() => note.Validate());
        Assert.Contains("200 caracteres", ex.Message);
    }

    [Fact]
    public void Validate_ValidNote_ShouldNotThrow()
    {
        var note = new Note { Title = "Título", Description = "Conteúdo" };

        note.Validate();
    }

    [Fact]
    public void Create_WithTitle_ShouldSetProperties()
    {
        var note = Note.Create("Meu Título", "Minha descrição", "Trabalho", "/audio.wav");

        Assert.Equal("Meu Título", note.Title);
        Assert.Equal("Minha descrição", note.Description);
        Assert.Equal("Trabalho", note.Category);
        Assert.Equal("/audio.wav", note.AudioFilePath);
        Assert.True(note.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_WithEmptyTitle_ShouldAutoGenerateFromDescription()
    {
        var note = Note.Create("", "Conteúdo da nota", "Ideias");

        Assert.Equal("Conteúdo da nota", note.Title);
    }

    [Fact]
    public void Create_WithEmptyTitle_LongDescription_ShouldTruncate()
    {
        var longDesc = new string('A', 100);
        var note = Note.Create("", longDesc, "Ideias");

        Assert.Equal(longDesc[..80] + "...", note.Title);
    }

    [Fact]
    public void Create_EmptyDescription_ShouldThrow()
    {
        Assert.Throws<InvalidOperationException>(() => Note.Create("Título", "", "Cat"));
    }

    [Fact]
    public void Update_ShouldSetPropertiesAndUpdateTimestamp()
    {
        var note = Note.Create("Original", "Descrição original", "Cat1");
        var before = note.UpdatedAt;

        note.Update("Novo", "Nova descrição", "Cat2", "/novo.wav");

        Assert.Equal("Novo", note.Title);
        Assert.Equal("Nova descrição", note.Description);
        Assert.Equal("Cat2", note.Category);
        Assert.Equal("/novo.wav", note.AudioFilePath);
        Assert.True(note.UpdatedAt >= before);
    }
}

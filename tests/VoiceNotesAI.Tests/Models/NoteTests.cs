using VoiceNotesAI.Models;

namespace VoiceNotesAI.Tests.Models;

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
}

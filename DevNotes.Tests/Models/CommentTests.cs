using DevNotes.Models;

namespace DevNotes.Tests.Models;

public class CommentTests
{
    [Fact]
    public void Validate_EmptyContent_ShouldThrow()
    {
        var comment = new Comment { NoteId = 1, Content = "" };

        var ex = Assert.Throws<InvalidOperationException>(() => comment.Validate());
        Assert.Contains("conteúdo do comentário", ex.Message);
    }

    [Fact]
    public void Validate_InvalidNoteId_ShouldThrow()
    {
        var comment = new Comment { NoteId = 0, Content = "Conteúdo" };

        var ex = Assert.Throws<InvalidOperationException>(() => comment.Validate());
        Assert.Contains("nota válida", ex.Message);
    }

    [Fact]
    public void Validate_ValidComment_ShouldNotThrow()
    {
        var comment = new Comment { NoteId = 1, Content = "Conteúdo" };

        comment.Validate();
    }

    [Fact]
    public void Create_ValidParameters_ShouldSetProperties()
    {
        var comment = Comment.Create(5, "Meu comentário");

        Assert.Equal(5, comment.NoteId);
        Assert.Equal("Meu comentário", comment.Content);
        Assert.True(comment.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_EmptyContent_ShouldThrow()
    {
        Assert.Throws<InvalidOperationException>(() => Comment.Create(1, ""));
    }

    [Fact]
    public void Create_InvalidNoteId_ShouldThrow()
    {
        Assert.Throws<InvalidOperationException>(() => Comment.Create(0, "Conteúdo"));
    }
}

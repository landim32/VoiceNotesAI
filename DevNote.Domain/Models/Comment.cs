using SQLite;

namespace DevNote.Models;

public class Comment
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int NoteId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Content))
            throw new InvalidOperationException("O conteúdo do comentário é obrigatório.");

        if (NoteId <= 0)
            throw new InvalidOperationException("O comentário deve estar associado a uma nota válida.");
    }

    public static Comment Create(int noteId, string content)
    {
        var comment = new Comment
        {
            NoteId = noteId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        comment.Validate();
        return comment;
    }
}

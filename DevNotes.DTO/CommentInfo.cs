namespace DevNotes.DTOs;

public class CommentInfo
{
    public int Id { get; set; }
    public int NoteId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

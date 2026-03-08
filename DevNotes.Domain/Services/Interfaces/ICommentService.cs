using DevNotes.DTOs;

namespace DevNotes.Services.Interfaces;

public interface ICommentService
{
    Task<List<CommentInfo>> GetByNoteIdAsync(int noteId);
    Task SaveAsync(CommentInfo comment);
    Task DeleteAsync(int id);
    Task DeleteByNoteIdAsync(int noteId);
}

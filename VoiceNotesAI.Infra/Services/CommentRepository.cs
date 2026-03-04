using VoiceNotesAI.Context;
using VoiceNotesAI.Models;

namespace VoiceNotesAI.Services;

public class CommentRepository : ICommentRepository
{
    private readonly AppDatabase _database;

    public CommentRepository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<Comment>> GetByNoteIdAsync(int noteId)
    {
        return await _database.Connection
            .Table<Comment>()
            .Where(c => c.NoteId == noteId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> SaveAsync(Comment comment)
    {
        if (comment.Id != 0)
        {
            return await _database.Connection.UpdateAsync(comment);
        }

        comment.CreatedAt = DateTime.UtcNow;
        return await _database.Connection.InsertAsync(comment);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _database.Connection.DeleteAsync<Comment>(id);
    }

    public async Task<int> DeleteByNoteIdAsync(int noteId)
    {
        return await _database.Connection.ExecuteAsync(
            "DELETE FROM Comment WHERE NoteId = ?", noteId);
    }
}

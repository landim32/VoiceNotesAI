using VoiceNotesAI.Data;
using VoiceNotesAI.Models;

namespace VoiceNotesAI.Services;

public class NoteRepository : INoteRepository
{
    private readonly AppDatabase _database;

    public NoteRepository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<Note>> GetAllAsync()
    {
        return await _database.Connection
            .Table<Note>()
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Note?> GetByIdAsync(int id)
    {
        return await _database.Connection
            .Table<Note>()
            .Where(n => n.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveAsync(Note note)
    {
        note.UpdatedAt = DateTime.UtcNow;

        if (note.Id != 0)
        {
            return await _database.Connection.UpdateAsync(note);
        }

        note.CreatedAt = DateTime.UtcNow;
        return await _database.Connection.InsertAsync(note);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _database.Connection.DeleteAsync<Note>(id);
    }

    public async Task<List<Note>> GetByCategoryAsync(string category)
    {
        return await _database.Connection
            .Table<Note>()
            .Where(n => n.Category == category)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
}

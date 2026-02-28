using VoiceNotesAI.Models;

namespace VoiceNotesAI.Services;

public interface INoteRepository
{
    Task<List<Note>> GetAllAsync();
    Task<Note?> GetByIdAsync(int id);
    Task<int> SaveAsync(Note note);
    Task<int> DeleteAsync(int id);
    Task<List<Note>> GetByCategoryAsync(string category);
}

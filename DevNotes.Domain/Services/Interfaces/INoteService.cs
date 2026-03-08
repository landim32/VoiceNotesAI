using DevNotes.DTOs;

namespace DevNotes.Services.Interfaces;

public interface INoteService
{
    Task<List<NoteInfo>> GetAllAsync();
    Task<List<NoteInfo>> GetByCategoryAsync(string category);
    Task<NoteInfo?> GetByIdAsync(int id);
    Task<NoteInfo> SaveAsync(NoteInfo note);
    Task DeleteAsync(int id);
    Task<List<string>> GetAllCategoryNamesAsync();
}

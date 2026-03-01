using VoiceNotesAI.Models;

namespace VoiceNotesAI.Services;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<int> SaveAsync(Category category);
    Task<int> DeleteAsync(int id);
}

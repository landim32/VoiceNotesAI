using DevNotes.DTOs;

namespace DevNotes.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryInfo>> GetAllAsync();
    Task<CategoryInfo?> GetByIdAsync(int id);
    Task SaveAsync(CategoryInfo category);
    Task DeleteAsync(int id);
    Task<List<string>> GetAllNamesAsync();
}

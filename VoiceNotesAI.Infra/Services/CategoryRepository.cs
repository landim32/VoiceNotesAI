using VoiceNotesAI.Context;
using VoiceNotesAI.Models;

namespace VoiceNotesAI.Services;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDatabase _database;

    public CategoryRepository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _database.Connection
            .Table<Category>()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _database.Connection
            .Table<Category>()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveAsync(Category category)
    {
        if (category.Id != 0)
        {
            return await _database.Connection.UpdateAsync(category);
        }

        return await _database.Connection.InsertAsync(category);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _database.Connection.DeleteAsync<Category>(id);
    }
}

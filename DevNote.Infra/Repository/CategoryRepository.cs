using AutoMapper;
using DevNote.Context;
using DevNote.DTOs;
using DevNote.Models;

namespace DevNote.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDatabase _database;
    private readonly IMapper _mapper;

    public CategoryRepository(AppDatabase database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public async Task<List<CategoryInfo>> GetAllAsync()
    {
        var categories = await _database.Connection
            .Table<Category>()
            .OrderBy(c => c.Name)
            .ToListAsync();
        return _mapper.Map<List<CategoryInfo>>(categories);
    }

    public async Task<CategoryInfo?> GetByIdAsync(int id)
    {
        var category = await _database.Connection
            .Table<Category>()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
        return category is null ? null : _mapper.Map<CategoryInfo>(category);
    }

    public async Task<int> SaveAsync(CategoryInfo categoryInfo)
    {
        var category = _mapper.Map<Category>(categoryInfo);

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

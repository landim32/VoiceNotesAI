using AutoMapper;
using DevNotes.DTOs;
using DevNotes.Models;
using DevNotes.Repository;
using DevNotes.Services.Interfaces;

namespace DevNotes.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<List<CategoryInfo>> GetAllAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<CategoryInfo?> GetByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task SaveAsync(CategoryInfo categoryInfo)
    {
        Category category;

        if (categoryInfo.Id != 0)
        {
            var existing = await _categoryRepository.GetByIdAsync(categoryInfo.Id)
                ?? throw new InvalidOperationException("Categoria não encontrada.");
            category = _mapper.Map<Category>(existing);
            category.Update(categoryInfo.Name.Trim());
        }
        else
        {
            category = Category.Create(categoryInfo.Name);
        }

        category.Validate();
        await _categoryRepository.SaveAsync(_mapper.Map<CategoryInfo>(category));
    }

    public async Task DeleteAsync(int id)
    {
        await _categoryRepository.DeleteAsync(id);
    }

    public async Task<List<string>> GetAllNamesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(c => c.Name).ToList();
    }
}

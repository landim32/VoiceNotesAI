using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoiceNotesAI.Models;
using VoiceNotesAI.Services;

namespace VoiceNotesAI.ViewModels;

public partial class CategoryListViewModel : ObservableObject
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryListViewModel(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [ObservableProperty]
    private ObservableCollection<Category> _categories = [];

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        IsLoading = true;

        try
        {
            var list = await _categoryRepository.GetAllAsync();
            Categories = new ObservableCollection<Category>(list);
            IsEmpty = Categories.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(Category category)
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Excluir categoria",
            $"Deseja excluir \"{category.Name}\"?",
            "Sim", "Não");

        if (!confirm) return;

        await _categoryRepository.DeleteAsync(category.Id);
        Categories.Remove(category);
        IsEmpty = Categories.Count == 0;
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Category category)
    {
        var parameters = new Dictionary<string, object> { { "Category", category } };
        await Shell.Current.GoToAsync("CategoryDetailPage", parameters);
    }

    [RelayCommand]
    private async Task AddCategoryAsync()
    {
        await Shell.Current.GoToAsync("CategoryDetailPage");
    }
}

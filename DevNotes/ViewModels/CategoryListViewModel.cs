using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevNotes.DTOs;
using DevNotes.Services.Interfaces;

namespace DevNotes.ViewModels;

public partial class CategoryListViewModel : ObservableObject
{
    private readonly ICategoryService _categoryService;

    public CategoryListViewModel(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [ObservableProperty]
    private ObservableCollection<CategoryInfo> _categories = [];

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
            var list = await _categoryService.GetAllAsync();
            Categories = new ObservableCollection<CategoryInfo>(list);
            IsEmpty = Categories.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(CategoryInfo category)
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Excluir categoria",
            $"Deseja excluir \"{category.Name}\"?",
            "Sim", "Não");

        if (!confirm) return;

        await _categoryService.DeleteAsync(category.Id);
        Categories.Remove(category);
        IsEmpty = Categories.Count == 0;
    }

    [RelayCommand]
    private async Task GoToDetailAsync(CategoryInfo category)
    {
        var parameters = new Dictionary<string, object> { { "CategoryInfo", category } };
        await Shell.Current.GoToAsync("CategoryDetailPage", parameters);
    }

    [RelayCommand]
    private async Task AddCategoryAsync()
    {
        await Shell.Current.GoToAsync("CategoryDetailPage");
    }
}

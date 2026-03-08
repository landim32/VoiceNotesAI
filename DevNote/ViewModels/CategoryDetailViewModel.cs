using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevNote.DTOs;
using DevNote.Services.Interfaces;

namespace DevNote.ViewModels;

public partial class CategoryDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly ICategoryService _categoryService;

    public CategoryDetailViewModel(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [ObservableProperty]
    private int _categoryId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private bool _isSaving;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("CategoryInfo", out var categoryObj) && categoryObj is CategoryInfo category)
        {
            CategoryId = category.Id;
            Name = category.Name;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        IsSaving = true;

        try
        {
            var categoryInfo = new CategoryInfo
            {
                Id = CategoryId,
                Name = Name
            };

            await _categoryService.SaveAsync(categoryInfo);
            await Shell.Current.GoToAsync("..");
        }
        catch (InvalidOperationException ex)
        {
            await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (CategoryId == 0) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Excluir categoria",
            $"Deseja excluir \"{Name}\"?",
            "Sim", "Não");

        if (!confirm) return;

        await _categoryService.DeleteAsync(CategoryId);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}

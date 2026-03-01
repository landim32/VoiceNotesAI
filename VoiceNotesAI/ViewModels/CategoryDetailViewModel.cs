using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoiceNotesAI.Models;
using VoiceNotesAI.Services;

namespace VoiceNotesAI.ViewModels;

public partial class CategoryDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryDetailViewModel(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [ObservableProperty]
    private int _categoryId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private bool _isSaving;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Category", out var categoryObj) && categoryObj is Category category)
        {
            CategoryId = category.Id;
            Name = category.Name;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("Erro", "O nome da categoria é obrigatório.", "OK");
            return;
        }

        IsSaving = true;

        try
        {
            var category = new Category
            {
                Id = CategoryId,
                Name = Name.Trim()
            };

            await _categoryRepository.SaveAsync(category);
            await Shell.Current.GoToAsync("..");
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

        await _categoryRepository.DeleteAsync(CategoryId);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}

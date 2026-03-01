using VoiceNotesAI.ViewModels;

namespace VoiceNotesAI.Pages;

public partial class CategoryListPage : ContentPage
{
    private readonly CategoryListViewModel _viewModel;

    public CategoryListPage(CategoryListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCategoriesCommand.ExecuteAsync(null);
    }
}

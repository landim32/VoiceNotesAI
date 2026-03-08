using DevNotes.ViewModels;

namespace DevNotes.Pages;

public partial class NoteResultPage : ContentPage
{
    private readonly NoteResultViewModel _viewModel;

    public NoteResultPage(NoteResultViewModel viewModel)
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

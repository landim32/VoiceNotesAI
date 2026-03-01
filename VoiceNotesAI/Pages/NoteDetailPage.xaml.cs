using VoiceNotesAI.ViewModels;

namespace VoiceNotesAI.Pages;

public partial class NoteDetailPage : ContentPage
{
    private readonly NoteDetailViewModel _viewModel;

    public NoteDetailPage(NoteDetailViewModel viewModel)
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

using VoiceNotesAI.ViewModels;

namespace VoiceNotesAI.Pages;

public partial class NoteListPage : ContentPage
{
    private readonly NoteListViewModel _viewModel;

    public NoteListPage(NoteListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadNotesCommand.ExecuteAsync(null);
    }
}

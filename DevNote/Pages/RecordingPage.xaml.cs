using DevNote.ViewModels;

namespace DevNote.Pages;

public partial class RecordingPage : ContentPage
{
    private readonly RecordingViewModel _viewModel;

    public RecordingPage(RecordingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.StartRecordingCommand.ExecuteAsync(null);
    }
}

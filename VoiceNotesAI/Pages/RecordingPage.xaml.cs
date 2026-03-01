using VoiceNotesAI.ViewModels;

namespace VoiceNotesAI.Pages;

public partial class RecordingPage : ContentPage
{
    public RecordingPage(RecordingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

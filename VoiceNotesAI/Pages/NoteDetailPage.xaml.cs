using VoiceNotesAI.ViewModels;

namespace VoiceNotesAI.Pages;

public partial class NoteDetailPage : ContentPage
{
    public NoteDetailPage(NoteDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        CategoryPicker.ItemsSource = NoteDetailViewModel.AvailableCategories;
    }
}

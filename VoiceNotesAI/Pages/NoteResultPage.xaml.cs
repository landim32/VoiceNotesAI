using VoiceNotesAI.ViewModels;

namespace VoiceNotesAI.Pages;

public partial class NoteResultPage : ContentPage
{
    public NoteResultPage(NoteResultViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        CategoryPicker.ItemsSource = NoteResultViewModel.AvailableCategories;
    }
}

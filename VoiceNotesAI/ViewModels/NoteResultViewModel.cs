using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoiceNotesAI.Models;
using VoiceNotesAI.Services;

namespace VoiceNotesAI.ViewModels;

public partial class NoteResultViewModel : ObservableObject, IQueryAttributable
{
    private readonly INoteRepository _noteRepository;
    private readonly ICategoryRepository _categoryRepository;

    public NoteResultViewModel(INoteRepository noteRepository, ICategoryRepository categoryRepository)
    {
        _noteRepository = noteRepository;
        _categoryRepository = categoryRepository;
    }

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private string _audioFilePath = string.Empty;

    [ObservableProperty]
    private string _transcribedText = string.Empty;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private ObservableCollection<string> _availableCategories = [];

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("NoteResult", out var noteResultObj) && noteResultObj is NoteResult result)
        {
            Title = result.Title;
            Description = result.Description;
            Category = result.Category;
        }

        if (query.TryGetValue("AudioFilePath", out var audioObj) && audioObj is string audioPath)
            AudioFilePath = audioPath;

        if (query.TryGetValue("TranscribedText", out var textObj) && textObj is string text)
            TranscribedText = text;
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        AvailableCategories = new ObservableCollection<string>(categories.Select(c => c.Name));
    }

    [RelayCommand]
    private async Task SaveNoteAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            await Shell.Current.DisplayAlert("Erro", "O título é obrigatório.", "OK");
            return;
        }

        IsSaving = true;

        try
        {
            var note = new Note
            {
                Title = Title,
                Description = Description,
                Category = Category,
                AudioFilePath = AudioFilePath
            };

            await _noteRepository.SaveAsync(note);
            await Shell.Current.GoToAsync("../..");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}

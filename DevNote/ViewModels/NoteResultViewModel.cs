using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevNote.DTOs;
using DevNote.AppServices;
using DevNote.Services.Interfaces;

namespace DevNote.ViewModels;

public partial class NoteResultViewModel : ObservableObject, IQueryAttributable
{
    private readonly INoteService _noteService;
    private readonly ICategoryService _categoryService;

    public NoteResultViewModel(INoteService noteService, ICategoryService categoryService)
    {
        _noteService = noteService;
        _categoryService = categoryService;
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
        var categoryNames = await _categoryService.GetAllNamesAsync();
        AvailableCategories = new ObservableCollection<string>(categoryNames);
    }

    [RelayCommand]
    private async Task SaveNoteAsync()
    {
        IsSaving = true;

        try
        {
            var noteInfo = new NoteInfo
            {
                Title = Title,
                Description = Description,
                Category = Category,
                AudioFilePath = AudioFilePath
            };

            await _noteService.SaveAsync(noteInfo);
            await Shell.Current.GoToAsync("../..");
        }
        catch (InvalidOperationException ex)
        {
            await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
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

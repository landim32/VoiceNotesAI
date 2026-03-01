using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoiceNotesAI.Models;
using VoiceNotesAI.Services;

namespace VoiceNotesAI.ViewModels;

public partial class NoteListViewModel : ObservableObject
{
    private readonly INoteRepository _noteRepository;

    public NoteListViewModel(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    [ObservableProperty]
    private ObservableCollection<Note> _notes = [];

    [ObservableProperty]
    private string _selectedCategory = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [RelayCommand]
    private async Task LoadNotesAsync()
    {
        IsLoading = true;

        try
        {
            List<Note> noteList;

            if (string.IsNullOrEmpty(SelectedCategory))
                noteList = await _noteRepository.GetAllAsync();
            else
                noteList = await _noteRepository.GetByCategoryAsync(SelectedCategory);

            Notes = new ObservableCollection<Note>(noteList);
            IsEmpty = Notes.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoToRecordingAsync()
    {
        await Shell.Current.GoToAsync("RecordingPage");
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Note note)
    {
        var parameters = new Dictionary<string, object> { { "Note", note } };
        await Shell.Current.GoToAsync("NoteDetailPage", parameters);
    }

    [RelayCommand]
    private async Task DeleteNoteAsync(Note note)
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Excluir nota",
            $"Deseja excluir \"{note.Title}\"?",
            "Sim", "Não");

        if (!confirm) return;

        await _noteRepository.DeleteAsync(note.Id);
        Notes.Remove(note);
        IsEmpty = Notes.Count == 0;
    }

    [RelayCommand]
    private async Task FilterByCategoryAsync(string category)
    {
        SelectedCategory = category;
        await LoadNotesAsync();
    }
}

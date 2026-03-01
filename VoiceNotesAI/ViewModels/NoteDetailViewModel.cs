using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoiceNotesAI.Models;
using VoiceNotesAI.Services;

namespace VoiceNotesAI.ViewModels;

public partial class NoteDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly INoteRepository _noteRepository;

    public NoteDetailViewModel(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    [ObservableProperty]
    private int _noteId;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private string _audioFilePath = string.Empty;

    [ObservableProperty]
    private DateTime _createdAt;

    [ObservableProperty]
    private bool _isSaving;

    public static readonly string[] AvailableCategories =
    [
        "Tarefas", "Ideias", "Lembretes", "Trabalho", "Pessoal", "Outros"
    ];

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Note", out var noteObj) && noteObj is Note note)
        {
            NoteId = note.Id;
            Title = note.Title;
            Description = note.Description;
            Category = note.Category;
            AudioFilePath = note.AudioFilePath;
            CreatedAt = note.CreatedAt;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
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
                Id = NoteId,
                Title = Title,
                Description = Description,
                Category = Category,
                AudioFilePath = AudioFilePath,
                CreatedAt = CreatedAt
            };

            await _noteRepository.SaveAsync(note);
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Excluir nota",
            $"Deseja excluir \"{Title}\"?",
            "Sim", "Não");

        if (!confirm) return;

        await _noteRepository.DeleteAsync(NoteId);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}

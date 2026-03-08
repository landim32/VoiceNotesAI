using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevNotes.DTOs;
using DevNotes.AppServices;
using DevNotes.Services.Interfaces;

namespace DevNotes.ViewModels;

public partial class NoteDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly INoteService _noteService;
    private readonly ICommentService _commentService;
    private readonly ISettingService _settingService;
    private readonly IAIAppService _aiService;
    private readonly ISpeechToTextAppService _speechToTextService;
    private readonly IAudioAppService _audioService;

    public NoteDetailViewModel(
        INoteService noteService,
        ICommentService commentService,
        ISettingService settingService,
        IAIAppService aiService,
        ISpeechToTextAppService speechToTextService,
        IAudioAppService audioService)
    {
        _noteService = noteService;
        _commentService = commentService;
        _settingService = settingService;
        _aiService = aiService;
        _speechToTextService = speechToTextService;
        _audioService = audioService;
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

    [ObservableProperty]
    private bool _isNewNote = true;

    [ObservableProperty]
    private string _pageTitle = "Nova Nota";

    [ObservableProperty]
    private ObservableCollection<string> _availableCategories = [];

    [ObservableProperty]
    private ObservableCollection<CommentInfo> _comments = [];

    [ObservableProperty]
    private bool _isFabMenuOpen;

    [ObservableProperty]
    private bool _isRecordingComment;

    [ObservableProperty]
    private bool _isTranscribing;

    [ObservableProperty]
    private bool _isConsolidating;

    [ObservableProperty]
    private bool _isImproving;

    [ObservableProperty]
    private string _recordingStatus = string.Empty;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("NoteInfo", out var noteObj) && noteObj is NoteInfo note)
        {
            NoteId = note.Id;
            Title = note.Title;
            Description = note.Description;
            Category = note.Category;
            AudioFilePath = note.AudioFilePath;
            CreatedAt = note.CreatedAt;
            IsNewNote = false;
            PageTitle = "Detalhes da Nota";
        }
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        var categoryNames = await _noteService.GetAllCategoryNamesAsync();
        AvailableCategories = new ObservableCollection<string>(categoryNames);

        if (IsNewNote && string.IsNullOrEmpty(Category))
        {
            var lastCategory = await _settingService.GetAsync("LastSelectedCategory");
            if (!string.IsNullOrEmpty(lastCategory) && AvailableCategories.Contains(lastCategory))
            {
                Category = lastCategory;
            }
        }
    }

    [RelayCommand]
    private async Task LoadCommentsAsync()
    {
        if (NoteId > 0)
        {
            var comments = await _commentService.GetByNoteIdAsync(NoteId);
            Comments = new ObservableCollection<CommentInfo>(comments);
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        IsSaving = true;

        try
        {
            var noteInfo = new NoteInfo
            {
                Id = NoteId,
                Title = Title,
                Description = Description,
                Category = Category,
                AudioFilePath = AudioFilePath
            };

            var saved = await _noteService.SaveAsync(noteInfo);
            NoteId = saved.Id;

            if (!string.IsNullOrEmpty(Category))
            {
                await _settingService.SetAsync("LastSelectedCategory", Category);
            }

            await Shell.Current.GoToAsync("..");
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

    private async Task EnsureNoteSavedAsync()
    {
        if (NoteId == 0)
        {
            var description = string.IsNullOrWhiteSpace(Description) ? "Nova Nota" : Description;
            var noteInfo = new NoteInfo
            {
                Title = Title,
                Description = description,
                Category = Category,
                AudioFilePath = AudioFilePath
            };

            var saved = await _noteService.SaveAsync(noteInfo);
            NoteId = saved.Id;
            IsNewNote = false;
            PageTitle = "Detalhes da Nota";
        }
    }

    [RelayCommand]
    private void ToggleFabMenu()
    {
        IsFabMenuOpen = !IsFabMenuOpen;
    }

    [RelayCommand]
    private async Task AddTextCommentAsync()
    {
        IsFabMenuOpen = false;

        var text = await Shell.Current.DisplayPromptAsync(
            "Novo Comentário",
            "Digite o comentário:",
            "Salvar",
            "Cancelar",
            placeholder: "Seu comentário...");

        if (string.IsNullOrWhiteSpace(text))
            return;

        await EnsureNoteSavedAsync();

        var commentInfo = new CommentInfo { NoteId = NoteId, Content = text };
        await _commentService.SaveAsync(commentInfo);
        await LoadCommentsAsync();
    }

    [RelayCommand]
    private async Task AddVoiceCommentAsync()
    {
        IsFabMenuOpen = false;

        await EnsureNoteSavedAsync();

        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert("Permissão negada",
                    "É necessário permissão do microfone para gravar comentários de voz.", "OK");
                return;
            }
        }

        RecordingStatus = "Gravando...";
        IsRecordingComment = true;
        await _audioService.StartRecordingAsync();
    }

    [RelayCommand]
    private async Task StopVoiceCommentAsync()
    {
        IsRecordingComment = false;
        RecordingStatus = "Transcrevendo...";
        IsTranscribing = true;

        try
        {
            var audioPath = await _audioService.StopRecordingAsync();

            if (string.IsNullOrEmpty(audioPath))
            {
                IsTranscribing = false;
                return;
            }

            var transcription = await _speechToTextService.TranscribeAsync(audioPath);

            if (string.IsNullOrWhiteSpace(transcription))
            {
                await Shell.Current.DisplayAlert("Aviso", "Não foi possível transcrever o áudio.", "OK");
                return;
            }

            var commentInfo = new CommentInfo { NoteId = NoteId, Content = transcription };
            await _commentService.SaveAsync(commentInfo);
            await LoadCommentsAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao transcrever: {ex.Message}", "OK");
        }
        finally
        {
            IsTranscribing = false;
            RecordingStatus = string.Empty;
        }
    }

    [RelayCommand]
    private async Task ConsolidateWithAIAsync()
    {
        IsFabMenuOpen = false;

        if (Comments.Count == 0)
        {
            await Shell.Current.DisplayAlert("Aviso", "Não há comentários para consolidar.", "OK");
            return;
        }

        var confirm = await Shell.Current.DisplayAlert(
            "Consolidar com IA",
            "A IA irá integrar a nota com todos os comentários em um texto consolidado. Os comentários serão removidos após a consolidação. Deseja continuar?",
            "Sim", "Não");

        if (!confirm) return;

        IsConsolidating = true;

        try
        {
            var commentTexts = Comments.Select(c => c.Content).ToList();
            var consolidated = await _aiService.ConsolidateNoteAsync(Description, commentTexts);

            Description = consolidated;
            await _commentService.DeleteByNoteIdAsync(NoteId);
            Comments.Clear();

            var noteInfo = new NoteInfo
            {
                Id = NoteId,
                Title = Title,
                Description = Description,
                Category = Category,
                AudioFilePath = AudioFilePath
            };
            await _noteService.SaveAsync(noteInfo);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao consolidar: {ex.Message}", "OK");
        }
        finally
        {
            IsConsolidating = false;
        }
    }

    [RelayCommand]
    private async Task ImproveWithAIAsync()
    {
        IsFabMenuOpen = false;

        await EnsureNoteSavedAsync();

        IsImproving = true;

        try
        {
            var result = await _aiService.InterpretNoteAsync(Description);

            Title = result.Title;
            Description = result.Description;
            Category = result.Category;

            var noteInfo = new NoteInfo
            {
                Id = NoteId,
                Title = Title,
                Description = Description,
                Category = Category,
                AudioFilePath = AudioFilePath
            };
            await _noteService.SaveAsync(noteInfo);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao melhorar com IA: {ex.Message}", "OK");
        }
        finally
        {
            IsImproving = false;
        }
    }

    [RelayCommand]
    private async Task DeleteCommentAsync(CommentInfo comment)
    {
        await _commentService.DeleteAsync(comment.Id);
        Comments.Remove(comment);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Excluir nota",
            $"Deseja excluir \"{Title}\"?",
            "Sim", "Não");

        if (!confirm) return;

        await _commentService.DeleteByNoteIdAsync(NoteId);
        await _noteService.DeleteAsync(NoteId);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}

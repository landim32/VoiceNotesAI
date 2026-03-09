using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevNote.DTOs;
using DevNote.AppServices;
using DevNote.Helpers;

namespace DevNote.ViewModels;

public partial class RecordingViewModel : ObservableObject
{
    private readonly IAudioAppService _audioService;
    private readonly ISpeechToTextAppService _speechToTextService;
    private readonly INoteService _noteService;
    private readonly OpenAISettings _openAISettings;

    public RecordingViewModel(
        IAudioAppService audioService,
        ISpeechToTextAppService speechToTextService,
        INoteService noteService,
        OpenAISettings openAISettings)
    {
        _audioService = audioService;
        _speechToTextService = speechToTextService;
        _noteService = noteService;
        _openAISettings = openAISettings;
    }

    [ObservableProperty]
    private bool _isRecording;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private string _statusMessage = "Iniciando gravação...";

    [ObservableProperty]
    private string _audioFilePath = string.Empty;

    [ObservableProperty]
    private string _transcribedText = string.Empty;

    [RelayCommand]
    private async Task StartRecordingAsync()
    {
        if (string.IsNullOrWhiteSpace(_openAISettings.ApiKey))
        {
            await Shell.Current.DisplayAlert(
                "Configuração necessária",
                "A chave da API do OpenAI não está configurada. Configure-a no arquivo appsettings.json.",
                "OK");
            return;
        }

        var status = await Permissions.RequestAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
        {
            StatusMessage = "Permissão de microfone negada";
            return;
        }

        AudioFilePath = await _audioService.StartRecordingAsync();
        IsRecording = true;
        StatusMessage = "Gravando... Toque para parar";
    }

    [RelayCommand]
    private async Task StopRecordingAsync()
    {
        if (!IsRecording) return;

        AudioFilePath = await _audioService.StopRecordingAsync();
        IsRecording = false;

        await ProcessAudioAsync();
    }

    private async Task ProcessAudioAsync()
    {
        if (string.IsNullOrEmpty(AudioFilePath))
        {
            StatusMessage = "Nenhuma gravação encontrada";
            return;
        }

        IsProcessing = true;
        StatusMessage = "Transcrevendo áudio...";

        try
        {
            TranscribedText = await _speechToTextService.TranscribeAsync(AudioFilePath);

            StatusMessage = "Salvando nota...";
            var noteInfo = new NoteInfo
            {
                Description = TranscribedText,
                AudioFilePath = AudioFilePath
            };

            var saved = await _noteService.SaveAsync(noteInfo);

            var parameters = new Dictionary<string, object>
            {
                { "NoteInfo", saved }
            };

            await Shell.Current.GoToAsync("//NoteListPage/NoteDetailPage", parameters);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }
}

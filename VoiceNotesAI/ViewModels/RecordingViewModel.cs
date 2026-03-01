using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoiceNotesAI.Models;
using VoiceNotesAI.Services;

namespace VoiceNotesAI.ViewModels;

public partial class RecordingViewModel : ObservableObject
{
    private readonly IAudioService _audioService;
    private readonly ISpeechToTextService _speechToTextService;
    private readonly IAIService _aiService;

    public RecordingViewModel(
        IAudioService audioService,
        ISpeechToTextService speechToTextService,
        IAIService aiService)
    {
        _audioService = audioService;
        _speechToTextService = speechToTextService;
        _aiService = aiService;
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

            StatusMessage = "Interpretando com IA...";
            var result = await _aiService.InterpretNoteAsync(TranscribedText);

            var parameters = new Dictionary<string, object>
            {
                { "NoteResult", result },
                { "AudioFilePath", AudioFilePath },
                { "TranscribedText", TranscribedText }
            };

            await Shell.Current.GoToAsync("NoteResultPage", parameters);
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

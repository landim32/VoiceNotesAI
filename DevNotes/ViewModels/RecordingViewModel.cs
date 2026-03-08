using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevNotes.DTOs;
using DevNotes.AppServices;
using DevNotes.Helpers;
using DevNotes.Services.Interfaces;

namespace DevNotes.ViewModels;

public partial class RecordingViewModel : ObservableObject
{
    private readonly IAudioAppService _audioService;
    private readonly ISpeechToTextAppService _speechToTextService;
    private readonly INoteService _noteService;
    private readonly ISettingService _settingService;
    private readonly OpenAISettings _openAISettings;

    public RecordingViewModel(
        IAudioAppService audioService,
        ISpeechToTextAppService speechToTextService,
        INoteService noteService,
        ISettingService settingService,
        OpenAISettings openAISettings)
    {
        _audioService = audioService;
        _speechToTextService = speechToTextService;
        _noteService = noteService;
        _settingService = settingService;
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
        var apiKey = _openAISettings.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = await _settingService.GetAsync(SettingsViewModel.ApiKeySettingKey);

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            await Shell.Current.DisplayAlert(
                "Configuração necessária",
                "A chave da API do OpenAI não está configurada. Configure-a nas opções antes de gravar.",
                "Ir para Opções");
            await Shell.Current.GoToAsync("//SettingsPage");
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

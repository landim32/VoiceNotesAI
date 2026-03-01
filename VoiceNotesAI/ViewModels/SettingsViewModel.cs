using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoiceNotesAI.Helpers;
using VoiceNotesAI.Services;

namespace VoiceNotesAI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly OpenAISettings _openAISettings;

    public const string ApiKeySettingKey = "OpenAI_ApiKey";
    public const string WhisperModelSettingKey = "OpenAI_WhisperModel";
    public const string GptModelSettingKey = "OpenAI_Model";

    public SettingsViewModel(ISettingsRepository settingsRepository, OpenAISettings openAISettings)
    {
        _settingsRepository = settingsRepository;
        _openAISettings = openAISettings;
    }

    [ObservableProperty]
    private string _apiKey = string.Empty;

    [ObservableProperty]
    private string _whisperModel = string.Empty;

    [ObservableProperty]
    private string _gptModel = string.Empty;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private bool _isLoading;

    [RelayCommand]
    private async Task LoadSettingsAsync()
    {
        IsLoading = true;

        try
        {
            ApiKey = await _settingsRepository.GetAsync(ApiKeySettingKey) ?? _openAISettings.ApiKey;
            WhisperModel = await _settingsRepository.GetAsync(WhisperModelSettingKey) ?? _openAISettings.WhisperModel;
            GptModel = await _settingsRepository.GetAsync(GptModelSettingKey) ?? _openAISettings.Model;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            await Shell.Current.DisplayAlert("Erro", "A chave da API é obrigatória.", "OK");
            return;
        }

        IsSaving = true;

        try
        {
            await _settingsRepository.SetAsync(ApiKeySettingKey, ApiKey.Trim());
            await _settingsRepository.SetAsync(WhisperModelSettingKey, WhisperModel.Trim());
            await _settingsRepository.SetAsync(GptModelSettingKey, GptModel.Trim());

            // Update the in-memory settings so services pick up changes
            _openAISettings.ApiKey = ApiKey.Trim();
            _openAISettings.WhisperModel = WhisperModel.Trim();
            _openAISettings.Model = GptModel.Trim();

            await Shell.Current.DisplayAlert("Sucesso", "Configurações salvas com sucesso.", "OK");
        }
        finally
        {
            IsSaving = false;
        }
    }
}

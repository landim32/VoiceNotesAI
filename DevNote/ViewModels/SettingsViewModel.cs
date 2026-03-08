using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevNote.Helpers;
using DevNote.Services.Interfaces;

namespace DevNote.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingService _settingService;
    private readonly OpenAISettings _openAISettings;

    public const string ApiKeySettingKey = "OpenAI_ApiKey";
    public const string WhisperModelSettingKey = "OpenAI_WhisperModel";
    public const string GptModelSettingKey = "OpenAI_Model";

    public SettingsViewModel(ISettingService settingService, OpenAISettings openAISettings)
    {
        _settingService = settingService;
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

    public ObservableCollection<string> AvailableGptModels { get; } =
    [
        "gpt-4o",
        "gpt-4o-mini",
        "gpt-4.1",
        "gpt-4.1-mini",
        "gpt-4.1-nano",
        "gpt-4.5-preview",
        "o4-mini"
    ];

    public ObservableCollection<string> AvailableWhisperModels { get; } =
    [
        "gpt-4o-mini-transcribe",
        "gpt-4o-transcribe",
        "whisper-1"
    ];

    [RelayCommand]
    private async Task LoadSettingsAsync()
    {
        IsLoading = true;

        try
        {
            ApiKey = await _settingService.GetAsync(ApiKeySettingKey) ?? _openAISettings.ApiKey;
            WhisperModel = await _settingService.GetAsync(WhisperModelSettingKey) ?? _openAISettings.WhisperModel;
            GptModel = await _settingService.GetAsync(GptModelSettingKey) ?? _openAISettings.Model;
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
            await _settingService.SetAsync(ApiKeySettingKey, ApiKey.Trim());
            await _settingService.SetAsync(WhisperModelSettingKey, WhisperModel.Trim());
            await _settingService.SetAsync(GptModelSettingKey, GptModel.Trim());

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

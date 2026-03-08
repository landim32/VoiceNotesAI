using DevNote.Context;
using DevNote.Helpers;
using DevNote.Repository;
using DevNote.ViewModels;

namespace DevNote;

public partial class App : Application
{
    public App(AppDatabase database, ISettingsRepository settingsRepository, OpenAISettings openAISettings)
    {
        InitializeComponent();

        Task.Run(async () =>
        {
            await database.InitializeAsync();
            await LoadSettingsFromDbAsync(settingsRepository, openAISettings);
        });
    }

    private static async Task LoadSettingsFromDbAsync(ISettingsRepository settingsRepository, OpenAISettings openAISettings)
    {
        var apiKey = await settingsRepository.GetAsync(SettingsViewModel.ApiKeySettingKey);
        if (!string.IsNullOrWhiteSpace(apiKey))
            openAISettings.ApiKey = apiKey;

        var whisperModel = await settingsRepository.GetAsync(SettingsViewModel.WhisperModelSettingKey);
        if (!string.IsNullOrWhiteSpace(whisperModel))
            openAISettings.WhisperModel = whisperModel;

        var gptModel = await settingsRepository.GetAsync(SettingsViewModel.GptModelSettingKey);
        if (!string.IsNullOrWhiteSpace(gptModel))
            openAISettings.Model = gptModel;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}

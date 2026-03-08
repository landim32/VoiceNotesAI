using DevNotes.Repository;
using DevNotes.Services.Interfaces;

namespace DevNotes.Services;

public class SettingService : ISettingService
{
    private readonly ISettingsRepository _settingsRepository;

    public SettingService(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public Task<string?> GetAsync(string key)
    {
        return _settingsRepository.GetAsync(key);
    }

    public Task SetAsync(string key, string value)
    {
        return _settingsRepository.SetAsync(key, value);
    }

    public Task<Dictionary<string, string>> GetAllAsync()
    {
        return _settingsRepository.GetAllAsync();
    }
}

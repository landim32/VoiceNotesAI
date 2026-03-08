namespace DevNotes.Services.Interfaces;

public interface ISettingService
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    Task<Dictionary<string, string>> GetAllAsync();
}

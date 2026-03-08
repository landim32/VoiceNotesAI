using DevNote.Context;
using DevNote.Models;

namespace DevNote.Repository;

public class SettingsRepository : ISettingsRepository
{
    private readonly AppDatabase _database;

    public SettingsRepository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<string?> GetAsync(string key)
    {
        var setting = await _database.Connection
            .Table<AppSetting>()
            .Where(s => s.Key == key)
            .FirstOrDefaultAsync();

        return setting?.Value;
    }

    public async Task SetAsync(string key, string value)
    {
        var setting = new AppSetting { Key = key, Value = value };

        var existing = await _database.Connection
            .Table<AppSetting>()
            .Where(s => s.Key == key)
            .FirstOrDefaultAsync();

        if (existing != null)
            await _database.Connection.UpdateAsync(setting);
        else
            await _database.Connection.InsertAsync(setting);
    }

    public async Task<Dictionary<string, string>> GetAllAsync()
    {
        var settings = await _database.Connection
            .Table<AppSetting>()
            .ToListAsync();

        return settings.ToDictionary(s => s.Key, s => s.Value);
    }
}

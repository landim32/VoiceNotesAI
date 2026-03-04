using VoiceNotesAI.Context;
using VoiceNotesAI.Services;

namespace VoiceNotesAI.Tests.Services;

public class SettingsRepositoryTests : IAsyncLifetime
{
    private AppDatabase _database = null!;
    private SettingsRepository _repository = null!;
    private string _dbPath = null!;

    public async Task InitializeAsync()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"voicenotes_settings_test_{Guid.NewGuid()}.db3");
        _database = new AppDatabase(_dbPath);
        await _database.InitializeAsync();
        _repository = new SettingsRepository(_database);
    }

    public async Task DisposeAsync()
    {
        await _database.Connection.CloseAsync();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    [Fact]
    public async Task SetAsync_NewKey_ShouldInsert()
    {
        await _repository.SetAsync("test_key", "test_value");

        var result = await _repository.GetAsync("test_key");
        Assert.Equal("test_value", result);
    }

    [Fact]
    public async Task SetAsync_ExistingKey_ShouldUpdate()
    {
        await _repository.SetAsync("key1", "value1");
        await _repository.SetAsync("key1", "value2");

        var result = await _repository.GetAsync("key1");
        Assert.Equal("value2", result);
    }

    [Fact]
    public async Task GetAsync_NonExistingKey_ShouldReturnNull()
    {
        var result = await _repository.GetAsync("non_existing");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSettings()
    {
        await _repository.SetAsync("key_a", "value_a");
        await _repository.SetAsync("key_b", "value_b");

        var all = await _repository.GetAllAsync();

        Assert.True(all.Count >= 2);
        Assert.Equal("value_a", all["key_a"]);
        Assert.Equal("value_b", all["key_b"]);
    }

    [Fact]
    public async Task SetAsync_ApiKeyAndWhisperModel_ShouldPersist()
    {
        await _repository.SetAsync("OpenAI_ApiKey", "sk-test-123");
        await _repository.SetAsync("OpenAI_WhisperModel", "whisper-1");

        var apiKey = await _repository.GetAsync("OpenAI_ApiKey");
        var whisperModel = await _repository.GetAsync("OpenAI_WhisperModel");

        Assert.Equal("sk-test-123", apiKey);
        Assert.Equal("whisper-1", whisperModel);
    }
}

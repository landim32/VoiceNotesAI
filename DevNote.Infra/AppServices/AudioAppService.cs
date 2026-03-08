using Plugin.Maui.Audio;

namespace DevNote.AppServices;

public class AudioAppService : IAudioAppService
{
    private readonly IAudioManager _audioManager;
    private IAudioRecorder? _recorder;
    private string _currentFilePath = string.Empty;

    public AudioAppService(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public bool IsRecording => _recorder?.IsRecording ?? false;

    public async Task<string> StartRecordingAsync()
    {
        var fileName = $"{Guid.NewGuid()}.wav";
        var audioDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "audio");
        Directory.CreateDirectory(audioDir);
        _currentFilePath = Path.Combine(audioDir, fileName);

        _recorder = _audioManager.CreateRecorder();
        await _recorder.StartAsync(_currentFilePath);

        return _currentFilePath;
    }

    public async Task<string> StopRecordingAsync()
    {
        if (_recorder is null || !_recorder.IsRecording)
            return string.Empty;

        var source = await _recorder.StopAsync();
        if (source is IDisposable disposable)
            disposable.Dispose();

        return _currentFilePath;
    }
}

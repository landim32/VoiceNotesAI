namespace DevNote.AppServices;

public interface IAudioAppService
{
    bool IsRecording { get; }
    Task<string> StartRecordingAsync();
    Task<string> StopRecordingAsync();
}

namespace VoiceNotesAI.Services;

public interface IAudioService
{
    bool IsRecording { get; }
    Task<string> StartRecordingAsync();
    Task<string> StopRecordingAsync();
}

namespace VoiceNotesAI.Services;

public interface ISpeechToTextService
{
    Task<string> TranscribeAsync(string audioFilePath);
}

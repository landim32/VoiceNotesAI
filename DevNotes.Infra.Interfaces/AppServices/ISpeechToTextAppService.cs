namespace DevNotes.AppServices;

public interface ISpeechToTextAppService
{
    Task<string> TranscribeAsync(string audioFilePath);
}

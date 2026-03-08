namespace DevNotes.Helpers;

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o";
    public string WhisperModel { get; set; } = "whisper-1";
}

using System.Net.Http.Headers;
using System.Text.Json;

namespace VoiceNotesAI.Services;

public class SpeechToTextService : ISpeechToTextService
{
    private const string WhisperEndpoint = "https://api.openai.com/v1/audio/transcriptions";

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public SpeechToTextService(HttpClient httpClient, string apiKey, string model = "whisper-1")
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
        _model = model;
    }

    public async Task<string> TranscribeAsync(string audioFilePath)
    {
        if (!File.Exists(audioFilePath))
            throw new FileNotFoundException("Audio file not found.", audioFilePath);

        using var request = new HttpRequestMessage(HttpMethod.Post, WhisperEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        using var content = new MultipartFormDataContent();
        var audioBytes = await File.ReadAllBytesAsync(audioFilePath);
        var audioContent = new ByteArrayContent(audioBytes);
        audioContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
        content.Add(audioContent, "file", Path.GetFileName(audioFilePath));
        content.Add(new StringContent(_model), "model");
        content.Add(new StringContent("pt"), "language");

        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement.GetProperty("text").GetString() ?? string.Empty;
    }
}

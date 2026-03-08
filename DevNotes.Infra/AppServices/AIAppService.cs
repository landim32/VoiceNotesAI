using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DevNotes.Helpers;
using DevNotes.DTOs;

namespace DevNotes.AppServices;

public class AIAppService : IAIAppService
{
    private const string ChatEndpoint = "https://api.openai.com/v1/chat/completions";

    private readonly HttpClient _httpClient;
    private readonly OpenAISettings _settings;

    public AIAppService(HttpClient httpClient, OpenAISettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<NoteResult> InterpretNoteAsync(string transcribedText)
    {
        var prompt = PromptTemplates.BuildNotePrompt(transcribedText);

        var requestBody = new
        {
            model = _settings.Model,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            temperature = 0.3
        };

        var json = JsonSerializer.Serialize(requestBody);

        using var request = new HttpRequestMessage(HttpMethod.Post, ChatEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        var messageContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(messageContent))
            throw new InvalidOperationException("AI returned an empty response.");

        var result = JsonSerializer.Deserialize<NoteResult>(messageContent);

        return result ?? throw new InvalidOperationException("Failed to deserialize AI response.");
    }

    public async Task<string> ConsolidateNoteAsync(string noteContent, List<string> comments)
    {
        var prompt = PromptTemplates.BuildConsolidationPrompt(noteContent, comments);

        var requestBody = new
        {
            model = _settings.Model,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            temperature = 0.3
        };

        var json = JsonSerializer.Serialize(requestBody);

        using var request = new HttpRequestMessage(HttpMethod.Post, ChatEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        var messageContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(messageContent))
            throw new InvalidOperationException("AI returned an empty response.");

        return messageContent;
    }
}

using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;
using VoiceNotesAI.Helpers;
using VoiceNotesAI.Services;

namespace VoiceNotesAI.Tests.Services;

public class AIServiceTests
{
    private static OpenAISettings CreateSettings(string apiKey = "test-key", string model = "gpt-4o")
    {
        return new OpenAISettings { ApiKey = apiKey, Model = model };
    }

    private static HttpClient CreateMockHttpClient(HttpStatusCode statusCode, string responseContent)
    {
        var handler = new Mock<HttpMessageHandler>();
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            });

        return new HttpClient(handler.Object);
    }

    private static string BuildOpenAIResponse(string title, string description, string category)
    {
        var noteResultJson = JsonSerializer.Serialize(new
        {
            title,
            description,
            category
        });

        return JsonSerializer.Serialize(new
        {
            choices = new[]
            {
                new
                {
                    message = new
                    {
                        content = noteResultJson
                    }
                }
            }
        });
    }

    [Fact]
    public async Task InterpretNoteAsync_ValidResponse_ShouldReturnNoteResult()
    {
        var responseJson = BuildOpenAIResponse(
            "Comprar leite",
            "Lembrar de comprar leite no mercado",
            "Tarefas");

        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, responseJson);
        var service = new AIService(httpClient, CreateSettings());

        var result = await service.InterpretNoteAsync("preciso comprar leite");

        Assert.Equal("Comprar leite", result.Title);
        Assert.Equal("Lembrar de comprar leite no mercado", result.Description);
        Assert.Equal("Tarefas", result.Category);
    }

    [Fact]
    public async Task InterpretNoteAsync_ShouldSendAuthorizationHeader()
    {
        var responseJson = BuildOpenAIResponse("Título", "Desc", "Ideias");

        HttpRequestMessage? capturedRequest = null;
        var handler = new Mock<HttpMessageHandler>();
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(handler.Object);
        var service = new AIService(httpClient, CreateSettings(apiKey: "my-secret-key"));

        await service.InterpretNoteAsync("texto teste");

        Assert.NotNull(capturedRequest);
        Assert.Equal("Bearer", capturedRequest.Headers.Authorization?.Scheme);
        Assert.Equal("my-secret-key", capturedRequest.Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task InterpretNoteAsync_ShouldSendCorrectModel()
    {
        var responseJson = BuildOpenAIResponse("T", "D", "Outros");

        string? capturedBody = null;
        var handler = new Mock<HttpMessageHandler>();
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>(async (req, _) =>
            {
                capturedBody = await req.Content!.ReadAsStringAsync();
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(handler.Object);
        var service = new AIService(httpClient, CreateSettings(model: "gpt-4o-mini"));

        await service.InterpretNoteAsync("teste");

        Assert.NotNull(capturedBody);
        Assert.Contains("gpt-4o-mini", capturedBody);
    }

    [Fact]
    public async Task InterpretNoteAsync_ApiError_ShouldThrowHttpRequestException()
    {
        var httpClient = CreateMockHttpClient(HttpStatusCode.Unauthorized, "Unauthorized");
        var service = new AIService(httpClient, CreateSettings(apiKey: "invalid-key"));

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.InterpretNoteAsync("texto"));
    }

    [Fact]
    public async Task InterpretNoteAsync_EmptyContent_ShouldThrowInvalidOperationException()
    {
        var responseJson = JsonSerializer.Serialize(new
        {
            choices = new[]
            {
                new
                {
                    message = new
                    {
                        content = (string?)null
                    }
                }
            }
        });

        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, responseJson);
        var service = new AIService(httpClient, CreateSettings());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.InterpretNoteAsync("texto"));
    }

    [Fact]
    public async Task InterpretNoteAsync_InvalidJson_ShouldThrowJsonException()
    {
        var responseJson = JsonSerializer.Serialize(new
        {
            choices = new[]
            {
                new
                {
                    message = new
                    {
                        content = "this is not valid JSON"
                    }
                }
            }
        });

        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, responseJson);
        var service = new AIService(httpClient, CreateSettings());

        await Assert.ThrowsAsync<JsonException>(
            () => service.InterpretNoteAsync("texto"));
    }
}

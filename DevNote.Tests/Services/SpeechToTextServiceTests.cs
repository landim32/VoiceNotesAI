using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using DevNote.Helpers;
using DevNote.AppServices;

namespace DevNote.Tests.Services;

public class SpeechToTextServiceTests
{
    private static OpenAISettings CreateSettings(string apiKey = "test-key", string whisperModel = "whisper-1")
    {
        return new OpenAISettings { ApiKey = apiKey, WhisperModel = whisperModel };
    }

    [Fact]
    public async Task TranscribeAsync_FileNotFound_ShouldThrowFileNotFoundException()
    {
        var httpClient = new HttpClient();
        var service = new SpeechToTextAppService(httpClient, CreateSettings());

        await Assert.ThrowsAsync<FileNotFoundException>(
            () => service.TranscribeAsync("/path/that/does/not/exist.wav"));
    }

    [Fact]
    public async Task TranscribeAsync_ValidFile_ShouldReturnTranscribedText()
    {
        var tempFile = Path.GetTempFileName();
        await File.WriteAllBytesAsync(tempFile, new byte[] { 0x00, 0x01, 0x02 });

        try
        {
            var responseJson = """{"text": "Olá, este é um teste de transcrição"}""";

            var handler = new Mock<HttpMessageHandler>();
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(handler.Object);
            var service = new SpeechToTextAppService(httpClient, CreateSettings());

            var result = await service.TranscribeAsync(tempFile);

            Assert.Equal("Olá, este é um teste de transcrição", result);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task TranscribeAsync_ShouldSendAuthorizationHeader()
    {
        var tempFile = Path.GetTempFileName();
        await File.WriteAllBytesAsync(tempFile, new byte[] { 0x00 });

        try
        {
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
                    Content = new StringContent("""{"text": "teste"}""", Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(handler.Object);
            var service = new SpeechToTextAppService(httpClient, CreateSettings(apiKey: "my-whisper-key"));

            await service.TranscribeAsync(tempFile);

            Assert.NotNull(capturedRequest);
            Assert.Equal("Bearer", capturedRequest.Headers.Authorization?.Scheme);
            Assert.Equal("my-whisper-key", capturedRequest.Headers.Authorization?.Parameter);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task TranscribeAsync_ApiError_ShouldThrowHttpRequestException()
    {
        var tempFile = Path.GetTempFileName();
        await File.WriteAllBytesAsync(tempFile, new byte[] { 0x00 });

        try
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
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("Server Error")
                });

            var httpClient = new HttpClient(handler.Object);
            var service = new SpeechToTextAppService(httpClient, CreateSettings());

            await Assert.ThrowsAsync<HttpRequestException>(
                () => service.TranscribeAsync(tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}

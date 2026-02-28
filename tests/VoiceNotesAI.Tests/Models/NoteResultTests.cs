using System.Text.Json;
using VoiceNotesAI.Models;

namespace VoiceNotesAI.Tests.Models;

public class NoteResultTests
{
    [Fact]
    public void NoteResult_DefaultValues_ShouldBeEmpty()
    {
        var result = new NoteResult();

        Assert.Equal(string.Empty, result.Title);
        Assert.Equal(string.Empty, result.Description);
        Assert.Equal(string.Empty, result.Category);
    }

    [Fact]
    public void NoteResult_Deserialize_ShouldMapJsonProperties()
    {
        var json = """
        {
            "title": "Comprar leite",
            "description": "Lembrar de comprar leite no mercado depois do trabalho",
            "category": "Tarefas"
        }
        """;

        var result = JsonSerializer.Deserialize<NoteResult>(json);

        Assert.NotNull(result);
        Assert.Equal("Comprar leite", result.Title);
        Assert.Equal("Lembrar de comprar leite no mercado depois do trabalho", result.Description);
        Assert.Equal("Tarefas", result.Category);
    }

    [Fact]
    public void NoteResult_Serialize_ShouldUseJsonPropertyNames()
    {
        var result = new NoteResult
        {
            Title = "Ideia de app",
            Description = "Criar um app de notas por voz com IA",
            Category = "Ideias"
        };

        var json = JsonSerializer.Serialize(result);

        Assert.Contains("\"title\":", json);
        Assert.Contains("\"description\":", json);
        Assert.Contains("\"category\":", json);
        Assert.DoesNotContain("\"Title\":", json);
    }

    [Fact]
    public void NoteResult_Deserialize_WithMissingFields_ShouldUseDefaults()
    {
        var json = """{ "title": "Apenas título" }""";

        var result = JsonSerializer.Deserialize<NoteResult>(json);

        Assert.NotNull(result);
        Assert.Equal("Apenas título", result.Title);
        Assert.Equal(string.Empty, result.Description);
        Assert.Equal(string.Empty, result.Category);
    }
}

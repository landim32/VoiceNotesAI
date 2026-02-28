using VoiceNotesAI.Helpers;

namespace VoiceNotesAI.Tests.Helpers;

public class PromptTemplatesTests
{
    [Fact]
    public void BuildNotePrompt_ShouldReplaceTranscribedTextPlaceholder()
    {
        var transcribed = "Preciso comprar pão amanhã cedo";

        var prompt = PromptTemplates.BuildNotePrompt(transcribed);

        Assert.Contains(transcribed, prompt);
        Assert.DoesNotContain("{{TRANSCRIBED_TEXT}}", prompt);
    }

    [Fact]
    public void BuildNotePrompt_ShouldContainJsonStructureInstruction()
    {
        var prompt = PromptTemplates.BuildNotePrompt("teste");

        Assert.Contains("title", prompt);
        Assert.Contains("description", prompt);
        Assert.Contains("category", prompt);
    }

    [Fact]
    public void BuildNotePrompt_ShouldContainAllValidCategories()
    {
        var prompt = PromptTemplates.BuildNotePrompt("teste");

        Assert.Contains("Tarefas", prompt);
        Assert.Contains("Ideias", prompt);
        Assert.Contains("Lembretes", prompt);
        Assert.Contains("Trabalho", prompt);
        Assert.Contains("Pessoal", prompt);
        Assert.Contains("Outros", prompt);
    }

    [Fact]
    public void BuildNotePrompt_WithEmptyText_ShouldStillBuildPrompt()
    {
        var prompt = PromptTemplates.BuildNotePrompt(string.Empty);

        Assert.DoesNotContain("{{TRANSCRIBED_TEXT}}", prompt);
        Assert.Contains("note-taking assistant", prompt);
    }

    [Fact]
    public void NoteInterpretation_ShouldContainPlaceholder()
    {
        Assert.Contains("{{TRANSCRIBED_TEXT}}", PromptTemplates.NoteInterpretation);
    }
}

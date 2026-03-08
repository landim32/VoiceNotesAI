using DevNote.Helpers;

namespace DevNote.Tests.Helpers;

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

        Assert.Contains("Tasks", prompt);
        Assert.Contains("Ideias", prompt);
        Assert.Contains("Prompts", prompt);
        Assert.Contains("Issues", prompt);
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

    [Fact]
    public void BuildConsolidationPrompt_ShouldReplaceNoteContentPlaceholder()
    {
        var noteContent = "Minha nota sobre reunião";
        var comments = new List<string> { "Adicionar item X", "Revisar item Y" };

        var prompt = PromptTemplates.BuildConsolidationPrompt(noteContent, comments);

        Assert.Contains(noteContent, prompt);
        Assert.DoesNotContain("{{NOTE_CONTENT}}", prompt);
    }

    [Fact]
    public void BuildConsolidationPrompt_ShouldReplaceCommentsPlaceholder()
    {
        var comments = new List<string> { "Primeiro comentário", "Segundo comentário" };

        var prompt = PromptTemplates.BuildConsolidationPrompt("Nota", comments);

        Assert.Contains("Primeiro comentário", prompt);
        Assert.Contains("Segundo comentário", prompt);
        Assert.DoesNotContain("{{COMMENTS}}", prompt);
    }

    [Fact]
    public void BuildConsolidationPrompt_WithEmptyComments_ShouldStillBuildPrompt()
    {
        var prompt = PromptTemplates.BuildConsolidationPrompt("Nota teste", new List<string>());

        Assert.Contains("Nota teste", prompt);
        Assert.Contains("Nenhum comentário", prompt);
        Assert.DoesNotContain("{{COMMENTS}}", prompt);
    }

    [Fact]
    public void NoteConsolidation_ShouldContainPlaceholders()
    {
        Assert.Contains("{{NOTE_CONTENT}}", PromptTemplates.NoteConsolidation);
        Assert.Contains("{{COMMENTS}}", PromptTemplates.NoteConsolidation);
    }
}

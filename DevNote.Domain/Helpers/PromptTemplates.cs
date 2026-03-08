namespace DevNote.Helpers;

public static class PromptTemplates
{
    public const string NoteInterpretation = """
        You are an intelligent note-taking assistant.

        The user will provide a transcribed audio text in Portuguese (Brazil).

        Your job is to:
        1. Understand the content and intent of the message
        2. Generate a structured note from it

        Return ONLY a valid JSON object with this exact structure:

        {
          "title": "Short and objective title (max 60 characters)",
          "description": "Full and clear description of the note, preserving the original meaning",
          "category": "One of: Tasks | Ideias | Prompts | Issues"
        }

        Rules:
        - Do NOT add any explanation outside the JSON
        - Do NOT wrap in markdown code blocks
        - Title must be concise and meaningful
        - Category must match exactly one of the allowed values
        - Always respond in Portuguese (Brazil)
        - If the content is unclear, do your best to interpret the intent

        Transcribed text:
        {{TRANSCRIBED_TEXT}}
        """;

    public const string NoteConsolidation = """
        Você é um assistente inteligente de notas.

        O usuário fornecerá o conteúdo de uma nota e uma lista de comentários adicionados posteriormente.

        Sua tarefa é:
        1. Integrar o conteúdo original da nota com todos os comentários
        2. Produzir uma nota consolidada, coesa e bem organizada
        3. Preservar todas as informações relevantes
        4. Manter a linguagem natural em Português (Brasil)

        Regras:
        - Retorne APENAS o texto consolidado da nota, sem explicações adicionais
        - NÃO use blocos de código markdown
        - NÃO adicione cabeçalhos ou formatação especial
        - Mantenha o tom e estilo do conteúdo original
        - Integre os comentários de forma natural no texto

        Conteúdo da nota:
        {{NOTE_CONTENT}}

        Comentários:
        {{COMMENTS}}
        """;

    public static string BuildNotePrompt(string transcribedText)
    {
        return NoteInterpretation.Replace("{{TRANSCRIBED_TEXT}}", transcribedText);
    }

    public static string BuildConsolidationPrompt(string noteContent, List<string> comments)
    {
        var commentsText = comments.Count > 0
            ? string.Join("\n", comments.Select((c, i) => $"{i + 1}. {c}"))
            : "Nenhum comentário.";

        return NoteConsolidation
            .Replace("{{NOTE_CONTENT}}", noteContent)
            .Replace("{{COMMENTS}}", commentsText);
    }
}

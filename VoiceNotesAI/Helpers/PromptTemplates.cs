namespace VoiceNotesAI.Helpers;

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
          "category": "One of: Tarefas | Ideias | Lembretes | Trabalho | Pessoal | Outros"
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

    public static string BuildNotePrompt(string transcribedText)
    {
        return NoteInterpretation.Replace("{{TRANSCRIBED_TEXT}}", transcribedText);
    }
}

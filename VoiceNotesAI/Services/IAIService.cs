using VoiceNotesAI.Models;

namespace VoiceNotesAI.Services;

public interface IAIService
{
    Task<NoteResult> InterpretNoteAsync(string transcribedText);
}

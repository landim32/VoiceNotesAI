using DevNotes.DTOs;

namespace DevNotes.AppServices;

public interface IAIAppService
{
    Task<NoteResult> InterpretNoteAsync(string transcribedText);
    Task<string> ConsolidateNoteAsync(string noteContent, List<string> comments);
}

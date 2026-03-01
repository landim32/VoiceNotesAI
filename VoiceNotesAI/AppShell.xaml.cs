using VoiceNotesAI.Pages;

namespace VoiceNotesAI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("RecordingPage", typeof(RecordingPage));
        Routing.RegisterRoute("NoteResultPage", typeof(NoteResultPage));
        Routing.RegisterRoute("NoteDetailPage", typeof(NoteDetailPage));
        Routing.RegisterRoute("CategoryDetailPage", typeof(CategoryDetailPage));
    }
}

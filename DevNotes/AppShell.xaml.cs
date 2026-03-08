using DevNotes.Pages;

namespace DevNotes;

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

    private async void OnNotaDeVozTapped(object? sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToAsync("//NoteListPage/RecordingPage");
    }

    private async void OnNotaDeTextoTapped(object? sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToAsync("//NoteListPage/NoteDetailPage");
    }

    private async void OnMinhasNotasTapped(object? sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToAsync("//NoteListPage");
    }

    private async void OnCategoriasTapped(object? sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToAsync("//CategoryListPage");
    }

    private async void OnOpcoesTapped(object? sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToAsync("//SettingsPage");
    }
}

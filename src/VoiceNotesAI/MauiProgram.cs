using Microsoft.Extensions.Logging;
using VoiceNotesAI.Data;
using VoiceNotesAI.Services;

namespace VoiceNotesAI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Database
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "voicenotesai.db3");
        builder.Services.AddSingleton(new AppDatabase(dbPath));

        // Repositories
        builder.Services.AddSingleton<INoteRepository, NoteRepository>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

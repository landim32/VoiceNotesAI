using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using DevNotes.Context;
using DevNotes.Helpers;
using DevNotes.Pages;
using DevNotes.ViewModels;

namespace DevNotes;

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

        // Configuration from embedded appsettings.json
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("DevNotes.appsettings.json");

        var config = new ConfigurationBuilder()
            .AddJsonStream(stream!)
            .Build();

        // Settings
        var openAISettings = new OpenAISettings();
        config.GetSection("OpenAI").Bind(openAISettings);
        builder.Services.AddSingleton(openAISettings);

        // Database
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "devnotes.db3");
        builder.Services.AddSingleton(new AppDatabase(dbPath));

        // Audio (MAUI-specific)
        builder.Services.AddSingleton(AudioManager.Current);

        // Application Services (Repositories, AppServices, Domain Services)
        builder.Services.AddApplicationServices();

        // ViewModels
        builder.Services.AddTransient<NoteListViewModel>();
        builder.Services.AddTransient<RecordingViewModel>();
        builder.Services.AddTransient<NoteResultViewModel>();
        builder.Services.AddTransient<NoteDetailViewModel>();
        builder.Services.AddTransient<CategoryListViewModel>();
        builder.Services.AddTransient<CategoryDetailViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        // Pages
        builder.Services.AddTransient<NoteListPage>();
        builder.Services.AddTransient<RecordingPage>();
        builder.Services.AddTransient<NoteResultPage>();
        builder.Services.AddTransient<NoteDetailPage>();
        builder.Services.AddTransient<CategoryListPage>();
        builder.Services.AddTransient<CategoryDetailPage>();
        builder.Services.AddTransient<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

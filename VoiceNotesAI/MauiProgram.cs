using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using VoiceNotesAI.Data;
using VoiceNotesAI.Helpers;
using VoiceNotesAI.Services;
using VoiceNotesAI.Pages;
using VoiceNotesAI.ViewModels;

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

        // Configuration from embedded appsettings.json
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("VoiceNotesAI.appsettings.json");

        var config = new ConfigurationBuilder()
            .AddJsonStream(stream!)
            .Build();

        // Settings
        var openAISettings = new OpenAISettings();
        config.GetSection("OpenAI").Bind(openAISettings);
        builder.Services.AddSingleton(openAISettings);

        // Database
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "voicenotesai.db3");
        builder.Services.AddSingleton(new AppDatabase(dbPath));

        // HttpClient
        builder.Services.AddSingleton<HttpClient>();

        // Audio
        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddSingleton<IAudioService, AudioService>();

        // AI Services
        builder.Services.AddSingleton<ISpeechToTextService>(sp =>
        {
            var http = sp.GetRequiredService<HttpClient>();
            var settings = sp.GetRequiredService<OpenAISettings>();
            return new SpeechToTextService(http, settings.ApiKey, settings.WhisperModel);
        });

        builder.Services.AddSingleton<IAIService>(sp =>
        {
            var http = sp.GetRequiredService<HttpClient>();
            var settings = sp.GetRequiredService<OpenAISettings>();
            return new AIService(http, settings.ApiKey, settings.Model);
        });

        // Repositories
        builder.Services.AddSingleton<INoteRepository, NoteRepository>();

        // ViewModels
        builder.Services.AddTransient<NoteListViewModel>();
        builder.Services.AddTransient<RecordingViewModel>();
        builder.Services.AddTransient<NoteResultViewModel>();
        builder.Services.AddTransient<NoteDetailViewModel>();

        // Pages
        builder.Services.AddTransient<NoteListPage>();
        builder.Services.AddTransient<RecordingPage>();
        builder.Services.AddTransient<NoteResultPage>();
        builder.Services.AddTransient<NoteDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

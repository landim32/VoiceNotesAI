using Microsoft.Extensions.DependencyInjection;
using DevNotes.AppServices;
using DevNotes.Context;
using DevNotes.Repository;
using DevNotes.Services;
using DevNotes.Services.Interfaces;

namespace DevNotes;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(AppDatabase).Assembly);

        // HttpClient
        services.AddSingleton<HttpClient>();

        // AppServices
        services.AddSingleton<IAudioAppService, AudioAppService>();
        services.AddSingleton<ISpeechToTextAppService, SpeechToTextAppService>();
        services.AddSingleton<IAIAppService, AIAppService>();

        // Repositories
        services.AddSingleton<INoteRepository, NoteRepository>();
        services.AddSingleton<ICategoryRepository, CategoryRepository>();
        services.AddSingleton<ISettingsRepository, SettingsRepository>();
        services.AddSingleton<ICommentRepository, CommentRepository>();

        // Domain Services
        services.AddSingleton<INoteService, NoteService>();
        services.AddSingleton<ICategoryService, CategoryService>();
        services.AddSingleton<ICommentService, CommentService>();
        services.AddSingleton<ISettingService, SettingService>();

        return services;
    }
}

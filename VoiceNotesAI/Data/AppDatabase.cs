using SQLite;
using VoiceNotesAI.Models;

namespace VoiceNotesAI.Data;

public class AppDatabase
{
    private readonly SQLiteAsyncConnection _database;

    public AppDatabase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
    }

    public SQLiteAsyncConnection Connection => _database;

    public async Task InitializeAsync()
    {
        await _database.CreateTableAsync<Note>();
        await _database.CreateTableAsync<Category>();

        await SeedCategoriesAsync();
    }

    private async Task SeedCategoriesAsync()
    {
        var count = await _database.Table<Category>().CountAsync();
        if (count > 0)
            return;

        var defaultCategories = new[]
        {
            "Tarefas",
            "Ideias",
            "Lembretes",
            "Trabalho",
            "Pessoal",
            "Outros"
        };

        foreach (var name in defaultCategories)
        {
            await _database.InsertAsync(new Category { Name = name });
        }
    }
}

using SQLite;

namespace VoiceNotesAI.Models;

public class Category
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

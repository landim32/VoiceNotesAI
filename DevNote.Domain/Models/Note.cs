using SQLite;

namespace DevNote.Models;

public class Note
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string AudioFilePath { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Description))
            throw new InvalidOperationException("O conteúdo da nota é obrigatório.");

        if (Title.Length > 200)
            throw new InvalidOperationException("O título não pode ter mais de 200 caracteres.");
    }

    public void Update(string title, string description, string category, string audioFilePath)
    {
        Title = title;
        Description = description;
        Category = category;
        AudioFilePath = audioFilePath;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Note Create(string title, string description, string category, string audioFilePath = "")
    {
        var resolvedTitle = string.IsNullOrWhiteSpace(title)
            ? (string.IsNullOrWhiteSpace(description)
                ? "Nova Nota"
                : description.Length > 80 ? description[..80] + "..." : description)
            : title;

        var note = new Note
        {
            Title = resolvedTitle,
            Description = description,
            Category = category,
            AudioFilePath = audioFilePath,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        note.Validate();
        return note;
    }
}

using SQLite;

namespace DevNotes.Models;

public class Category
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new InvalidOperationException("O nome da categoria é obrigatório.");

        if (Name.Length > 200)
            throw new InvalidOperationException("O nome da categoria não pode ter mais de 200 caracteres.");
    }

    public void Update(string name)
    {
        Name = name;
    }

    public static Category Create(string name)
    {
        var category = new Category { Name = name.Trim() };
        category.Validate();
        return category;
    }
}

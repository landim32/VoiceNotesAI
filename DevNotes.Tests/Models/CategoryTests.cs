using DevNotes.Models;

namespace DevNotes.Tests.Models;

public class CategoryTests
{
    [Fact]
    public void Validate_EmptyName_ShouldThrow()
    {
        var category = new Category { Name = "" };

        var ex = Assert.Throws<InvalidOperationException>(() => category.Validate());
        Assert.Contains("nome da categoria", ex.Message);
    }

    [Fact]
    public void Validate_NameTooLong_ShouldThrow()
    {
        var category = new Category { Name = new string('A', 201) };

        var ex = Assert.Throws<InvalidOperationException>(() => category.Validate());
        Assert.Contains("200 caracteres", ex.Message);
    }

    [Fact]
    public void Validate_ValidName_ShouldNotThrow()
    {
        var category = new Category { Name = "Trabalho" };

        category.Validate();
    }

    [Fact]
    public void Create_ValidName_ShouldSetProperties()
    {
        var category = Category.Create("  Trabalho  ");

        Assert.Equal("Trabalho", category.Name);
    }

    [Fact]
    public void Create_EmptyName_ShouldThrow()
    {
        Assert.Throws<InvalidOperationException>(() => Category.Create(""));
    }

    [Fact]
    public void Update_ShouldSetName()
    {
        var category = Category.Create("Original");

        category.Update("Atualizado");

        Assert.Equal("Atualizado", category.Name);
    }
}

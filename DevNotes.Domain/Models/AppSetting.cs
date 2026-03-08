using SQLite;

namespace DevNotes.Models;

public class AppSetting
{
    [PrimaryKey]
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Key))
            throw new InvalidOperationException("A chave da configuração é obrigatória.");
    }

    public void Update(string value)
    {
        Value = value;
    }

    public static AppSetting Create(string key, string value)
    {
        var setting = new AppSetting { Key = key, Value = value };
        setting.Validate();
        return setting;
    }
}

---
name: dotnet-arch-entity
description: Guides the implementation of a new entity following the MVVM pattern of this project. Covers all layers from Model to Page, including SQLite table, Repository pattern, ViewModel, Page, and DI registration. Use when creating or modifying entities, adding new tables, or scaffolding CRUD features.
allowed-tools: Read, Grep, Glob, Bash, Write, Edit, Task
---

# .NET MAUI MVVM — Entity Implementation Guide

You are an expert assistant that helps developers create or modify entities following the exact architecture patterns of this VoiceNotesAI project. You guide the user through ALL required layers.

## Input

The user will describe the entity to create or modify: `$ARGUMENTS`

Before generating code, read existing files (use Note/Category as primary reference) to match current patterns exactly.

---

## Architecture & Data Flow

```
Page (XAML) → ViewModel (MVVM Toolkit) → Service/Repository → AppDatabase → SQLite
```

**Project:** Single .NET MAUI project (`VoiceNotesAI`) with test project (`VoiceNotesAI.Tests`)

**Key Directories:**
- `VoiceNotesAI/Models/` — SQLite entities and DTOs
- `VoiceNotesAI/Services/` — Interfaces and implementations (repository, API services)
- `VoiceNotesAI/ViewModels/` — MVVM ViewModels with CommunityToolkit.Mvvm
- `VoiceNotesAI/Pages/` — XAML pages with code-behind
- `VoiceNotesAI/Data/` — AppDatabase (SQLite wrapper)

---

## Step-by-Step Implementation

### Step 1: Model — `VoiceNotesAI/Models/{Entity}.cs`

```csharp
using SQLite;

namespace VoiceNotesAI.Models;

public class {Entity}
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    // Add entity-specific properties
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

Conventions:
- File-scoped namespace
- `[PrimaryKey, AutoIncrement]` on `Id`
- Default values via `= string.Empty` or `= DateTime.UtcNow`
- No navigation properties (SQLite-net doesn't support them)

### Step 2: Database Table — Modify `VoiceNotesAI/Data/AppDatabase.cs`

Add table creation in `InitializeAsync()`:

```csharp
await _database.CreateTableAsync<{Entity}>();
```

Add seed data method if needed (see `SeedCategoriesAsync()` as reference).

### Step 3: Repository Interface — `VoiceNotesAI/Services/I{Entity}Repository.cs`

```csharp
using VoiceNotesAI.Models;

namespace VoiceNotesAI.Services;

public interface I{Entity}Repository
{
    Task<List<{Entity}>> GetAllAsync();
    Task<{Entity}?> GetByIdAsync(int id);
    Task<int> SaveAsync({Entity} entity);
    Task<int> DeleteAsync(int id);
}
```

Convention: Async methods, nullable return for `GetByIdAsync`.

### Step 4: Repository Implementation — `VoiceNotesAI/Services/{Entity}Repository.cs`

```csharp
using VoiceNotesAI.Data;
using VoiceNotesAI.Models;

namespace VoiceNotesAI.Services;

public class {Entity}Repository : I{Entity}Repository
{
    private readonly AppDatabase _database;

    public {Entity}Repository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<{Entity}>> GetAllAsync()
    {
        return await _database.Connection
            .Table<{Entity}>()
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<{Entity}?> GetByIdAsync(int id)
    {
        return await _database.Connection
            .Table<{Entity}>()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<int> SaveAsync({Entity} entity)
    {
        if (entity.Id != 0)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            return await _database.Connection.UpdateAsync(entity);
        }
        else
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            return await _database.Connection.InsertAsync(entity);
        }
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _database.Connection.DeleteAsync<{Entity}>(id);
    }
}
```

Convention: Insert vs Update decided by `Id != 0`. Uses `_database.Connection` (SQLiteAsyncConnection).

### Step 5: ViewModel — `VoiceNotesAI/ViewModels/{Entity}ListViewModel.cs`

```csharp
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoiceNotesAI.Models;
using VoiceNotesAI.Services;

namespace VoiceNotesAI.ViewModels;

public partial class {Entity}ListViewModel : ObservableObject
{
    private readonly I{Entity}Repository _{entity}Repository;

    public {Entity}ListViewModel(I{Entity}Repository {entity}Repository)
    {
        _{entity}Repository = {entity}Repository;
    }

    [ObservableProperty]
    private ObservableCollection<{Entity}> _items = [];

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        IsLoading = true;
        var items = await _{entity}Repository.GetAllAsync();
        Items = new ObservableCollection<{Entity}>(items);
        IsEmpty = Items.Count == 0;
        IsLoading = false;
    }

    [RelayCommand]
    private async Task DeleteAsync({Entity} item)
    {
        await _{entity}Repository.DeleteAsync(item.Id);
        Items.Remove(item);
        IsEmpty = Items.Count == 0;
    }
}
```

For detail/edit ViewModel, implement `IQueryAttributable`:

```csharp
public partial class {Entity}DetailViewModel : ObservableObject, IQueryAttributable
{
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        // Receive navigation parameters
    }
}
```

Conventions:
- `[ObservableProperty]` with `_camelCase` private fields
- `[RelayCommand]` with `Async` suffix on async methods
- `ObservableCollection<T>` for lists
- Shell navigation via `Shell.Current.GoToAsync()`

### Step 6: Page — `VoiceNotesAI/Pages/{Entity}ListPage.xaml` + `.cs`

**Code-behind:**
```csharp
using VoiceNotesAI.ViewModels;

namespace VoiceNotesAI.Pages;

public partial class {Entity}ListPage : ContentPage
{
    private readonly {Entity}ListViewModel _viewModel;

    public {Entity}ListPage({Entity}ListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadItemsCommand.ExecuteAsync(null);
    }
}
```

Convention: ViewModel injected via constructor, assigned to `BindingContext`. Load data in `OnAppearing`.

### Step 7: Shell Navigation — Modify `VoiceNotesAI/AppShell.xaml`

Add route registration:

```csharp
Routing.RegisterRoute(nameof({Entity}ListPage), typeof({Entity}ListPage));
Routing.RegisterRoute(nameof({Entity}DetailPage), typeof({Entity}DetailPage));
```

### Step 8: DI Registration — Modify `VoiceNotesAI/MauiProgram.cs`

```csharp
// Services (Singleton)
builder.Services.AddSingleton<I{Entity}Repository, {Entity}Repository>();

// ViewModels (Transient)
builder.Services.AddTransient<{Entity}ListViewModel>();
builder.Services.AddTransient<{Entity}DetailViewModel>();

// Pages (Transient)
builder.Services.AddTransient<{Entity}ListPage>();
builder.Services.AddTransient<{Entity}DetailPage>();
```

Convention: Services as **Singleton**, ViewModels and Pages as **Transient**.

### Step 9: Tests — `VoiceNotesAI.Tests/Services/{Entity}RepositoryTests.cs`

```csharp
using VoiceNotesAI.Data;
using VoiceNotesAI.Models;
using VoiceNotesAI.Services;

namespace VoiceNotesAI.Tests.Services;

public class {Entity}RepositoryTests : IAsyncLifetime
{
    private AppDatabase _database = null!;
    private {Entity}Repository _repository = null!;
    private string _dbPath = null!;

    public async Task InitializeAsync()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db3");
        _database = new AppDatabase(_dbPath);
        await _database.InitializeAsync();
        _repository = new {Entity}Repository(_database);
    }

    public Task DisposeAsync()
    {
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task SaveAsync_NewEntity_ReturnsPositiveId()
    {
        var entity = new {Entity} { Name = "Test" };
        var result = await _repository.SaveAsync(entity);
        Assert.True(result > 0);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        await _repository.SaveAsync(new {Entity} { Name = "Item 1" });
        await _repository.SaveAsync(new {Entity} { Name = "Item 2" });
        var items = await _repository.GetAllAsync();
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntity()
    {
        var entity = new {Entity} { Name = "To Delete" };
        await _repository.SaveAsync(entity);
        await _repository.DeleteAsync(entity.Id);
        var result = await _repository.GetByIdAsync(entity.Id);
        Assert.Null(result);
    }
}
```

Convention: `IAsyncLifetime` for setup/teardown, temp SQLite DB, test naming `Method_Scenario_Expected`.

---

## Checklist

| # | Layer | Action | File |
|---|-------|--------|------|
| 1 | Model | Create | `VoiceNotesAI/Models/{Entity}.cs` |
| 2 | Data | Modify | `VoiceNotesAI/Data/AppDatabase.cs` (add CreateTableAsync) |
| 3 | Service | Create | `VoiceNotesAI/Services/I{Entity}Repository.cs` |
| 4 | Service | Create | `VoiceNotesAI/Services/{Entity}Repository.cs` |
| 5 | ViewModel | Create | `VoiceNotesAI/ViewModels/{Entity}ListViewModel.cs` |
| 6 | ViewModel | Create | `VoiceNotesAI/ViewModels/{Entity}DetailViewModel.cs` (if needed) |
| 7 | Page | Create | `VoiceNotesAI/Pages/{Entity}ListPage.xaml` + `.cs` |
| 8 | Page | Create | `VoiceNotesAI/Pages/{Entity}DetailPage.xaml` + `.cs` (if needed) |
| 9 | Navigation | Modify | `VoiceNotesAI/AppShell.xaml` (register routes) |
| 10 | DI | Modify | `VoiceNotesAI/MauiProgram.cs` (register services, VMs, pages) |
| 11 | Tests | Create | `VoiceNotesAI.Tests/Services/{Entity}RepositoryTests.cs` |

## Response Guidelines

1. **Read existing files first** to match current patterns exactly
2. **Follow the order** — Model → Database → Repository → ViewModel → Page → DI → Tests
3. **Use Note/Category** as primary reference (simplest complete examples)
4. **Match conventions**: file-scoped namespaces, `[ObservableProperty]`, `[RelayCommand]`, async/await
5. **SQLite-net**: `[PrimaryKey, AutoIncrement]`, `DateTime.UtcNow`, no EF Core
6. **UI language**: Portuguese (Brazil) for user-facing strings

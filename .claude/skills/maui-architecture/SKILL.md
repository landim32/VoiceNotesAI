---
name: maui-architecture
description: Guides the implementation of a new entity following the MVVM + layered architecture for .NET MAUI apps. Covers all layers from SQLite Model to Page, including Repository pattern, ViewModel with CommunityToolkit.Mvvm, XAML Page, AppDatabase registration, and DI setup. Use when creating or modifying entities, adding new tables, or scaffolding CRUD features in any MAUI project.
allowed-tools: Read, Grep, Glob, Bash, Write, Edit, Task
user-invocable: true
---

# .NET MAUI Layered Architecture — Entity Implementation Guide

You are an expert assistant that helps developers create or modify entities following the MVVM + layered architecture pattern for .NET MAUI mobile apps. You guide the user through ALL required layers.

## Input

The user will describe the entity to create or modify: `$ARGUMENTS`

## Before You Start

1. **Discover the project structure.** Run `dotnet sln list` and `ls` to identify project names and folder layout.
2. **Identify the naming convention.** The placeholder `{App}` below stands for the actual project root namespace (e.g., `MyApp`, `VoiceNotesAI`). Replace it everywhere.
3. **Read at least one existing entity end-to-end** (Model → Interface → Repository → ViewModel → Page) to match the codebase's exact style, naming, and patterns before generating any code.
4. **Detect the app language.** Check existing UI strings (DisplayAlert, Labels, Titles) to determine the language used in the app and match it in new code.

---

## Recommended Project Structure

```
{App}.sln
├── {App}.Domain/              ← Models + Helpers (net8.0, no deps)
├── {App}.Infra.Interfaces/    ← Service/Repository interfaces (net8.0, refs Domain)
├── {App}.Infra/               ← Implementations + Context (net8.0, refs Domain + Infra.Interfaces)
├── {App}/                     ← MAUI app: Pages, ViewModels, Converters (refs all 3)
└── {App}.Tests/               ← Unit tests (refs Domain + Infra.Interfaces + Infra)
```

**Dependency flow (no cycles):**
```
Domain ← Infra.Interfaces ← Infra
               ↑                ↑
               └── MAUI ────────┘
                    ↑
                  Tests
```

All projects should use `<RootNamespace>{App}</RootNamespace>` to share namespaces and avoid `using` changes when moving files between projects.

> **Note:** If the project uses a single-project structure (all code inside the MAUI project), adapt the file paths accordingly — the patterns and layer separation remain the same, just within a single project.

---

## Architecture & Data Flow

```
Page (XAML) → ViewModel → Repository Interface → Repository → AppDatabase → SQLite
```

**Key packages:**
- `sqlite-net-pcl` + `SQLitePCLRaw.bundle_green` — SQLite ORM
- `CommunityToolkit.Mvvm` — MVVM source generators (`[ObservableProperty]`, `[RelayCommand]`)
- `Plugin.Maui.Audio` — only if audio features are needed

---

## Step-by-Step Implementation

### Step 1: SQLite Model — `{App}.Domain/Models/{Entity}.cs`

```csharp
using SQLite;

namespace {App}.Models;

public class {Entity}
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

**Conventions:**
- `[PrimaryKey, AutoIncrement]` on `int Id`
- Default values on string properties (`= string.Empty`)
- `DateTime.UtcNow` as default for timestamps
- `[Indexed]` for foreign key columns or frequently filtered fields
- No navigation properties — sqlite-net-pcl does not support them

### Step 2: Repository Interface — `{App}.Infra.Interfaces/Services/I{Entity}Repository.cs`

```csharp
using {App}.Models;

namespace {App}.Services;

public interface I{Entity}Repository
{
    Task<List<{Entity}>> GetAllAsync();
    Task<{Entity}?> GetByIdAsync(int id);
    Task<int> SaveAsync({Entity} entity);
    Task<int> DeleteAsync(int id);
}
```

**Conventions:**
- All methods return `Task<>` (async)
- `SaveAsync` handles both insert and update (upsert pattern)
- `GetByIdAsync` returns nullable
- Add domain-specific queries as needed (e.g., `GetByParentIdAsync(int parentId)`)

### Step 3: Repository Implementation — `{App}.Infra/Services/{Entity}Repository.cs`

```csharp
using {App}.Data;
using {App}.Models;

namespace {App}.Services;

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
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<{Entity}?> GetByIdAsync(int id)
    {
        return await _database.Connection
            .Table<{Entity}>()
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveAsync({Entity} entity)
    {
        if (entity.Id != 0)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            return await _database.Connection.UpdateAsync(entity);
        }

        return await _database.Connection.InsertAsync(entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _database.Connection.DeleteAsync<{Entity}>(id);
    }
}
```

**Conventions:**
- Inject `AppDatabase`, use `_database.Connection` for all queries
- Upsert in `SaveAsync`: `Id != 0` → update, else insert
- Set `UpdatedAt` on update if the model has timestamps
- Use fluent LINQ via `Table<T>()` with `OrderBy`, `Where`, etc.
- No try/catch in repository — let exceptions bubble up to ViewModel

### Step 4: Register Table — Modify `{App}.Infra/Context/AppDatabase.cs`

Add to `InitializeAsync()`:

```csharp
await _database.CreateTableAsync<{Entity}>();
```

Optionally add seed data method following any existing seed patterns in the file.

### Step 5: List ViewModel — `{App}/ViewModels/{Entity}ListViewModel.cs`

```csharp
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using {App}.Models;
using {App}.Services;

namespace {App}.ViewModels;

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
        try
        {
            var list = await _{entity}Repository.GetAllAsync();
            Items = new ObservableCollection<{Entity}>(list);
            IsEmpty = Items.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoToDetailAsync({Entity} item)
    {
        var parameters = new Dictionary<string, object> { { "{Entity}", item } };
        await Shell.Current.GoToAsync("{Entity}DetailPage", parameters);
    }

    [RelayCommand]
    private async Task DeleteAsync({Entity} item)
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Delete", $"Delete \"{item.Name}\"?", "Yes", "No");

        if (!confirm) return;

        await _{entity}Repository.DeleteAsync(item.Id);
        Items.Remove(item);
        IsEmpty = Items.Count == 0;
    }
}
```

### Step 6: Detail ViewModel — `{App}/ViewModels/{Entity}DetailViewModel.cs`

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using {App}.Models;
using {App}.Services;

namespace {App}.ViewModels;

public partial class {Entity}DetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly I{Entity}Repository _{entity}Repository;

    public {Entity}DetailViewModel(I{Entity}Repository {entity}Repository)
    {
        _{entity}Repository = {entity}Repository;
    }

    [ObservableProperty]
    private int _{entity}Id;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private bool _isNewItem = true;

    [ObservableProperty]
    private string _pageTitle = "New {Entity}";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("{Entity}", out var obj) && obj is {Entity} item)
        {
            {Entity}Id = item.Id;
            Name = item.Name;
            IsNewItem = false;
            PageTitle = "Edit {Entity}";
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("Error", "Name is required.", "OK");
            return;
        }

        IsSaving = true;
        try
        {
            var entity = new {Entity}
            {
                Id = {Entity}Id,
                Name = Name
            };

            await _{entity}Repository.SaveAsync(entity);
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
```

**ViewModel conventions:**
- `[ObservableProperty]` on private `_camelCase` fields → generates public `PascalCase` property
- `[RelayCommand]` on private `{Method}Async` → generates public `{Method}Command`
- `ObservableCollection<T>` for list bindings
- Navigation via `Shell.Current.GoToAsync()` with `Dictionary<string, object>` parameters
- `IQueryAttributable` to receive navigation parameters
- `DisplayAlert` for confirmations and errors
- Load data in commands, never in constructor

### Step 7: List Page XAML — `{App}/Pages/{Entity}ListPage.xaml`

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:{App}.ViewModels"
             xmlns:models="clr-namespace:{App}.Models;assembly={App}.Domain"
             x:Class="{App}.Pages.{Entity}ListPage"
             x:DataType="vm:{Entity}ListViewModel"
             Title="{Entities}">

    <Grid RowDefinitions="*,Auto" Padding="16">

        <!-- Loading -->
        <ActivityIndicator Grid.Row="0"
                           IsRunning="{Binding IsLoading}"
                           IsVisible="{Binding IsLoading}"
                           HorizontalOptions="Center" VerticalOptions="Center" />

        <!-- Empty State -->
        <VerticalStackLayout Grid.Row="0" IsVisible="{Binding IsEmpty}"
                             VerticalOptions="Center" HorizontalOptions="Center" Spacing="12">
            <Label Text="No items yet" FontSize="18" FontAttributes="Bold"
                   HorizontalOptions="Center" />
        </VerticalStackLayout>

        <!-- List -->
        <CollectionView Grid.Row="0" ItemsSource="{Binding Items}" SelectionMode="None">
            <CollectionView.ItemTemplate>
                <DataTemplate x:DataType="models:{Entity}">
                    <SwipeView>
                        <SwipeView.RightItems>
                            <SwipeItems>
                                <SwipeItem Text="Delete" BackgroundColor="#E53935"
                                    Command="{Binding Source={RelativeSource AncestorType={x:Type vm:{Entity}ListViewModel}}, Path=DeleteCommand}"
                                    CommandParameter="{Binding}" />
                            </SwipeItems>
                        </SwipeView.RightItems>

                        <Frame Margin="0,4" Padding="16" CornerRadius="12" BorderColor="Transparent">
                            <Frame.GestureRecognizers>
                                <TapGestureRecognizer
                                    Command="{Binding Source={RelativeSource AncestorType={x:Type vm:{Entity}ListViewModel}}, Path=GoToDetailCommand}"
                                    CommandParameter="{Binding}" />
                            </Frame.GestureRecognizers>

                            <Label Text="{Binding Name}" FontSize="16" FontAttributes="Bold" />
                        </Frame>
                    </SwipeView>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

        <!-- FAB -->
        <Button Grid.Row="1" Text="+ New" Command="{Binding GoToDetailCommand}"
                FontSize="16" FontAttributes="Bold" HeightRequest="56"
                CornerRadius="28" Margin="0,12,0,0" />
    </Grid>
</ContentPage>
```

**IMPORTANT — XAML assembly references:**
- Models in a separate project require the assembly qualifier:
  `xmlns:models="clr-namespace:{App}.Models;assembly={App}.Domain"`
- ViewModels in the MAUI project do NOT need an assembly qualifier:
  `xmlns:vm="clr-namespace:{App}.ViewModels"`
- If the project is single-project (no Domain layer), neither needs an assembly qualifier.

### Step 8: Page Code-Behind — `{App}/Pages/{Entity}ListPage.xaml.cs`

```csharp
using {App}.ViewModels;

namespace {App}.Pages;

public partial class {Entity}ListPage : ContentPage
{
    private readonly {Entity}ListViewModel _viewModel;

    public {Entity}ListPage({Entity}ListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadItemsCommand.ExecuteAsync(null);
    }
}
```

**Page conventions:**
- Inject ViewModel via constructor (DI)
- Set `BindingContext` in constructor
- Load/refresh data in `OnAppearing` via command execution
- Same pattern for Detail pages (inject Detail ViewModel)

### Step 9: DI Registration — Modify `{App}/MauiProgram.cs`

Add entries in the appropriate sections:

```csharp
// Repository
builder.Services.AddSingleton<I{Entity}Repository, {Entity}Repository>();

// ViewModels
builder.Services.AddTransient<{Entity}ListViewModel>();
builder.Services.AddTransient<{Entity}DetailViewModel>();

// Pages
builder.Services.AddTransient<{Entity}ListPage>();
builder.Services.AddTransient<{Entity}DetailPage>();
```

**Convention:** Repositories and Services as `Singleton` (single DB connection), ViewModels and Pages as `Transient` (fresh state per navigation).

### Step 10: Shell Navigation — Modify `AppShell`

**In `AppShell.xaml`** — add tab or shell content:
```xml
<ShellContent Title="{Entities}"
              ContentTemplate="{DataTemplate pages:{Entity}ListPage}"
              Route="{Entity}ListPage" />
```

**In `AppShell.xaml.cs`** — register routes for pages navigated to programmatically:
```csharp
Routing.RegisterRoute("{Entity}DetailPage", typeof({Entity}DetailPage));
```

### Step 11: Unit Tests — `{App}.Tests/Services/{Entity}RepositoryTests.cs`

```csharp
using {App}.Data;
using {App}.Models;
using {App}.Services;

namespace {App}.Tests.Services;

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
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task SaveAsync_NewEntity_InsertsSuccessfully()
    {
        var entity = new {Entity} { Name = "Test" };
        var result = await _repository.SaveAsync(entity);
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        await _repository.SaveAsync(new {Entity} { Name = "A" });
        await _repository.SaveAsync(new {Entity} { Name = "B" });

        var all = await _repository.GetAllAsync();
        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingEntity_ReturnsEntity()
    {
        var entity = new {Entity} { Name = "Find Me" };
        await _repository.SaveAsync(entity);

        var found = await _repository.GetByIdAsync(entity.Id);
        Assert.NotNull(found);
        Assert.Equal("Find Me", found!.Name);
    }

    [Fact]
    public async Task SaveAsync_ExistingEntity_Updates()
    {
        var entity = new {Entity} { Name = "Original" };
        await _repository.SaveAsync(entity);

        entity.Name = "Updated";
        await _repository.SaveAsync(entity);

        var updated = await _repository.GetByIdAsync(entity.Id);
        Assert.Equal("Updated", updated!.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntity()
    {
        var entity = new {Entity} { Name = "ToDelete" };
        await _repository.SaveAsync(entity);

        await _repository.DeleteAsync(entity.Id);

        var result = await _repository.GetByIdAsync(entity.Id);
        Assert.Null(result);
    }
}
```

**Test conventions:**
- xUnit + `IAsyncLifetime` for async setup/teardown
- Temporary SQLite DB per test class (unique path via `Guid.NewGuid()`)
- Clean up DB file in `DisposeAsync`
- Test each CRUD operation independently
- Use Moq for service/ViewModel tests when needed

---

## Checklist

| # | Layer | Action | File |
|---|-------|--------|------|
| 1 | Domain | Create | `{App}.Domain/Models/{Entity}.cs` |
| 2 | Infra.Interfaces | Create | `{App}.Infra.Interfaces/Services/I{Entity}Repository.cs` |
| 3 | Infra | Create | `{App}.Infra/Services/{Entity}Repository.cs` |
| 4 | Infra | Modify | `{App}.Infra/Context/AppDatabase.cs` (add `CreateTableAsync`) |
| 5 | MAUI | Create | `{App}/ViewModels/{Entity}ListViewModel.cs` |
| 6 | MAUI | Create | `{App}/ViewModels/{Entity}DetailViewModel.cs` |
| 7 | MAUI | Create | `{App}/Pages/{Entity}ListPage.xaml` |
| 8 | MAUI | Create | `{App}/Pages/{Entity}ListPage.xaml.cs` |
| 9 | MAUI | Create | `{App}/Pages/{Entity}DetailPage.xaml` |
| 10 | MAUI | Create | `{App}/Pages/{Entity}DetailPage.xaml.cs` |
| 11 | MAUI | Modify | `{App}/MauiProgram.cs` (DI registrations) |
| 12 | MAUI | Modify | `{App}/AppShell.xaml` + `.xaml.cs` (navigation) |
| 13 | Tests | Create | `{App}.Tests/Services/{Entity}RepositoryTests.cs` |

## Response Guidelines

1. **Discover the project first** — run `dotnet sln list`, read existing entities to match patterns
2. **Follow the order** — Domain → Infra.Interfaces → Infra → MAUI → Tests
3. **Build after each layer** to catch errors early:
   - `dotnet build {App}.Domain/{App}.Domain.csproj`
   - `dotnet build {App}.Infra/{App}.Infra.csproj`
   - `dotnet build {App}/{App}.csproj`
   - `dotnet test {App}.Tests/{App}.Tests.csproj`
4. **XAML assembly references** — use `assembly={App}.Domain` for models in separate projects
5. **Match the app language** — read existing UI strings and use the same language for new ones
6. **Match conventions** — SQLite attributes, `[ObservableProperty]`, `[RelayCommand]`, upsert pattern
7. **Singleton** for repos/services, **Transient** for ViewModels/Pages
8. **Adapt to project structure** — if single-project (no Domain/Infra split), adjust file paths but keep the same layered patterns

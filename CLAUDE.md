# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

DevNotes is a .NET MAUI Android app (net8.0-android) that records voice notes and structures them automatically using OpenAI's Whisper (speech-to-text) and GPT-4 (note interpretation) APIs. The UI and prompts are in Portuguese (Brazil).

## Build & Test Commands

```bash
# Build entire solution
dotnet build DevNotes.sln

# Run all tests
dotnet test DevNotes.Tests/DevNotes.Tests.csproj

# Run a single test by name
dotnet test DevNotes.Tests/DevNotes.Tests.csproj --filter "FullyQualifiedName~TestMethodName"

# Build Android APK (release)
dotnet publish DevNotes/DevNotes.csproj -c Release -f net8.0-android -o output

# First-time setup: install MAUI workload
dotnet workload install maui-android
```

## Architecture

**MVVM pattern** with CommunityToolkit.Mvvm source generators, dependency injection via `MauiProgram.cs`, and interface-based services. The solution is organized into 6 projects following a layered architecture. **AutoMapper** handles Model↔DTO mapping between Domain entities and DTO classes (Info suffix).

### Core Flow
Recording (AudioService) → Transcription (SpeechToTextService → Whisper API) → Interpretation (AIService → GPT-4) → Storage (NoteService → NoteRepository → SQLite)

### Data Flow
ViewModels → Services (DTOs) → Repositories (Domain Models) → SQLite

### Project Structure

```
DevNotes.Domain           (net8.0)  — Models + Helpers (no dependencies)
DevNotes.DTO              (net8.0)  — DTOs with "Info" suffix (no dependencies)
DevNotes.Infra.Interfaces (net8.0)  — Repository + AppService interfaces (refs Domain + DTO)
DevNotes.Application      (net8.0)  — Service interfaces + implementations, DI config (refs Domain + DTO + Infra.Interfaces)
DevNotes.Infra            (net8.0)  — Repository + AppService implementations, Context, AutoMapper Profiles (refs Domain + DTO + Infra.Interfaces)
DevNotes                  (MAUI)    — UI layer: Pages, ViewModels, Converters (refs all 5)
DevNotes.Tests            (net8.0)  — Unit tests (refs Domain + DTO + Application + Infra.Interfaces + Infra)
```

All projects use `<RootNamespace>DevNotes</RootNamespace>` to share namespaces.

### Key Layers

- **DevNotes.Domain** — `Note`, `NoteResult`, `Category`, `Comment`, `AppSetting` (SQLite entities); `OpenAISettings`, `PromptTemplates` (helpers).
- **DevNotes.DTO** — `NoteInfo`, `CategoryInfo`, `CommentInfo` (pure data classes, "Info" suffix convention).
- **DevNotes.Infra.Interfaces** — Repository interfaces (`INoteRepository`, `ICategoryRepository`, `ICommentRepository`, `ISettingsRepository`), AppService interfaces (`IAIService`, `IAudioService`, `ISpeechToTextService`).
- **DevNotes.Application** — Service interfaces (`INoteService`, `ICategoryService`, `ICommentService`, `ISettingService`), Service implementations (`NoteService`, `CategoryService`, `CommentService`, `SettingService`), DI configuration (`DependencyInjection.AddApplicationServices()`).
- **DevNotes.Infra** — `AppDatabase` (SQLite wrapper), Repository implementations (`Repository/`), AppService implementations (`AppServices/`: `AIService`, `SpeechToTextService`), AutoMapper profiles (`Mappers/`).
- **DevNotes (MAUI)** — `AudioService` (only service that stays here, depends on Plugin.Maui.Audio), ViewModels (use DTOs via Service interfaces), Pages (XAML + Shell navigation), Converters.

### DI Registration (MauiProgram.cs)
- **Singleton:** `OpenAISettings`, `AppDatabase`, `HttpClient`, `AudioManager`, all Services, Repositories, and AppServices
- **Transient:** All Pages and ViewModels

## Configuration

OpenAI API settings are in `appsettings.json` (gitignored). See `appsettings.example.json` for the schema. The file is embedded as a build resource and loaded from the assembly manifest at runtime.

## Testing

- **Framework:** xUnit + Moq
- **Pattern:** Services tested by mocking `HttpMessageHandler`; repository tests use temporary SQLite databases with `IAsyncLifetime` for setup/teardown.
- Tests reference Domain, Infra.Interfaces, and Infra projects directly (no linked source files).

## CI/CD

GitHub Actions workflow (`.github/workflows/build-apk.yml`) builds the Android APK on push to main/develop and on PRs. Requires .NET 8.0 SDK and JDK 17.

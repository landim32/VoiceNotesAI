# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

VoiceNotesAI is a .NET MAUI Android app (net8.0-android) that records voice notes and structures them automatically using OpenAI's Whisper (speech-to-text) and GPT-4 (note interpretation) APIs. The UI and prompts are in Portuguese (Brazil).

## Build & Test Commands

```bash
# Build entire solution
dotnet build VoiceNotesAI.sln

# Run all tests
dotnet test VoiceNotesAI.Tests/VoiceNotesAI.Tests.csproj

# Run a single test by name
dotnet test VoiceNotesAI.Tests/VoiceNotesAI.Tests.csproj --filter "FullyQualifiedName~TestMethodName"

# Build Android APK (release)
dotnet publish VoiceNotesAI/VoiceNotesAI.csproj -c Release -f net8.0-android -o output

# First-time setup: install MAUI workload
dotnet workload install maui-android
```

## Architecture

**MVVM pattern** with CommunityToolkit.Mvvm source generators, dependency injection via `MauiProgram.cs`, and interface-based services. The solution is organized into 4 projects following a layered architecture.

### Core Flow
Recording (AudioService) → Transcription (SpeechToTextService → Whisper API) → Interpretation (AIService → GPT-4) → Storage (NoteRepository → SQLite)

### Project Structure

```
VoiceNotesAI.Domain        (net8.0)  — Models + Helpers (no dependencies)
VoiceNotesAI.Infra.Interfaces (net8.0)  — Service interfaces (refs Domain)
VoiceNotesAI.Infra         (net8.0)  — Service implementations + Context (refs Domain + Infra.Interfaces)
VoiceNotesAI               (MAUI)    — UI layer: Pages, ViewModels, Converters (refs all 3)
VoiceNotesAI.Tests          (net8.0)  — Unit tests (refs Domain + Infra.Interfaces + Infra)
```

All projects use `<RootNamespace>VoiceNotesAI</RootNamespace>` to share namespaces.

### Key Layers

- **VoiceNotesAI.Domain** — `Note`, `NoteResult`, `Category`, `Comment`, `AppSetting` (SQLite entities/DTOs); `OpenAISettings`, `PromptTemplates` (helpers).
- **VoiceNotesAI.Infra.Interfaces** — Service interfaces (`IAIService`, `IAudioService`, `ISpeechToTextService`, `INoteRepository`, `ICategoryRepository`, `ICommentRepository`, `ISettingsRepository`).
- **VoiceNotesAI.Infra** — `AppDatabase` (SQLite wrapper), repository implementations, `AIService`, `SpeechToTextService`.
- **VoiceNotesAI (MAUI)** — `AudioService` (only service that stays here, depends on Plugin.Maui.Audio), ViewModels (`[ObservableProperty]`/`[RelayCommand]`), Pages (XAML + Shell navigation), Converters.

### DI Registration (MauiProgram.cs)
- **Singleton:** `OpenAISettings`, `AppDatabase`, `HttpClient`, `AudioManager`
- **Transient:** All Pages, ViewModels, and Services

## Configuration

OpenAI API settings are in `appsettings.json` (gitignored). See `appsettings.example.json` for the schema. The file is embedded as a build resource and loaded from the assembly manifest at runtime.

## Testing

- **Framework:** xUnit + Moq
- **Pattern:** Services tested by mocking `HttpMessageHandler`; repository tests use temporary SQLite databases with `IAsyncLifetime` for setup/teardown.
- Tests reference Domain, Infra.Interfaces, and Infra projects directly (no linked source files).

## CI/CD

GitHub Actions workflow (`.github/workflows/build-apk.yml`) builds the Android APK on push to main/develop and on PRs. Requires .NET 8.0 SDK and JDK 17.

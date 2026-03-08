# VoiceNotes AI — Notas de Voz Inteligentes

![.NET MAUI](https://img.shields.io/badge/.NET_MAUI-8.0-512BD4?logo=dotnet)
![Android](https://img.shields.io/badge/Android-21%2B-3DDC84?logo=android)
![OpenAI](https://img.shields.io/badge/OpenAI-Whisper%20%2B%20GPT--4-412991?logo=openai)
![License](https://img.shields.io/badge/License-MIT-green)
![Build](https://github.com/landim32/DevNotes/actions/workflows/build-apk.yml/badge.svg)

## Overview

**VoiceNotes AI** is a .NET MAUI Android app that records voice notes and automatically structures them using OpenAI's **Whisper** (speech-to-text) and **GPT-4** (note interpretation) APIs. Simply tap the microphone, speak, and the app transcribes your audio, interprets the content, and generates a structured note with title, description, and category — all in Portuguese (Brazil).

Built with **MVVM + layered architecture** across 7 projects using CommunityToolkit.Mvvm, SQLite for local storage, AutoMapper for Model↔DTO mapping, and dependency injection throughout.

---

## 🚀 Features

- 🎙️ **Voice Recording** — One-tap recording with automatic processing after stop
- 🧠 **AI Transcription** — Speech-to-text via OpenAI Whisper API
- 📝 **Smart Structuring** — GPT-4 interprets transcriptions and generates titled, categorized notes
- 💬 **Comments & Consolidation** — Add comments to notes and consolidate them into a single coherent text via GPT-4
- 🗂️ **Category Management** — Full CRUD for custom categories with 6 default seeds
- 🔍 **Category Filtering** — Filter notes by category on the main list
- ⚡ **Auto-Processing** — Records, transcribes, and structures in a single flow (no manual steps)
- ⚙️ **In-App Settings** — Configure OpenAI API key and model preferences at runtime
- 🌙 **Dark Mode** — Full light/dark theme support with brand colors
- 💾 **Offline Storage** — All notes persisted locally in SQLite
- ✏️ **Edit & Delete** — Swipe-to-delete and full note editing with category reassignment
- 📖 **Auto-Generated API Docs** — XML documentation deployed to GitHub Wiki

---

## 🛠️ Technologies Used

### Core Framework
- **.NET MAUI 8** — Cross-platform UI framework (targeting Android)
- **CommunityToolkit.Mvvm 8.3.2** — Source-generated MVVM with `[ObservableProperty]` and `[RelayCommand]`

### AI & Speech
- **OpenAI Whisper API** — Audio transcription (speech-to-text)
- **OpenAI GPT-4o** — Note interpretation, structuring, and consolidation

### Database
- **SQLite** (sqlite-net-pcl 1.9.172) — Local storage for notes, categories, comments, and settings
- **SQLitePCLRaw.bundle_green 2.1.10** — SQLite native bindings

### Audio
- **Plugin.Maui.Audio 3.0** — Cross-platform audio recording

### Mapping
- **AutoMapper 12.0.1** — Model↔DTO mapping between Domain entities and DTO classes

### Configuration
- **Microsoft.Extensions.Configuration 8.0** — Embedded `appsettings.json` with runtime override via SQLite

### Testing
- **xUnit 2.6.6** — Test framework
- **Moq 4.20.72** — Mocking library
- **SQLite in-memory** — Temporary databases for repository tests

### CI/CD
- **GitHub Actions** — 4 workflows: versioning, release, APK build, and API docs
- **GitVersion 5.x** — Semantic versioning from git history

---

## 📐 Architecture

The solution follows a **layered architecture** with clear separation of concerns across 7 projects:

```
┌─────────────────────────────────────────────────┐
│              DevNotes (MAUI)                │
│         Pages, ViewModels, Converters           │
└────────────────────┬────────────────────────────┘
                     │ uses DTOs via Service interfaces
┌────────────────────▼────────────────────────────┐
│           DevNotes.Application              │
│    Service interfaces + implementations + DI    │
└────────────────────┬────────────────────────────┘
          ┌──────────┼──────────┐
          ▼          ▼          ▼
┌─────────────┐ ┌────────┐ ┌──────────────────────┐
│   Domain    │ │  DTO   │ │   Infra.Interfaces   │
│Models+Helpers│ │  Info  │ │  Repo + AppService   │
│  +Services  │ │ classes│ │     interfaces        │
└─────────────┘ └────────┘ └──────────┬───────────┘
                                      │
                           ┌──────────▼───────────┐
                           │   DevNotes.Infra  │
                           │ Repos, AppServices,   │
                           │ Context, AutoMapper   │
                           └───────────────────────┘
```

### Core Flow
```
Recording (AudioAppService) → Transcription (SpeechToTextAppService → Whisper API)
→ Interpretation (AIAppService → GPT-4) → Storage (NoteService → NoteRepository → SQLite)
```

### Data Flow
```
ViewModels → Domain Services (DTOs) → Repositories (Domain Models) → SQLite
```

---

## 📁 Project Structure

```
DevNotes/
├── .github/
│   └── workflows/
│       ├── version-tag.yml          # Semantic versioning + git tag
│       ├── create-release.yml       # GitHub Release on minor/major bumps
│       ├── build-apk.yml           # Build + publish Android APK
│       └── generate-docs.yml       # XML docs → GitHub Wiki
├── DevNotes/                    # MAUI app (net8.0-android)
│   ├── Converters/                  # 6 IValueConverter implementations
│   ├── Pages/                       # 7 XAML pages (UI layer)
│   ├── ViewModels/                  # 7 ViewModels (CommunityToolkit.Mvvm)
│   ├── Platforms/Android/           # MainActivity, MainApplication
│   ├── Resources/                   # Icons, splash, images, styles
│   ├── App.xaml(.cs)                # App entry + runtime settings load
│   ├── AppShell.xaml(.cs)           # Flyout navigation (hamburger menu)
│   ├── MauiProgram.cs              # DI bootstrap
│   ├── appsettings.example.json     # Configuration template
│   └── DevNotes.csproj
├── DevNotes.Domain/             # Domain layer (net8.0)
│   ├── Models/                      # SQLite entities (Note, Category, Comment, AppSetting)
│   ├── Helpers/                     # OpenAISettings, PromptTemplates
│   └── Services/                    # Domain service interfaces + implementations
├── DevNotes.DTO/                # Data Transfer Objects (net8.0)
│   ├── NoteInfo.cs                  # Note DTO
│   ├── CategoryInfo.cs              # Category DTO
│   ├── CommentInfo.cs               # Comment DTO
│   └── NoteResult.cs                # AI response DTO (title, description, category)
├── DevNotes.Infra.Interfaces/   # Contracts (net8.0)
│   ├── Repository/                  # INoteRepository, ICategoryRepository, etc.
│   └── AppServices/                 # IAIAppService, IAudioAppService, ISpeechToTextAppService
├── DevNotes.Infra/              # Infrastructure (net8.0)
│   ├── Context/                     # AppDatabase (SQLite wrapper + seed)
│   ├── Repository/                  # Repository implementations
│   ├── AppServices/                 # AI, Audio, SpeechToText implementations
│   └── Mapping/                     # AutoMapper profiles (Note, Category, Comment)
├── DevNotes.Application/        # Application layer (net8.0)
│   └── Startup.cs                   # AddApplicationServices() — DI registration
├── DevNotes.Tests/              # Unit tests (net8.0)
│   ├── Services/                    # Repository + API service tests
│   ├── Models/                      # Domain entity tests
│   ├── Mappers/                     # AutoMapper profile tests
│   └── Helpers/                     # Prompt template tests
├── GitVersion.yml                   # Semantic versioning config
├── global.json                      # .NET SDK version pinning (8.0.100)
├── DevNotes.sln
└── README.md
```

---

## ⚙️ Environment Configuration

### 1. Copy the configuration template

```bash
cp DevNotes/appsettings.example.json DevNotes/appsettings.json
```

### 2. Edit `appsettings.json`

```json
{
  "OpenAI": {
    "ApiKey": "sk-your-openai-api-key-here",
    "Model": "gpt-4o",
    "WhisperModel": "whisper-1"
  }
}
```

> **Note:** The API key can also be configured at runtime via the in-app **Settings** page (hamburger menu → Configurações). Runtime settings are persisted in SQLite and override `appsettings.json` defaults.

⚠️ **IMPORTANT**:
- Never commit `appsettings.json` with real credentials (it is gitignored)
- Only `appsettings.example.json` is version controlled
- You need a valid [OpenAI API key](https://platform.openai.com/api-keys) with access to Whisper and GPT-4

---

## 🔧 Setup

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- .NET MAUI Android workload
- Android SDK (API 21+)
- Android emulator or physical device

### Installation

#### 1. Clone the repository

```bash
git clone https://github.com/landim32/DevNotes.git
cd DevNotes
```

#### 2. Install the MAUI workload

```bash
dotnet workload install maui-android
```

#### 3. Configure the API key

```bash
cp DevNotes/appsettings.example.json DevNotes/appsettings.json
# Edit appsettings.json with your OpenAI API key
```

#### 4. Build the solution

```bash
dotnet build DevNotes.sln
```

#### 5. Deploy to emulator/device

```bash
dotnet build DevNotes/DevNotes.csproj -f net8.0-android -t:Install
adb shell am start -n br.com.emagine.devnotes/crc648da53556643be544.MainActivity
```

### Build Release APK

```bash
dotnet publish DevNotes/DevNotes.csproj -c Release -f net8.0-android -p:AndroidPackageFormat=apk
```

---

## 🧪 Testing

### Running Tests

**All tests:**
```bash
dotnet test DevNotes.Tests/DevNotes.Tests.csproj
```

**Single test by name:**
```bash
dotnet test DevNotes.Tests/DevNotes.Tests.csproj --filter "FullyQualifiedName~TestMethodName"
```

### Test Structure

```
DevNotes.Tests/
├── Services/
│   ├── AIServiceTests.cs              # GPT-4 API mocked HTTP tests
│   ├── SpeechToTextServiceTests.cs    # Whisper API mocked HTTP tests
│   ├── NoteRepositoryTests.cs         # SQLite CRUD tests
│   ├── CategoryRepositoryTests.cs     # Category CRUD tests
│   ├── CommentRepositoryTests.cs      # Comment CRUD tests
│   └── SettingsRepositoryTests.cs     # Key-value settings tests
├── Models/
│   ├── NoteTests.cs                   # Note entity tests
│   ├── NoteResultTests.cs            # AI response DTO tests
│   ├── CategoryTests.cs              # Category entity tests
│   └── CommentTests.cs               # Comment entity tests
├── Mappers/
│   ├── NoteProfileTests.cs           # Note AutoMapper profile tests
│   ├── CategoryProfileTests.cs       # Category AutoMapper profile tests
│   └── CommentProfileTests.cs        # Comment AutoMapper profile tests
└── Helpers/
    └── PromptTemplatesTests.cs        # Prompt generation tests
```

### Test Patterns

- **Service tests** mock `HttpMessageHandler` to simulate OpenAI API responses
- **Repository tests** use temporary SQLite databases with `IAsyncLifetime` for setup/teardown
- **Mapper tests** verify AutoMapper profile configurations for all entities
- **Model tests** validate domain entity behavior (Create, Validate, Update methods)

---

## 🔄 CI/CD

### GitHub Actions

The project uses 4 chained workflows for automated versioning, releases, and builds:

| Workflow | File | Trigger | Purpose |
|----------|------|---------|---------|
| **Version and Tag** | `version-tag.yml` | Push to `main` | Determines semantic version via GitVersion and creates a git tag |
| **Create Release** | `create-release.yml` | After Version and Tag | Creates a GitHub Release on minor/major version bumps |
| **Build Android APK** | `build-apk.yml` | After Create Release | Builds, publishes, and attaches APK to the release |
| **Generate API Docs** | `generate-docs.yml` | Push to `main` (Domain/Infra paths) | Generates XML docs and deploys to GitHub Wiki |

### APK Signing — Required Secrets

The **Build Android APK** workflow signs the APK using a keystore. You must configure 4 secrets in your GitHub repository (**Settings → Secrets and variables → Actions → New repository secret**):

| Secret | Description | Example |
|--------|-------------|---------|
| `ANDROID_KEYSTORE_BASE64` | Keystore file (`.jks`) encoded in base64 | `MIIEvgIBADANBg...` |
| `ANDROID_KEYSTORE_PASSWORD` | Password used to create the keystore | `your_keystore_password` |
| `ANDROID_KEY_ALIAS` | Alias of the signing key | `devnotes` |
| `ANDROID_KEY_PASSWORD` | Password for the signing key | `your_key_password` |

#### Generating the keystore

```bash
# 1. Generate the keystore (requires JDK keytool)
keytool -genkey -v -keystore devnotes.keystore -alias devnotes \
  -keyalg RSA -keysize 2048 -validity 10000

# 2. Convert to base64
# Linux/macOS:
base64 devnotes.keystore > keystore.b64

# PowerShell (Windows):
[Convert]::ToBase64String([IO.File]::ReadAllBytes("devnotes.keystore")) | Set-Content keystore.b64
```

Copy the contents of `keystore.b64` into the `ANDROID_KEYSTORE_BASE64` secret.

⚠️ **IMPORTANT**: Keep your keystore and passwords safe. Without them, you cannot publish updates to an existing app. The `keystore/` directory is gitignored — never commit keystore files.

**Pipeline flow:**
```
Push to main → Version and Tag → Create Release → Build Android APK
                                                        ↓
                                            Upload APK artifact (90 days)
                                            Attach APK to GitHub Release
```

**Versioning:** Semantic versioning via [GitVersion](https://gitversion.net/) with branch-based policies:

| Branch | Increment | Tag |
|--------|-----------|-----|
| `main` | Patch | (release) |
| `develop` | Minor | alpha |
| `feature/*` | Minor | alpha |
| `release/*` | Patch | beta |
| `hotfix/*` | Patch | (none) |

**Commit prefixes:** `feat:`/`feature:` → minor, `fix:`/`patch:` → patch, `major:`/`breaking:` → major.

---

## 🔍 Troubleshooting

### Common Issues

#### NETSDK1202: net8.0-android is out of support

**Cause:** .NET 10+ SDK treats net8.0 as EOL.

**Solution:** Ensure `global.json` pins the SDK and `<CheckEolTargetFramework>false</CheckEolTargetFramework>` is set in the `.csproj`:
```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "latestFeature"
  }
}
```

#### appsettings.json not found during build

**Cause:** The file is gitignored and must be created manually.

**Solution:**
```bash
cp DevNotes/appsettings.example.json DevNotes/appsettings.json
```

#### Audio recording not working on emulator

**Cause:** Android emulator may not have microphone permissions granted.

**Solution:** Grant microphone permission in the emulator's app settings, or test on a physical device.

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

### Development Setup

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Make your changes
4. Run tests (`dotnet test DevNotes.Tests/DevNotes.Tests.csproj`)
5. Commit your changes (`git commit -m 'feat: add some AmazingFeature'`)
6. Push to the branch (`git push origin feature/AmazingFeature`)
7. Open a Pull Request

### Commit Convention

This project uses [GitVersion](https://gitversion.net/) for automatic semantic versioning:
- `feat:` / `feature:` — minor version bump
- `fix:` / `patch:` — patch version bump
- `major:` / `breaking:` — major version bump

---

## 👨‍💻 Author

Developed by **[Rodrigo Landim Carneiro](https://github.com/landim32)**

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Built with [.NET MAUI](https://dotnet.microsoft.com/apps/maui)
- AI powered by [OpenAI](https://openai.com/) (Whisper + GPT-4)
- MVVM toolkit by [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)
- Object mapping by [AutoMapper](https://automapper.org/)
- Audio recording by [Plugin.Maui.Audio](https://github.com/jfversluis/Plugin.Maui.Audio)
- Local storage by [sqlite-net](https://github.com/praeclarum/sqlite-net)

---

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/landim32/DevNotes/issues)
- **Wiki**: [API Documentation](https://github.com/landim32/DevNotes/wiki)

---

**⭐ If you find this project useful, please consider giving it a star!**

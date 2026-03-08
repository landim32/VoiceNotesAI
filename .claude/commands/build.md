---
allowed-tools: Bash(dotnet *)
---

Build the DevNotes .NET MAUI project for Android:

```bash
dotnet build DevNotes/DevNotes.csproj -f net8.0-android -p:AndroidSdkDirectory="C:/Program Files (x86)/Android/android-sdk"
```

Report whether the build succeeded or failed, and summarize any errors or warnings.

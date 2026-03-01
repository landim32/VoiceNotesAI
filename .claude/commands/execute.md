---
allowed-tools: Bash(dotnet *), Bash(adb *)
---

Deploy and run the FamilyFinance .NET MAUI app on the Android emulator:

1. First, check if an Android emulator is running:
```bash
adb devices
```

2. If no emulator is listed, inform the user they need to start one (e.g., via Android Studio or `emulator -avd <avd_name>`).

3. If an emulator is available, build and deploy the app:
```bash
dotnet build FamilyFinance/FamilyFinance.csproj -f net8.0-android -t:Install -p:AndroidSdkDirectory="C:/Program Files (x86)/Android/android-sdk"
```

4. Then launch the app on the emulator:
```bash
adb shell am start -n com.companyname.familyfinance/crc64e10fe10fa0b1ee85.MainActivity
```

Report whether the deployment and launch succeeded or failed, and summarize any errors.

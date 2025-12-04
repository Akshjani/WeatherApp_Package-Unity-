# CleverTap Unity SDK - Weather App with Toast Package

A Unity-based weather application demonstrating a custom Toast/Snackbar package for Android and iOS platforms. The app fetches real-time weather data based on the user's GPS location and displays notifications using native platform UI elements.

## 📋 Table of Contents

- [Features](#features)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
- [Testing](#testing)
- [Building](#building)
- [API Documentation](#api-documentation)
- [Troubleshooting](#troubleshooting)
- [License](#license)

## ✨ Features

- ✅ **Custom Unity Package**: Reusable Toast/Snackbar package for cross-platform notifications
- ✅ **Location Services**: Automatic GPS location detection
- ✅ **Weather Integration**: Real-time weather data from Open-Meteo API
- ✅ **Native UI**: Platform-specific Toast (Android) and Snackbar (iOS)
- ✅ **Unit Tests**: Comprehensive test coverage using Unity Test Framework
- ✅ **Clean Architecture**: MVC pattern with separation of concerns
- ✅ **Editor Support**: Debug logging when testing in Unity Editor

## 📁 Project Structure

```
CleverTapUnitySDK/
├── Packages/
│   └── com.clevertap.toastpackage/          # Custom Toast Package
│       ├── Runtime/
│       │   ├── ToastManager.cs              # Main toast manager (Singleton)
│       │   ├── AndroidToastBridge.cs        # Android native bridge
│       │   ├── IOSSnackbarBridge.cs         # iOS native bridge
│       │   └── CleverTap.ToastPackage.asmdef
│       ├── Plugins/
│       │   └── iOS/
│       │       └── SnackbarPlugin.mm        # iOS Objective-C++ plugin
│       ├── Tests/
│       │   └── Runtime/
│       │       ├── ToastManagerTests.cs     # Package unit tests
│       │       └── CleverTap.ToastPackage.Tests.asmdef
│       └── package.json                      # Package manifest
├── Assets/
│   ├── Scripts/
│   │   ├── WeatherApp.cs                    # Main application controller
│   │   ├── LocationService.cs               # GPS location service
│   │   └── WeatherAPIClient.cs              # Weather API client
│   ├── Scenes/
│   │   └── Main.unity                       # Main scene
│   ├── Prefabs/
│   │   └── WeatherUI.prefab                 # UI components
│   └── Tests/
│       ├── WeatherAppTests.cs               # Application unit tests
│       └── Assets.Tests.asmdef              # Test assembly definition
└── README.md
```

## 🏗 Architecture

This project follows a **modular, layered architecture** with clear separation of concerns and platform abstraction.

### Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                     Presentation Layer                       │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────────┐   │
│  │ WeatherApp  │  │ WeatherWidget│  │   Unity UI       │   │
│  │ (Controller)│  │   (Prefab)   │  │  (View Layer)    │   │
│  └──────┬──────┘  └──────┬───────┘  └────────┬─────────┘   │
└─────────┼─────────────────┼───────────────────┼─────────────┘
          │                 │                   │
┌─────────┼─────────────────┼───────────────────┼─────────────┐
│         │      Business Logic Layer           │             │
│  ┌──────▼──────┐  ┌──────▼────────┐  ┌───────▼──────────┐  │
│  │ Location    │  │ Weather API   │  │  ToastManager    │  │
│  │  Service    │  │    Client     │  │   (Singleton)    │  │
│  └──────┬──────┘  └──────┬────────┘  └───────┬──────────┘  │
└─────────┼─────────────────┼───────────────────┼─────────────┘
          │                 │                   │
┌─────────┼─────────────────┼───────────────────┼─────────────┐
│         │      Platform Abstraction Layer     │             │
│  ┌──────▼──────┐  ┌──────▼────────┐  ┌───────▼──────────┐  │
│  │  Unity GPS  │  │ UnityWebReq   │  │ Platform Bridge  │  │
│  │   Service   │  │  (HTTP/JSON)  │  │   (Android/iOS)  │  │
│  └─────────────┘  └───────────────┘  └──────────────────┘  │
└─────────────────────────────────────────────────────────────┘
          │                 │                   │
┌─────────┼─────────────────┼───────────────────┼─────────────┐
│         │         Native Platform Layer       │             │
│  ┌──────▼──────┐  ┌──────▼────────┐  ┌───────▼──────────┐  │
│  │ Android GPS │  │   Network     │  │ Android Toast /  │  │
│  │  iOS Core   │  │     Stack     │  │  iOS Snackbar    │  │
│  │  Location   │  │               │  │                  │  │
│  └─────────────┘  └───────────────┘  └──────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### Design Patterns Used

#### 1. **MVC (Model-View-Controller)**
   - **Model**: Data structures and business logic
     - `WeatherResponse`, `WeatherData` - Data models
     - `LocationService` - Location data provider
     - `WeatherAPIClient` - API communication
   
   - **View**: UI presentation
     - Unity UI components (Text, Button, Canvas)
     - `WeatherWidget` prefab for visual representation
   
   - **Controller**: Application logic coordination
     - `WeatherApp` - Main application controller
     - `WeatherWidget` - Widget controller for prefab

**Benefits**: Separation of concerns, easier testing, maintainable code

#### 2. **Singleton Pattern**
   - **Implementation**: `ToastManager`
   - **Purpose**: Single global instance for toast/snackbar management
   - **Lifecycle**: Persists across scenes via `DontDestroyOnLoad`
   
```csharp
public static ToastManager Instance {
    get {
        if (_instance == null) {
            GameObject go = new GameObject("ToastManager");
            _instance = go.AddComponent<ToastManager>();
            DontDestroyOnLoad(go);
        }
        return _instance;
    }
}
```

**Benefits**: Global access point, controlled instantiation, resource management

#### 3. **Bridge Pattern**
   - **Abstraction**: `ToastManager` (platform-independent interface)
   - **Implementations**:
     - `AndroidToastBridge` - Android native Toast via JNI
     - `IOSSnackbarBridge` - iOS native Snackbar via Objective-C++
   
```csharp
// Platform abstraction
#if UNITY_ANDROID
    AndroidToastBridge.ShowToast(message, duration);
#elif UNITY_IOS
    IOSSnackbarBridge.ShowSnackbar(message, duration);
#else
    Debug.Log($"[Toast] {message}");
#endif
```

**Benefits**: Platform independence, easy extensibility, compile-time optimization

#### 4. **Facade Pattern**
   - **Implementation**: `WeatherWidget` class
   - **Purpose**: Simplifies complex subsystems (GPS + API + Toast)
   - **Usage**: Single prefab encapsulates entire weather fetching workflow

**Benefits**: Simplified API, reduced coupling, easier to use

#### 5. **Observer/Callback Pattern**
   - **Implementation**: Delegate-based callbacks
   - **Usage**: Asynchronous operations (location, API calls)
   
```csharp
public delegate void WeatherDataCallback(WeatherResponse data, string error);
public void GetWeatherData(float lat, float lon, WeatherDataCallback callback);
```

**Benefits**: Non-blocking operations, loose coupling, event-driven architecture

#### 6. **Factory Pattern** (Implicit)
   - **Implementation**: Platform-specific bridge creation
   - **Purpose**: Creates appropriate native bridge based on platform
   
**Benefits**: Runtime platform selection, code reusability

### Architectural Principles

#### SOLID Principles Applied

1. **Single Responsibility Principle (SRP)**
   - `LocationService` - Only handles location
   - `WeatherAPIClient` - Only handles API calls
   - `ToastManager` - Only handles notifications

2. **Open/Closed Principle (OCP)**
   - Easy to add new platforms (Windows, WebGL) without modifying existing code
   - New weather data sources can be added by implementing same interface

3. **Dependency Inversion Principle (DIP)**
   - High-level modules (WeatherApp) don't depend on low-level modules (Android Toast)
   - Both depend on abstractions (ToastManager interface)

#### Component Responsibilities

| Component | Layer | Responsibility | Dependencies |
|-----------|-------|----------------|--------------|
| **ToastManager** | Business Logic | Notification abstraction | Platform bridges |
| **WeatherApp** | Presentation | UI coordination | LocationService, WeatherAPIClient, ToastManager |
| **WeatherWidget** | Presentation | Self-contained widget | LocationService, WeatherAPIClient, ToastManager |
| **LocationService** | Business Logic | GPS location acquisition | Unity Input.location |
| **WeatherAPIClient** | Business Logic | HTTP API communication | UnityWebRequest |
| **AndroidToastBridge** | Platform | Android native UI | Android JNI |
| **IOSSnackbarBridge** | Platform | iOS native UI | iOS Objective-C++ |

### Data Flow

```
User Action (Button Click)
    ↓
Controller (WeatherApp/WeatherWidget)
    ↓
LocationService.GetCurrentLocation()
    ↓
GPS Acquisition (Unity → Native Platform)
    ↓
Controller receives (latitude, longitude)
    ↓
WeatherAPIClient.GetWeatherData(lat, lon)
    ↓
HTTP Request to Open-Meteo API
    ↓
JSON Response Parsing
    ↓
Controller receives WeatherResponse
    ↓
Update UI (Text components)
    ↓
ToastManager.ShowToast(temperature)
    ↓
Platform Bridge (Android/iOS)
    ↓
Native UI Display (Toast/Snackbar)
```

### Package Architecture

The Toast Package follows a **plugin architecture** for cross-platform support:

```
com.clevertap.toastpackage/
├── Runtime/                    # Core runtime code
│   ├── ToastManager.cs        # Public API (Facade)
│   ├── AndroidToastBridge.cs  # Android implementation
│   ├── IOSSnackbarBridge.cs   # iOS implementation
│   └── WeatherWidget.cs       # Complete widget
├── Plugins/                    # Native plugins
│   ├── Android/               # Auto-included in Android builds
│   └── iOS/
│       └── SnackbarPlugin.mm  # Objective-C++ implementation
├── Prefabs/                    # Ready-to-use prefabs
│   └── WeatherWidget.prefab   # Drag-and-drop widget
└── Tests/                      # Unit tests
    └── Runtime/
        └── ToastManagerTests.cs
```

### Threading Model

- **Main Thread**: All Unity API calls, UI updates
- **Native UI Thread**: Android Toast, iOS Snackbar (dispatched via `runOnUiThread`)
- **Background Thread**: HTTP requests handled by UnityWebRequest
- **Coroutines**: Used for asynchronous operations without blocking

### Error Handling Strategy

```
Layer 1 (Platform): Catch native exceptions → Log error
    ↓
Layer 2 (Business Logic): Validate data → Provide defaults
    ↓
Layer 3 (Presentation): Show user-friendly messages → Graceful degradation
```

### Scalability Considerations

1. **Extensibility**: New platforms can be added by implementing new bridge classes
2. **Maintainability**: Clear separation allows independent updates to each layer
3. **Testability**: Each component can be tested independently
4. **Reusability**: Package can be used in any Unity project
5. **Performance**: Platform conditionals ensure only necessary code is compiled

This architecture ensures the codebase is **maintainable**, **testable**, **scalable**, and follows **industry best practices**.

## 📋 Prerequisites

- **Unity**: 2020.3 LTS or higher (tested on Unity 2022.3.62f2)
- **Android**: Min SDK 23 (Android 6.0) or higher
- **iOS**: iOS 11.0 or higher
- **IDE**: Visual Studio, VS Code, or JetBrains Rider (optional)

## 🚀 Installation

### 1. Clone Repository

```bash
git clone https://github.com/yourusername/clevertap-unity-sdk.git
cd clevertap-unity-sdk
```

### 2. Open in Unity

1. Launch Unity Hub
2. Click "Open" and select the project folder
3. Wait for Unity to import all assets

### 3. Install Toast Package

The package is already included in the `Packages/` folder. Verify installation:

1. Open **Window > Package Manager**
2. Select **Packages: In Project**
3. Look for "CleverTap Toast Package"

If not visible, reimport the package:
- Close Unity
- Delete `Library` folder
- Reopen project in Unity

### 4. Configure Platform Settings

#### Android Configuration

1. Go to **File > Build Settings > Android**
2. Click **Player Settings**
3. Under **Other Settings**:
   - Set **Minimum API Level**: Android 6.0 (API 23)
   - Set **Target API Level**: Latest

4. Add permissions to `AndroidManifest.xml` (Create if doesn't exist):

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION"/>
    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION"/>
    <uses-permission android:name="android.permission.INTERNET"/>
    
    <application>
        <!-- Your application configuration -->
    </application>
</manifest>
```

Place at: `Assets/Plugins/Android/AndroidManifest.xml`

#### iOS Configuration

1. Go to **File > Build Settings > iOS**
2. Click **Player Settings**
3. After building to Xcode, add to `Info.plist`:

```xml
<key>NSLocationWhenInUseUsageDescription</key>
<string>This app needs your location to show weather data for your area</string>
```

## 💻 How to Use the Library

### Overview
The CleverTap Toast Package provides a simple, unified API for displaying native notifications across Android and iOS platforms. The library is designed to be intuitive and requires minimal setup.

---

### Basic Usage

#### 1. **Installing the Package**

**Method A: Copy to Packages folder (Recommended)**
```bash
# Copy the entire package folder
cp -r com.clevertap.toastpackage /path/to/your/project/Packages/
```

**Method B: Via Unity Package Manager**
1. Open Unity Editor
2. Go to **Window > Package Manager**
3. Click **+** button (top-left)
4. Select **Add package from disk...**
5. Navigate to `com.clevertap.toastpackage/package.json`
6. Click **Open**

**Method C: Git URL (if hosted on GitHub)**
```
In Package Manager:
Add package from git URL:
https://github.com/yourusername/com.clevertap.toastpackage.git
```

**Verify Installation:**
- Open **Window > Package Manager**
- Switch to **Packages: In Project**
- Look for "CleverTap Toast Package" in the list

---

#### 2. **Using the Toast Manager**

##### **Import Namespace**
```csharp
using CleverTap.ToastPackage;
```

##### **Basic Toast Display**
```csharp
using UnityEngine;
using CleverTap.ToastPackage;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        // Show a short toast (2 seconds)
        ToastManager.Instance.ShowShortToast("Hello World!");
    }
}
```

##### **Show Toast with Custom Duration**
```csharp
// Duration: 0 = short (2s), 1 = long (3.5s)
ToastManager.Instance.ShowToast("This is a message", 0);  // Short
ToastManager.Instance.ShowToast("This is a message", 1);  // Long
```

##### **Show Long Toast**
```csharp
// Long toast (3.5 seconds)
ToastManager.Instance.ShowLongToast("This message stays longer");
```

---

#### 3. **Using the Weather Widget Prefab**

The package includes a **ready-to-use Weather Widget prefab** that demonstrates the Toast functionality with real-world usage.

##### **Quick Start with Prefab**
```
1. Locate the prefab:
   Project Window > Packages > CleverTap Toast Package > Prefabs > WeatherWidget

2. Drag and drop into your scene

3. Press Play
   - Widget automatically fetches weather data
   - Shows temperature in a toast notification

4. Done! No additional setup needed.
```

##### **Programmatic Usage of Weather Widget**
```csharp
using UnityEngine;

public class MyController : MonoBehaviour
{
    public WeatherWidget weatherWidget;

    void Start()
    {
        // Manually trigger weather fetch
        weatherWidget.FetchWeatherData();
    }

    public void OnRefreshButtonClick()
    {
        // Refresh weather when user clicks a button
        weatherWidget.FetchWeatherData();
    }
}
```

---

### Advanced Usage

#### 4. **Creating Custom Toast Implementations**

You can extend the library to support additional platforms or custom behaviors:

```csharp
using UnityEngine;
using CleverTap.ToastPackage;

public class CustomToastHandler : MonoBehaviour
{
    void ShowCustomNotification(string message)
    {
        #if UNITY_STANDALONE_WIN
            // Custom Windows notification
            ShowWindowsNotification(message);
        #elif UNITY_WEBGL
            // Custom WebGL notification
            ShowWebNotification(message);
        #else
            // Fallback to standard toast
            ToastManager.Instance.ShowToast(message, 1);
        #endif
    }
}
```

#### 5. **Integrating with Game Events**

```csharp
public class GameManager : MonoBehaviour
{
    void OnPlayerLevelUp(int newLevel)
    {
        ToastManager.Instance.ShowLongToast($"Level Up! You are now level {newLevel}");
    }

    void OnAchievementUnlocked(string achievementName)
    {
        ToastManager.Instance.ShowLongToast($"🏆 Achievement Unlocked: {achievementName}");
    }

    void OnItemCollected(string itemName)
    {
        ToastManager.Instance.ShowShortToast($"Collected: {itemName}");
    }
}
```

#### 6. **Sequential Toast Messages**

```csharp
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    IEnumerator ShowTutorialMessages()
    {
        ToastManager.Instance.ShowLongToast("Welcome to the game!");
        yield return new WaitForSeconds(4f);
        
        ToastManager.Instance.ShowLongToast("Collect coins to earn points");
        yield return new WaitForSeconds(4f);
        
        ToastManager.Instance.ShowLongToast("Avoid obstacles!");
        yield return new WaitForSeconds(4f);
        
        ToastManager.Instance.ShowShortToast("Good luck!");
    }

    void Start()
    {
        StartCoroutine(ShowTutorialMessages());
    }
}
```

#### 7. **Conditional Toast Display**

```csharp
public class NotificationManager : MonoBehaviour
{
    [SerializeField] private bool showDebugToasts = true;

    public void ShowDebugMessage(string message)
    {
        if (showDebugToasts)
        {
            ToastManager.Instance.ShowShortToast($"[DEBUG] {message}");
        }
    }

    public void ShowErrorMessage(string error)
    {
        ToastManager.Instance.ShowLongToast($"❌ Error: {error}");
    }

    public void ShowSuccessMessage(string message)
    {
        ToastManager.Instance.ShowShortToast($"✓ {message}");
    }
}
```

---

### Platform-Specific Behavior

#### **Android**
```csharp
// Shows native Android Toast at bottom of screen
ToastManager.Instance.ShowToast("Android Toast", 0);

// Platform: Android
// Display: Bottom of screen (system default)
// Duration: 0 = Toast.LENGTH_SHORT (2s), 1 = Toast.LENGTH_LONG (3.5s)
// Threading: Runs on UI thread via runOnUiThread
```

#### **iOS**
```csharp
// Shows custom Snackbar at bottom of screen
ToastManager.Instance.ShowToast("iOS Snackbar", 1);

// Platform: iOS
// Display: Bottom of screen with fade animation
// Duration: 2s (short) or 3.5s (long)
// Implementation: Custom UILabel with animation
```

#### **Unity Editor**
```csharp
// Shows formatted message in Console for testing
ToastManager.Instance.ShowToast("Editor Toast", 0);

// Output in Console:
// ╔════════════════════════════════════════╗
// ║  TOAST: Editor Toast
// ╚════════════════════════════════════════╝
```

---

### Complete Example: Weather App Integration

```csharp
using UnityEngine;
using UnityEngine.UI;
using CleverTap.ToastPackage;

public class WeatherAppExample : MonoBehaviour
{
    [SerializeField] private Button fetchWeatherButton;
    [SerializeField] private Text temperatureText;
    
    private float currentTemperature;

    void Start()
    {
        fetchWeatherButton.onClick.AddListener(OnFetchWeather);
        
        // Show welcome message
        ToastManager.Instance.ShowLongToast("Welcome to Weather App!");
    }

    void OnFetchWeather()
    {
        // Show loading toast
        ToastManager.Instance.ShowShortToast("Fetching weather...");
        
        // Simulate API call
        StartCoroutine(FetchWeatherData());
    }

    IEnumerator FetchWeatherData()
    {
        yield return new WaitForSeconds(2f); // Simulate network delay
        
        // Simulate receiving weather data
        currentTemperature = 25.5f;
        temperatureText.text = $"{currentTemperature:F1}°C";
        
        // Show result in toast
        ToastManager.Instance.ShowLongToast(
            $"🌡️ Current Temperature: {currentTemperature:F1}°C"
        );
    }
}
```

---

### API Reference

#### **ToastManager Class**

```csharp
namespace CleverTap.ToastPackage
{
    public class ToastManager : MonoBehaviour
    {
        // Singleton instance
        public static ToastManager Instance { get; }

        // Show toast with specific duration
        public void ShowToast(string message, int duration = 0)
        
        // Show short toast (~2 seconds)
        public void ShowShortToast(string message)
        
        // Show long toast (~3.5 seconds)
        public void ShowLongToast(string message)
    }
}
```

**Parameters:**
- `message` (string): The text to display
- `duration` (int): 0 for short, 1 for long

**Returns:** void

**Thread Safety:** Can be called from any Unity thread

---

### Configuration

#### **Android Permissions**

Add to `Assets/Plugins/Android/AndroidManifest.xml`:
```xml
<uses-permission android:name="android.permission.INTERNET"/>
```

#### **iOS Permissions**

Add to `Info.plist` (if using location in WeatherWidget):
```xml
<key>NSLocationWhenInUseUsageDescription</key>
<string>This app needs your location for weather data</string>
```

#### **Build Settings**

**Android:**
- Minimum API Level: 23 (Android 6.0)
- Target API Level: 33+ (recommended)

**iOS:**
- Minimum iOS Version: 11.0
- Target SDK: Device SDK

---

### Best Practices

1. **✅ DO**: Use short messages for toasts (under 50 characters)
2. **✅ DO**: Space out sequential toasts by 2-3 seconds
3. **✅ DO**: Use toasts for non-critical notifications
4. **✅ DO**: Test on actual devices (Editor simulation is limited)
5. **❌ DON'T**: Use toasts for critical error messages (use dialogs instead)
6. **❌ DON'T**: Show too many toasts rapidly (they queue and frustrate users)
7. **❌ DON'T**: Use long text (Android truncates after ~90 characters)
8. **❌ DON'T**: Rely on toasts for user input (they're informational only)

---

### Troubleshooting

| Issue | Solution |
|-------|----------|
| **Toast doesn't show** | Check platform, verify package installation |
| **Multiple toasts queue** | Add delays between calls (2.5s+) |
| **"Namespace not found" error** | Add `using CleverTap.ToastPackage;` |
| **Works in Editor but not device** | Rebuild APK/IPA with new code |
| **Toast shows once only** | See Android Toast Fix guide, update AndroidToastBridge.cs |

---

### Examples Repository

For more examples, see:
- `Assets/Scripts/WeatherApp.cs` - Full weather application
- `Packages/.../Runtime/WeatherWidget.cs` - Self-contained widget
- `Packages/.../Tests/Runtime/ToastManagerTests.cs` - Unit test examples

---

### Support

For issues or questions:
1. Check [Troubleshooting](#troubleshooting) section
2. Review [API Reference](#api-reference)
3. Check Console logs for errors
4. Contact: [your.email@example.com]

---

**Next Steps:**
- ✅ Install the package
- ✅ Try the basic example
- ✅ Test the WeatherWidget prefab
- ✅ Integrate into your project
- ✅ Build and test on device

## 🧪 Testing

### Testing Framework

**Unity Test Framework (UTF)** - Based on NUnit 3

### Running Tests

#### In Unity Editor:

1. Open **Window > General > Test Runner**
2. Select **PlayMode** tab
3. You'll see two test suites:
   - `CleverTap.ToastPackage.Tests` - Package tests
   - `WeatherAppTests` - Application tests
4. Click **Run All** or select specific tests

#### Via Command Line:

```bash
# Run all PlayMode tests
Unity -runTests -batchmode -projectPath "path/to/project" \
  -testResults "path/to/results.xml" -testPlatform PlayMode

# Run specific test
Unity -runTests -batchmode -projectPath "path/to/project" \
  -testResults "path/to/results.xml" -testPlatform PlayMode \
  -assemblyNames "CleverTap.ToastPackage.Tests"
```

### Test Coverage

#### Toast Package Tests (`ToastManagerTests.cs`):
- ✅ Singleton pattern verification
- ✅ Instance creation
- ✅ Null/empty message handling
- ✅ Different duration handling
- ✅ Method invocation without exceptions

#### Weather App Tests (`WeatherAppTests.cs`):
- ✅ Component initialization
- ✅ API data fetching
- ✅ Response validation
- ✅ Invalid coordinate handling
- ✅ Error handling

### Expected Test Results

All tests should pass. If any fail:
- Check internet connection for API tests
- Verify API endpoint is accessible
- Check Console for detailed error messages

## 🔨 Building

### Android Build

1. **Switch Platform**:
   - File > Build Settings > Android
   - Click "Switch Platform"

2. **Configure Settings**:
   - Player Settings > Resolution and Presentation
   - Default Orientation: Portrait or Auto Rotation
   - Player Settings > Other Settings
   - Set Package Name: `com.clevertap.weatherapp`

3. **Build APK**:
   - Click "Build" or "Build and Run"
   - Choose output location
   - Wait for build completion

4. **Build App Bundle** (for Google Play):
   - Player Settings > Publishing Settings
   - Check "Build App Bundle (Google Play)"
   - Click "Build"

### iOS Build

1. **Switch Platform**:
   - File > Build Settings > iOS
   - Click "Switch Platform"

2. **Configure Settings**:
   - Player Settings > Other Settings
   - Set Bundle Identifier: `com.clevertap.weatherapp`
   - Set Target SDK: Device SDK

3. **Build to Xcode**:
   - Click "Build"
   - Choose output folder
   - Wait for Unity to generate Xcode project

4. **Open in Xcode**:
   - Open generated `.xcodeproj` file
   - Select your development team
   - Connect iOS device
   - Click "Run" to build and install

5. **Add Permissions** (if not added by Unity):
   - In Xcode, open `Info.plist`
   - Add `NSLocationWhenInUseUsageDescription`

## 📚 API Documentation

### Toast Package API

#### ToastManager

```csharp
public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance { get; }
    
    // Show toast with custom duration
    public void ShowToast(string message, int duration = 0)
    
    // Show short toast (~2 seconds)
    public void ShowShortToast(string message)
    
    // Show long toast (~3.5 seconds)
    public void ShowLongToast(string message)
}
```

**Parameters**:
- `message` (string): Text to display
- `duration` (int): 0 = short, 1 = long

**Platform Behavior**:
- **Android**: Uses `android.widget.Toast`
- **iOS**: Custom snackbar with UILabel animation
- **Editor**: Logs to Console with `[Toast/Snackbar]` prefix

### Weather API

#### WeatherAPIClient

```csharp
public class WeatherAPIClient : MonoBehaviour
{
    public delegate void WeatherDataCallback(WeatherResponse data, string error);
    
    public void GetWeatherData(float latitude, float longitude, 
                               WeatherDataCallback callback)
}
```

#### LocationService

```csharp
public class LocationService : MonoBehaviour
{
    public delegate void LocationCallback(float latitude, float longitude, 
                                         string error);
    
    public IEnumerator GetCurrentLocation(LocationCallback callback)
}
```

#### Data Models

```csharp
[Serializable]
public class WeatherResponse
{
    public float latitude;
    public float longitude;
    public string timezone;
    public DailyUnits daily_units;
    public DailyData daily;
}

[Serializable]
public class DailyData
{
    public string[] time;
    public float[] temperature_2m_max;
}
```

### Weather API Endpoint

**URL**: `https://api.open-meteo.com/v1/forecast`

**Parameters**:
- `latitude`: GPS latitude (-90 to 90)
- `longitude`: GPS longitude (-180 to 180)
- `timezone`: Time zone (e.g., "IST", "auto")
- `daily`: Requested daily data (e.g., "temperature_2m_max")

**Example Request**:
```
https://api.open-meteo.com/v1/forecast?latitude=19.07&longitude=72.87&timezone=auto&daily=temperature_2m_max
```

**Example Response**:
```json
{
  "latitude": 19.125,
  "longitude": 72.875,
  "timezone": "Asia/Calcutta",
  "timezone_abbreviation": "IST",
  "daily": {
    "time": ["2024-12-02", "2024-12-03"],
    "temperature_2m_max": [32.5, 33.1]
  }
}
```

## 🔧 Troubleshooting

### Common Issues

#### 1. Package Not Found

**Error**: `The type or namespace name 'CleverTap' could not be found`

**Solution**:
- Verify `Packages/com.clevertap.toastpackage/package.json` exists
- Close and reopen Unity
- Check Package Manager for the package
- Delete `Library` folder and reimport

#### 2. Toast Not Showing on Android

**Problem**: Toast doesn't appear on Android device

**Solution**:
- Verify you're testing on actual device, not Editor
- Check LogCat: `adb logcat | grep Unity`
- Ensure Min SDK is 23+
- Check for exceptions in Console

#### 3. iOS Plugin Not Working

**Problem**: Snackbar doesn't show on iOS

**Solution**:
- Ensure `.mm` file is in `Plugins/iOS/`
- Verify file is included in Xcode build
- Check Xcode console for errors
- Test on device, not simulator (recommended)

#### 4. Location Permission Denied

**Problem**: App can't access location

**Solution**:
- **Android**: Check AndroidManifest.xml permissions
- **iOS**: Verify Info.plist has location description
- Grant permissions in device settings
- Request permissions at runtime

#### 5. Network Request Fails

**Problem**: Weather API returns errors

**Solution**:
- Check internet connectivity
- Verify API endpoint: https://api.open-meteo.com
- Test API directly in browser
- Check firewall settings
- Ensure INTERNET permission on Android

#### 6. Tests Failing

**Problem**: Unit tests don't pass

**Solution**:
- Verify Test Framework is installed
- Check `.asmdef` files are correct
- Ensure `using NUnit.Framework;` is present
- Run tests individually to isolate issues
- Check Console for compilation errors

#### 7. Assembly Definition Errors

**Error**: CS0246 or namespace errors

**Solution**:
```bash
# Steps:
1. Delete Library/ScriptAssemblies
2. Restart Unity
3. Verify .asmdef files are valid JSON
4. Check references in .asmdef files
```

### Debug Logging

Enable verbose logging for debugging:

```csharp
// In WeatherApp.cs
private void OnWeatherDataReceived(WeatherResponse data, string error)
{
    Debug.Log($"[WeatherApp] Response received");
    Debug.Log($"[WeatherApp] Error: {error}");
    Debug.Log($"[WeatherApp] Data: {JsonUtility.ToJson(data)}");
    
    // ... rest of code
}
```

### Platform-Specific Debugging

**Android**:
```bash
# View Unity logs
adb logcat -s Unity

# View all logs
adb logcat

# Clear logs
adb logcat -c
```

**iOS**:
- Open Xcode
- Window > Devices and Simulators
- Select device > View Device Logs
- Filter by "Unity"

## 📝 Development Notes

### Key Design Decisions

1. **Package Location**: Placed in `Packages/` for reusability across projects
2. **Singleton Pattern**: Ensures single ToastManager instance
3. **Platform Conditionals**: `#if UNITY_ANDROID` prevents unnecessary code in builds
4. **Async Callbacks**: Non-blocking API calls for better UX
5. **Editor Simulation**: Debug logs allow testing without device builds

### Performance Considerations

- Location service stops after data retrieval (battery saving)
- Single API call per weather fetch (rate limit friendly)
- Toast messages auto-dismiss (no memory leaks)
- Coroutines used for timeout handling

### Security Notes

- No API keys required (Open-Meteo is free)
- Location data not stored or transmitted except to weather API
- No user authentication required

## 📄 License

MIT License

Copyright (c) 2024 CleverTap Unity SDK Team

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## 👨‍💻 Author

Created as part of CleverTap Unity SDK Team Technical Assessment

**Developer**: [Your Name]  
**Email**: [your.email@example.com]  
**GitHub**: [github.com/yourusername]

## 🔗 Links

- [Weather API Documentation](https://open-meteo.com/en/docs)
- [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@latest)
- [Unity Package Manager](https://docs.unity3d.com/Manual/Packages.html)

## 📮 Support

For questions or issues:
1. Check [Troubleshooting](#troubleshooting) section
2. Review Console logs for errors
3. Create an issue on GitHub
4. Contact: [your.email@example.com]

---

**Last Updated**: December 2, 2024  
**Version**: 1.0.0  
**Unity Version**: 2022.3.62f2

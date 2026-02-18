﻿﻿# UDTool Desktop

A modern Avalonia-based desktop application for managing files on the UDTool cloud service. This application provides a user-friendly GUI for uploading, downloading, searching, and managing files with API key authentication.

![Version](https://img.shields.io/badge/version-1.0-blue)
![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![Avalonia](https://img.shields.io/badge/Avalonia-11.3.12-red)

## Features

- 🔑 **API Key Management** - Create new keys or validate existing ones
- 📤 **File Upload** - Upload files with custom names to the cloud
- 📥 **File Download** - Download files to your local system
- 🔍 **File Search** - Search for files by name pattern
- 🗑️ **File Delete** - Remove files from the server
- 📋 **File List** - View all uploaded files with refresh capability
- 💾 **Persistent Storage** - API key automatically saved between sessions
- 🎨 **Modern UI** - Professional dark theme with card-based layout
- ⚡ **Async Operations** - Non-blocking UI for all network operations

## Installation

### Download Pre-built Installers

Download the latest installer for your platform from the [Releases](../../releases) page:

#### Windows
- Download `UDTool-Desktop-Setup.msi`
- Double-click to run the installer
- Follow the installation wizard

#### macOS
- Download `UDTool-Desktop.pkg`
- Double-click to run the installer
- Follow the installation wizard

#### Linux (Debian/Ubuntu)
```bash
# Download the .deb file, then:
sudo dpkg -i udtool-desktop_1.0.0_amd64.deb
```

#### Linux (Fedora/RHEL/CentOS)
```bash
# Download the .rpm file, then:
sudo rpm -i udtool-desktop-1.0.0-1.x86_64.rpm
```

### Build from Source

#### Prerequisites

- .NET 10.0 SDK or later
- Windows, macOS, or Linux

#### Clone the Repository

```bash
git clone <repository-url>
cd UDTool-Desktop
```

#### Build the Project

```bash
cd UDTool-Desktop
dotnet build
```

#### Run the Application

```bash
dotnet run
```

## Usage

### First-Time Setup

1. **Launch the application**
2. **Create or enter an API key:**
   - **Option A**: Click "Create New Key" to generate a new API key
   - **Option B**: Enter an existing API key and click "Check Key"
3. The key is automatically validated and saved for future sessions

### Upload a File

1. Enter the full path to the file you want to upload
   ```
   Example: C:\Users\YourName\Documents\myfile.txt
   ```
2. Enter the target name (how the file should be named on the server)
   ```
   Example: myfile.txt
   ```
3. Click "Upload File"
4. The status message will show success and provide the file URL

### Download a File

1. Enter the file name to download
2. Click "Download File"
3. The file will be saved to the current directory
4. Status message confirms successful download

### Search for Files

1. Enter a search query (partial file name works)
   ```
   Example: report
   ```
2. Click "Search"
3. Matching files will appear in the files list below

### Delete a File

1. Enter the file name to delete
2. Click "Delete File"
3. Confirmation message appears on success
4. The file list automatically refreshes

### View All Files

1. Click the "↻ Refresh" button in the Files List section
2. All your files will be displayed in the list box
3. Click on any file to select it for download or deletion

## Project Structure

```
UDTool-Desktop/
├── Assets/                      # Application resources
│   ├── logo.ico                # Application icon
│   ├── logo.png                # Logo image
│   └── Fonts/                  # Custom fonts
├── Converters/                  # Value converters for XAML
│   └── BoolToColorConverter.cs # Boolean to color conversion
├── Elements/                    # Reusable UI components
│   ├── Topbar.axaml           # Application header
│   └── Topbar.axaml.cs
├── Models/                      # Data models and business logic
│   └── UDToolClient.cs        # API client implementation
├── ViewModels/                  # MVVM ViewModels
│   ├── MainWindowViewModel.cs # Main window logic
│   └── ViewModelBase.cs       # Base ViewModel class
├── Views/                       # XAML views
│   ├── MainWindow.axaml       # Main application window
│   └── MainWindow.axaml.cs
├── App.axaml                    # Application resources and styles
├── App.axaml.cs                # Application entry point
├── AppDefaultStyles.axaml      # Global styling definitions
├── Program.cs                   # Program entry point
└── ViewLocator.cs              # View resolution logic
```

## Architecture

### UDToolClient Class

The `UDToolClient` class provides all API interactions:

```csharp
public class UDToolClient
{
    // Constructor with optional API key
    public UDToolClient(string? apiKey = null)
    
    // API Key Management
    public async Task<KeyCheckResult> CheckKeyAsync(string key)
    public async Task<NewKeyResult> CreateNewKeyAsync()
    public void SetApiKey(string apiKey)
    
    // File Operations
    public async Task<OperationResult> UploadAsync(string filePath, string targetName)
    public async Task<OperationResult> DownloadAsync(string fileName, string? savePath = null)
    public async Task<SearchResult> SearchAsync(string query)
    public async Task<OperationResult> DeleteAsync(string fileName)
    public async Task<SearchResult> ListAsync()
}
```

### API Endpoints

The application communicates with the following endpoints:

- `POST /key/new` - Create a new API key
- `GET /key/check/{key}` - Validate an API key
- `POST /{fileName}` - Upload a file (requires API-Key header)
- `GET /{fileName}` - Download a file (requires API-Key header)
- `GET /search/{query}` - Search for files (requires API-Key header)
- `DELETE /{fileName}` - Delete a file (requires API-Key header)
- `GET /list` - List all files (requires API-Key header)

All authenticated requests include the `API-Key` header.

### MVVM Pattern

The application follows the Model-View-ViewModel (MVVM) pattern:

- **Models**: `UDToolClient` and result classes
- **ViewModels**: `MainWindowViewModel` with observable properties and relay commands
- **Views**: XAML files defining the UI structure

Uses `CommunityToolkit.Mvvm` for:
- `[ObservableProperty]` - Auto-generates property changed notifications
- `[RelayCommand]` - Auto-generates ICommand implementations

## Configuration

### API Key Storage

The API key is automatically saved to:
```
Windows: %AppData%\UDTool\apikey.txt
macOS: ~/.config/UDTool/apikey.txt
Linux: ~/.config/UDTool/apikey.txt
```

### API Base URL

The default API base URL is:
```
https://UDTool.delphigamerz.xyz
```

To change this, modify the `BaseUrl` constant in `Models/UDToolClient.cs`:

```csharp
private const string BaseUrl = "https://your-api-url.com";
```

## Dependencies

```xml
<PackageReference Include="Avalonia" Version="11.3.12"/>
<PackageReference Include="Avalonia.Desktop" Version="11.3.12"/>
<PackageReference Include="Avalonia.Themes.Fluent" Version="11.3.12"/>
<PackageReference Include="Avalonia.Fonts.Inter" Version="11.3.12"/>
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.1"/>
```

## Building for Production

### Building Installers

The project includes installers for all major platforms: Windows MSI, macOS PKG, Linux DEB, and Linux RPM.

#### Quick Start - Build Windows MSI Locally

```powershell
# Run the build script from project root
.\build-msi.ps1

# Output: installer-output\UDTool-Desktop-Setup.msi
```

**Requirements:**
- .NET 10.0 SDK
- WiX Toolset v5/v6 (installed automatically by script)

**Test the installer:**
```powershell
# Install
msiexec /i installer-output\UDTool-Desktop-Setup.msi

# Uninstall
msiexec /x installer-output\UDTool-Desktop-Setup.msi

# Install with logging (for debugging)
msiexec /i installer-output\UDTool-Desktop-Setup.msi /l*v install.log
```

#### Manual Windows MSI Build

```powershell
# 1. Publish the application
dotnet publish UDTool-Desktop/UDTool-Desktop.csproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=false `
    -o publish/win-x64

# 2. Install WiX (if not installed)
dotnet tool install --global wix --version 5.0.1

# 3. Add WiX UI extension
wix extension add WixToolset.UI.wixext

# 4. Build the MSI
wix build UDTool-Desktop/Installer.wxs `
    -d PublishDir=publish/win-x64 `
    -o UDTool-Desktop-Setup.msi `
    -arch x64 `
    -ext WixToolset.UI.wixext
```

### Automated Release with GitHub Actions

The project includes a complete CI/CD workflow that builds installers for all platforms automatically.

#### Creating a Release

**Option 1: Tag-based Release (Recommended)**
```bash
# Create and push a version tag
git tag v1.0.0
git push origin v1.0.0

# GitHub Actions automatically:
# 1. Builds installers for all platforms
# 2. Creates a GitHub release
# 3. Attaches all installer files
```

**Option 2: Manual Trigger**
1. Go to GitHub → "Actions" tab
2. Select "Build and Release Installers" workflow
3. Click "Run workflow" button
4. Select branch and run

#### What Gets Built

Each release includes:
- **Windows**: `UDTool-Desktop-Setup.msi`
  - Professional MSI installer
  - Start Menu shortcuts (app + uninstaller)
  - Program Files installation
  - All dependencies bundled
  
- **macOS**: `UDTool-Desktop.pkg`
  - Native PKG installer
  - Installs to /Applications
  - Proper app bundle structure
  
- **Linux Debian/Ubuntu**: `udtool-desktop_1.0.0_amd64.deb`
  - DEB package for apt
  - Desktop entry included
  - Installs to /usr/local/bin
  
- **Linux Fedora/RHEL/CentOS**: `udtool-desktop-1.0.0-1.x86_64.rpm`
  - RPM package for yum/dnf
  - Desktop entry included
  - Installs to /usr/local/bin

#### Workflow Triggers

The GitHub Actions workflow runs on:
- **Push with tag**: Any tag starting with 'v' (e.g., `v1.0.0`, `v2.1.3`)
- **Manual dispatch**: Via GitHub Actions UI

### Building Single-File Executables

If you prefer a single executable without an installer:

#### Windows
```bash
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

#### macOS
```bash
dotnet publish -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true
```

#### Linux
```bash
dotnet publish -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true
```

The published executable will be in `bin/Release/net10.0/{runtime}/publish/`

### Version Management

To release a new version:

1. **Update MSI version** in `UDTool-Desktop/Installer.wxs`:
```xml
<Package Name="UDTool Desktop"
         Version="1.1.0.0"  <!-- Update this -->
         ...>
```

2. **Update package versions** in `.github/workflows/release.yml` (DEB/RPM version strings)

3. **Commit and tag**:
```bash
git add .
git commit -m "Bump version to 1.1.0"
git tag v1.1.0
git push origin v1.1.0
```

### Installer Customization

#### Windows MSI
Edit `UDTool-Desktop/Installer.wxs` to customize:
- Installation directory
- Start Menu shortcuts
- Registry keys
- Files included
- Product metadata

#### macOS/Linux
Edit `.github/workflows/release.yml` to modify:
- Installation paths
- Desktop entry details
- Package metadata
- Build configurations

## Theme Customization

The application uses a modern red/dark theme defined in `App.axaml`:

### Color Scheme

```xml
<SolidColorBrush x:Key="PrimaryRed">#E63946</SolidColorBrush>
<SolidColorBrush x:Key="DarkRed">#A8202A</SolidColorBrush>
<SolidColorBrush x:Key="BackgroundBrush">#0F172A</SolidColorBrush>
<SolidColorBrush x:Key="CardBackgroundBrush">#1E293B</SolidColorBrush>
<SolidColorBrush x:Key="PrimaryForeground">#F8FAFC</SolidColorBrush>
```

To customize colors, edit the resource definitions in `App.axaml`.

## Troubleshooting

### Application Won't Start

1. Ensure .NET 10.0 SDK is installed:
   ```bash
   dotnet --version
   ```
2. Clean and rebuild:
   ```bash
   dotnet clean
   dotnet build
   ```

### API Key Not Saving

Check that the application has write permissions to the AppData directory.

### Upload/Download Fails

1. Verify your API key is valid using "Check Key"
2. Ensure the file paths are correct
3. Check your internet connection
4. Verify the API server is accessible

### Build Errors

If you encounter build errors about locked files:
1. Close all running instances of the application
2. Clean the build:
   ```bash
   dotnet clean
   ```
3. Delete `bin` and `obj` directories manually
4. Rebuild the project

## Development

### Adding New Features

1. **Add API method** in `Models/UDToolClient.cs`
2. **Add ViewModel properties/commands** in `ViewModels/MainWindowViewModel.cs`
3. **Add UI elements** in `Views/MainWindow.axaml`
4. **Update bindings** to connect UI to ViewModel

### Code Style

- Follow C# naming conventions
- Use async/await for all network operations
- Use MVVM pattern for all UI logic
- Add XML documentation comments to public methods

## License

[Add your license information here]

## Contributing

[Add contribution guidelines here]

## Support

For issues, questions, or contributions, please [add contact/repository information here].

## Acknowledgments

- Built with [Avalonia UI](https://avaloniaui.net/)
- Uses [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)
- Integrates with UDTool API service

---

**Version**: 1.0  
**Last Updated**: February 18, 2026  
**Author**: Ari Cummings


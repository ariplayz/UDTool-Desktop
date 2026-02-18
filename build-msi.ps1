# Build MSI Installer for UDTool Desktop
# This script builds the Windows MSI installer using WiX v6

param(
    [string]$Configuration = "Release",
    [string]$OutputDir = "installer-output"
)

Write-Host "Building UDTool Desktop MSI Installer..." -ForegroundColor Cyan

# Step 1: Clean previous builds
Write-Host "`n[1/5] Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path $OutputDir) {
    Remove-Item -Path $OutputDir -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputDir | Out-Null

# Step 2: Publish the application
Write-Host "`n[2/5] Publishing application for Windows x64..." -ForegroundColor Yellow
$publishDir = Join-Path $OutputDir "publish"
dotnet publish UDTool-Desktop/UDTool-Desktop.csproj `
    -c $Configuration `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=false `
    -p:PublishTrimmed=false `
    -o $publishDir

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to publish application!" -ForegroundColor Red
    exit 1
}

# Step 3: Check if WiX is installed
Write-Host "`n[3/5] Checking WiX installation..." -ForegroundColor Yellow
$wixInstalled = Get-Command wix -ErrorAction SilentlyContinue
if (-not $wixInstalled) {
    Write-Host "WiX not found. Installing WiX Toolset..." -ForegroundColor Yellow
    dotnet tool install --global wix --version 5.0.1
    
    # Refresh environment variables
    $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
}

# Step 4: Add WiX UI extension
Write-Host "`n[4/5] Adding WiX UI extension..." -ForegroundColor Yellow
wix extension add WixToolset.UI.wixext -ErrorAction SilentlyContinue

# Step 5: Build the MSI
Write-Host "`n[5/5] Building MSI installer..." -ForegroundColor Yellow
$msiOutput = Join-Path $OutputDir "UDTool-Desktop-Setup.msi"
wix build UDTool-Desktop/Installer.wxs `
    -d PublishDir=$publishDir `
    -o $msiOutput `
    -arch x64 `
    -ext WixToolset.UI.wixext

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to build MSI!" -ForegroundColor Red
    exit 1
}

Write-Host "`n✓ Build completed successfully!" -ForegroundColor Green
Write-Host "MSI installer location: $msiOutput" -ForegroundColor Cyan
Write-Host "`nTo test the installer, run:" -ForegroundColor Yellow
Write-Host "  msiexec /i `"$msiOutput`"" -ForegroundColor White


using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace UDTool_Desktop.Services;

public static class FilePickerService
{
    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
    }

    public static async Task<string?> OpenFileAsync()
    {
        var window = GetMainWindow();
        if (window == null)
            return null;

        var storageProvider = window.StorageProvider;

        var options = new FilePickerOpenOptions
        {
            Title = "Select File to Upload",
            AllowMultiple = false
        };

        var result = await storageProvider.OpenFilePickerAsync(options);
        
        if (result.Count > 0)
        {
            return result[0].Path.LocalPath;
        }

        return null;
    }

    public static async Task<string?> SaveFileAsync(string defaultFileName)
    {
        var window = GetMainWindow();
        if (window == null)
            return null;

        var storageProvider = window.StorageProvider;

        var options = new FilePickerSaveOptions
        {
            Title = "Save File",
            SuggestedFileName = defaultFileName,
            ShowOverwritePrompt = true
        };

        var result = await storageProvider.SaveFilePickerAsync(options);
        
        return result?.Path.LocalPath;
    }
}



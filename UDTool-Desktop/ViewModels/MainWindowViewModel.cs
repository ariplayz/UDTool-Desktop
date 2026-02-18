using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UDTool_Desktop.Models;

namespace UDTool_Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private UDToolClient? _client;
    
    [ObservableProperty]
    private string _apiKey = string.Empty;
    
    [ObservableProperty]
    private bool _isKeyValid;
    
    [ObservableProperty]
    private string _statusMessage = "Please enter your API key or create a new one.";
    
    [ObservableProperty]
    private string _uploadFilePath = string.Empty;
    
    [ObservableProperty]
    private string _uploadTargetName = string.Empty;
    
    [ObservableProperty]
    private string _downloadFileName = string.Empty;
    
    [ObservableProperty]
    private string _searchQuery = string.Empty;
    
    [ObservableProperty]
    private string _deleteFileName = string.Empty;
    
    [ObservableProperty]
    private ObservableCollection<string> _filesList = new();
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string? _selectedFile;

    private Views.SettingsWindow? _settingsWindow;

    public MainWindowViewModel()
    {
        // Try to load saved API key
        LoadApiKey();
    }

    [RelayCommand]
    private void OpenSettings()
    {
        if (_settingsWindow == null || !_settingsWindow.IsVisible)
        {
            _settingsWindow = new Views.SettingsWindow
            {
                DataContext = this
            };
            _settingsWindow.Show();
        }
        else
        {
            _settingsWindow.Activate();
        }
    }

    [RelayCommand]
    private void CloseSettings()
    {
        _settingsWindow?.Close();
    }

    [RelayCommand]
    private void Exit()
    {
        Environment.Exit(0);
    }

    [RelayCommand]
    private void BrowseUploadFile()
    {
        // File picker will be implemented
        StatusMessage = "File picker not yet implemented. Enter path manually.";
    }

    [RelayCommand]
    private async Task DownloadSelected()
    {
        // Use the generated property SelectedFile (from _selectedFile field)
        var fileToDownload = SelectedFile;
        if (!string.IsNullOrEmpty(fileToDownload))
        {
            DownloadFileName = fileToDownload;
            await DownloadFileAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteSelected()
    {
        // Use the generated property SelectedFile (from _selectedFile field)
        var fileToDelete = SelectedFile;
        if (!string.IsNullOrEmpty(fileToDelete))
        {
            DeleteFileName = fileToDelete;
            await DeleteFileAsync();
        }
    }

    [RelayCommand]
    private async Task CheckKeyAsync()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            StatusMessage = "Please enter an API key.";
            return;
        }

        IsLoading = true;
        StatusMessage = "Checking API key...";

        try
        {
            var tempClient = new UDToolClient(ApiKey);
            var result = await tempClient.CheckKeyAsync(ApiKey);
            
            if (result.IsValid)
            {
                IsKeyValid = true;
                _client = new UDToolClient(ApiKey);
                StatusMessage = "API key is valid! You can now use all features.";
                SaveApiKey();
                await RefreshFileListAsync();
            }
            else
            {
                IsKeyValid = false;
                StatusMessage = $"API key is invalid: {result.Message}";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error checking key: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CreateNewKeyAsync()
    {
        IsLoading = true;
        StatusMessage = "Creating new API key...";

        try
        {
            var tempClient = new UDToolClient();
            var result = await tempClient.CreateNewKeyAsync();
            
            if (result.IsSuccess && result.Key != null)
            {
                ApiKey = result.Key;
                IsKeyValid = true;
                _client = new UDToolClient(ApiKey);
                StatusMessage = $"New API key created successfully!\nKey: {result.Key}";
                SaveApiKey();
                await RefreshFileListAsync();
            }
            else
            {
                StatusMessage = $"Failed to create new key: {result.Message}";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error creating key: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task UploadFileAsync()
    {
        if (_client == null || !IsKeyValid)
        {
            StatusMessage = "Please set a valid API key first.";
            return;
        }

        if (string.IsNullOrWhiteSpace(UploadFilePath))
        {
            StatusMessage = "Please select a file to upload.";
            return;
        }

        if (string.IsNullOrWhiteSpace(UploadTargetName))
        {
            StatusMessage = "Please enter a target file name.";
            return;
        }

        IsLoading = true;
        StatusMessage = $"Uploading {UploadTargetName}...";

        try
        {
            var result = await _client.UploadAsync(UploadFilePath, UploadTargetName);
            StatusMessage = result.Message;
            
            if (result.IsSuccess)
            {
                UploadFilePath = string.Empty;
                UploadTargetName = string.Empty;
                await RefreshFileListAsync();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Upload error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DownloadFileAsync()
    {
        if (_client == null || !IsKeyValid)
        {
            StatusMessage = "Please set a valid API key first.";
            return;
        }

        if (string.IsNullOrWhiteSpace(DownloadFileName))
        {
            StatusMessage = "Please enter a file name to download.";
            return;
        }

        IsLoading = true;
        StatusMessage = $"Downloading {DownloadFileName}...";

        try
        {
            var result = await _client.DownloadAsync(DownloadFileName);
            StatusMessage = result.Message;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Download error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchFilesAsync()
    {
        if (_client == null || !IsKeyValid)
        {
            StatusMessage = "Please set a valid API key first.";
            return;
        }

        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            StatusMessage = "Please enter a search query.";
            return;
        }

        IsLoading = true;
        StatusMessage = $"Searching for '{SearchQuery}'...";

        try
        {
            var result = await _client.SearchAsync(SearchQuery);
            
            if (result.IsSuccess)
            {
                FilesList.Clear();
                foreach (var file in result.Files)
                {
                    FilesList.Add(file);
                }
                
                StatusMessage = result.Files.Count > 0 
                    ? $"Found {result.Files.Count} file(s) matching '{SearchQuery}'." 
                    : $"No files found matching '{SearchQuery}'.";
            }
            else
            {
                StatusMessage = result.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Search error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteFileAsync()
    {
        if (_client == null || !IsKeyValid)
        {
            StatusMessage = "Please set a valid API key first.";
            return;
        }

        if (string.IsNullOrWhiteSpace(DeleteFileName))
        {
            StatusMessage = "Please enter a file name to delete.";
            return;
        }

        IsLoading = true;
        StatusMessage = $"Deleting {DeleteFileName}...";

        try
        {
            var result = await _client.DeleteAsync(DeleteFileName);
            StatusMessage = result.Message;
            
            if (result.IsSuccess)
            {
                DeleteFileName = string.Empty;
                await RefreshFileListAsync();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Delete error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshFileListAsync()
    {
        if (_client == null || !IsKeyValid)
        {
            return;
        }

        IsLoading = true;

        try
        {
            var result = await _client.ListAsync();
            
            if (result.IsSuccess)
            {
                FilesList.Clear();
                foreach (var file in result.Files)
                {
                    FilesList.Add(file);
                }
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error refreshing file list: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void SelectFileFromList(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            DownloadFileName = fileName;
            DeleteFileName = fileName;
        }
    }

    private void SaveApiKey()
    {
        try
        {
            var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UDTool");
            Directory.CreateDirectory(appDataPath);
            var keyFilePath = Path.Combine(appDataPath, "apikey.txt");
            File.WriteAllText(keyFilePath, ApiKey);
        }
        catch
        {
            // Ignore save errors
        }
    }

    private void LoadApiKey()
    {
        try
        {
            var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UDTool");
            var keyFilePath = Path.Combine(appDataPath, "apikey.txt");
            
            if (File.Exists(keyFilePath))
            {
                var savedKey = File.ReadAllText(keyFilePath);
                if (!string.IsNullOrWhiteSpace(savedKey))
                {
                    ApiKey = savedKey;
                    _ = CheckKeyAsync(); // Fire and forget
                }
            }
        }
        catch
        {
            // Ignore load errors
        }
    }
}
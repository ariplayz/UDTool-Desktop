using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace UDTool_Desktop.Models;

/// <summary>
/// A client for interacting with the UDTool API.
/// Provides methods for uploading, downloading, searching, deleting, and listing files.
/// </summary>
public class UDToolClient
{
    private const string BaseUrl = "https://UDTool.delphigamerz.xyz";
    private const string Version = "1.0";
    private readonly HttpClient _client;

    public UDToolClient()
    {
        _client = new HttpClient(new SocketsHttpHandler())
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    /// <summary>
    /// Uploads a file to the UDTool server.
    /// </summary>
    /// <param name="filePath">Path to the local file to upload</param>
    /// <param name="targetName">Name to save the file as on the server</param>
    /// <returns>A result object containing success status and messages</returns>
    public async Task<OperationResult> UploadAsync(string filePath, string targetName)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return OperationResult.FailureResult($"File not found: {filePath}");
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var fileName = Path.GetFileName(filePath);

            using (var content = new MultipartFormDataContent())
            {
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                content.Add(fileContent, "file", targetName);

                var response = await _client.PostAsync($"{BaseUrl}/{targetName}", content);

                if (response.IsSuccessStatusCode)
                {
                    return OperationResult.SuccessResult(
                        $"File successfully uploaded.\r\nFile URL: {BaseUrl}/{targetName}");
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    var errorMsg = $"Upload failed with status: {response.StatusCode}";
                    if (!string.IsNullOrEmpty(errorBody))
                    {
                        errorMsg += $"\r\nServer response: {errorBody}";
                    }
                    return OperationResult.FailureResult(errorMsg);
                }
            }
        }
        catch (Exception ex)
        {
            return OperationResult.FailureResult($"Upload error: {ex.Message}");
        }
    }

    /// <summary>
    /// Downloads a file from the UDTool server.
    /// </summary>
    /// <param name="fileName">Name of the file to download from the server</param>
    /// <param name="savePath">Optional path to save the file. If null, saves to current directory with original name</param>
    /// <returns>A result object containing success status and messages</returns>
    public async Task<OperationResult> DownloadAsync(string fileName, string? savePath = null)
    {
        try
        {
            var response = await _client.GetAsync($"{BaseUrl}/{fileName}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                var outputPath = savePath ?? fileName;
                await File.WriteAllBytesAsync(outputPath, content);
                return OperationResult.SuccessResult($"Downloaded {fileName} to {outputPath}");
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var errorMsg = $"Download failed with status: {response.StatusCode}";
                if (!string.IsNullOrEmpty(errorBody))
                {
                    errorMsg += $"\r\nServer response: {errorBody}";
                }
                return OperationResult.FailureResult(errorMsg);
            }
        }
        catch (Exception ex)
        {
            return OperationResult.FailureResult($"Download error: {ex.Message}");
        }
    }

    /// <summary>
    /// Searches for files on the UDTool server.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>A result object containing search results or error message</returns>
    public async Task<SearchResult> SearchAsync(string query)
    {
        try
        {
            var response = await _client.GetAsync($"{BaseUrl}/search/{query}");

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var files = System.Text.Json.JsonSerializer.Deserialize<List<string>>(jsonContent) ?? new List<string>();
                return SearchResult.SuccessResult(files);
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var errorMsg = $"Search failed with status: {response.StatusCode}";
                if (!string.IsNullOrEmpty(errorBody))
                {
                    errorMsg += $"\r\nServer response: {errorBody}";
                }
                return SearchResult.FailureResult(errorMsg);
            }
        }
        catch (Exception ex)
        {
            return SearchResult.FailureResult($"Search error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a file from the UDTool server.
    /// </summary>
    /// <param name="fileName">Name of the file to delete</param>
    /// <returns>A result object containing success status and messages</returns>
    public async Task<OperationResult> DeleteAsync(string fileName)
    {
        try
        {
            var response = await _client.DeleteAsync($"{BaseUrl}/{fileName}");

            if (response.IsSuccessStatusCode)
            {
                return OperationResult.SuccessResult($"File '{fileName}' successfully deleted.");
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var errorMsg = $"Delete failed with status: {response.StatusCode}";
                if (!string.IsNullOrEmpty(errorBody))
                {
                    errorMsg += $"\r\nServer response: {errorBody}";
                }
                return OperationResult.FailureResult(errorMsg);
            }
        }
        catch (Exception ex)
        {
            return OperationResult.FailureResult($"Delete error: {ex.Message}");
        }
    }

    /// <summary>
    /// Lists all files on the UDTool server.
    /// </summary>
    /// <returns>A result object containing the list of files or error message</returns>
    public async Task<SearchResult> ListAsync()
    {
        try
        {
            var response = await _client.GetAsync($"{BaseUrl}/list");

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var files = System.Text.Json.JsonSerializer.Deserialize<List<string>>(jsonContent) ?? new List<string>();
                return SearchResult.SuccessResult(files);
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var errorMsg = $"List failed with status: {response.StatusCode}";
                if (!string.IsNullOrEmpty(errorBody))
                {
                    errorMsg += $"\r\nServer response: {errorBody}";
                }
                return SearchResult.FailureResult(errorMsg);
            }
        }
        catch (Exception ex)
        {
            return SearchResult.FailureResult($"List error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets version information.
    /// </summary>
    /// <returns>The version string</returns>
    public string GetVersion() => Version;

    /// <summary>
    /// Disposes the HTTP client resources.
    /// </summary>
    public void Dispose()
    {
        _client?.Dispose();
    }
}

/// <summary>
/// Represents the result of a UDTool operation.
/// </summary>
public class OperationResult
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }

    private OperationResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static OperationResult SuccessResult(string message) => new(true, message);
    public static OperationResult FailureResult(string message) => new(false, message);
}

/// <summary>
/// Represents the result of a search or list operation.
/// </summary>
public class SearchResult
{
    public bool IsSuccess { get; private set; }
    public List<string> Files { get; private set; }
    public string ErrorMessage { get; private set; }

    private SearchResult(bool isSuccess, List<string> files, string errorMessage)
    {
        IsSuccess = isSuccess;
        Files = files ?? new List<string>();
        ErrorMessage = errorMessage;
    }

    public static SearchResult SuccessResult(List<string> files) => new(true, files, string.Empty);
    public static SearchResult FailureResult(string errorMessage) => new(false, new List<string>(), errorMessage);
}


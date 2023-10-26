using AzureBlobProject.Models;

namespace AzureBlobProject.Services;

public interface IBlobService
{
    Task<string> GetBlobAsync(string name, string containerName);

    Task<List<Blob>> GetAllBlobsWithUriAsync(string containerName);

    Task<List<string>> GetAllBlobsAsync(string containerName);

    Task<bool> UploadBlobAsync(string name, IFormFile file, string containerName, Blob blob);

    Task<bool> DeleteBlobAsync(string name, string containerName);
}
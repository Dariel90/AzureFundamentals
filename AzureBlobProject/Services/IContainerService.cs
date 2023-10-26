namespace AzureBlobProject.Services;

public interface IContainerService
{
    Task<List<string>> GetAllContainerAndBlobsAsync();

    Task<List<string>> GetAllContainerAsync();

    Task CreateContainerAsync(string containerName);

    Task DeleteContainerAsync(string containerName);
}
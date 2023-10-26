using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureBlobProject.Services;

public class ContainerService : IContainerService
{
    private readonly BlobServiceClient blobServiceClient;

    public ContainerService(BlobServiceClient blobServiceClient)
    {
        this.blobServiceClient = blobServiceClient;
    }

    public async Task<List<string>> GetAllContainerAndBlobsAsync()
    {
        List<string> containerAndBlobName = new();
        containerAndBlobName.Add("Account Name : " + this.blobServiceClient.AccountName);
        containerAndBlobName.Add("--------------------------------------------");
        await foreach (BlobContainerItem blobContainerItem in blobServiceClient.GetBlobContainersAsync())
        {
            containerAndBlobName.Add("---" + blobContainerItem.Name);

            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerItem.Name);

            await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
            {
                //get metadata
                var bobClient = blobContainerClient.GetBlobClient(blobItem.Name);

                BlobProperties blobProperties = await bobClient.GetPropertiesAsync();
                string blobToAdd = blobItem.Name;
                if (blobProperties.Metadata.ContainsKey("title"))
                {
                    blobToAdd += $"({blobProperties.Metadata["title"]})";
                }

                containerAndBlobName.Add("------" + blobToAdd);
            }
            containerAndBlobName.Add("--------------------------------------------");
        }
        return containerAndBlobName;
    }

    public async Task<List<string>> GetAllContainerAsync()
    {
        List<string> containerNames = new();
        await foreach (BlobContainerItem blobContainerItem in blobServiceClient.GetBlobContainersAsync())
        {
            containerNames.Add(blobContainerItem.Name);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerItem.Name);
        }

        return containerNames;
    }

    public async Task CreateContainerAsync(string containerName)
    {
        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
    }

    public async Task DeleteContainerAsync(string containerName)
    {
        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

        await blobContainerClient.DeleteIfExistsAsync();
    }
}
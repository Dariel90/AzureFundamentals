using Azure.Storage.Blobs;
using System.Linq;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using AzureBlobProject.Models;

namespace AzureBlobProject.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient blobServiceClient;

    public BlobService(BlobServiceClient blobServiceClient)
    {
        this.blobServiceClient = blobServiceClient;
    }

    public async Task<string> GetBlobAsync(string name, string containerName)
    {
        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

        return blobContainerClient.GetBlobClient(name).Uri.AbsoluteUri;
    }

    public async Task<List<Blob>> GetAllBlobsWithUriAsync(string containerName)
    {
        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

        var blobs = blobContainerClient.GetBlobsAsync();
        var blobList = new List<Blob>();
        //var sasContainerSignature = string.Empty;
        //if (blobContainerClient.CanGenerateSasUri)
        //{
        //    BlobSasBuilder sasBuilder = new()
        //    {
        //        BlobContainerName = blobContainerClient.Name,
        //        Resource = "b",
        //        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        //    };
        //    sasBuilder.SetPermissions(BlobSasPermissions.Read);

        //    sasContainerSignature = blobContainerClient.GenerateSasUri(sasBuilder).AbsoluteUri.Split('?')[1].ToString();
        //}

        await foreach (var item in blobs)
        {
            var blobClient = blobContainerClient.GetBlobClient(item.Name);

            var blobIndividual = new Blob
            {
                Uri = blobClient.Uri.AbsoluteUri /*+ "?" + sasContainerSignature*/,
            };

            //if (blobClient.CanGenerateSasUri)
            //{
            //    BlobSasBuilder sasBuilder = new()
            //    {
            //        BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
            //        BlobName = blobClient.Name,
            //        Resource = "b",
            //        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            //    };
            //    sasBuilder.SetPermissions(BlobSasPermissions.Read);

            //    blobIndividual.Uri = blobClient.GenerateSasUri(sasBuilder).AbsoluteUri;
            //}

            BlobProperties blobProperties = await blobClient.GetPropertiesAsync();

            if (blobProperties.Metadata.ContainsKey("title"))
            {
                blobIndividual.Title = blobProperties.Metadata["title"];
            }

            if (blobProperties.Metadata.ContainsKey("comment"))
            {
                blobIndividual.Title = blobProperties.Metadata["comment"];
            }
            blobList.Add(blobIndividual);
        }
        return blobList;
    }

    public async Task<List<string>> GetAllBlobsAsync(string containerName)
    {
        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobs = blobContainerClient.GetBlobsAsync();

        var blobList = new List<string>();
        await foreach (var item in blobs)
        {
            blobList.Add(item.Name);
        }

        return blobList;
    }

    public async Task<bool> UploadBlobAsync(string name, IFormFile file, string containerName, Blob blob)
    {
        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

        var blobClient = blobContainerClient.GetBlobClient(name);

        var httpHeaders = new BlobHttpHeaders
        {
            ContentType = file.ContentType
        };
        IDictionary<string, string> metadata = new Dictionary<string, string>()
        {
            { "title", blob.Title },
            { "comment", blob.Comment }
        };

        var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders, metadata);

        //metadata.Remove("title");
        //await blobClient.SetMetadataAsync(metadata);

        if (result != null)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteBlobAsync(string name, string containerName)
    {
        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

        var blobClient = blobContainerClient.GetBlobClient(name);

        return await blobClient.DeleteIfExistsAsync();
    }
}
using AzureBlobProject.Models;
using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers;

public class BlobController : Controller
{
    private readonly IBlobService blobService;

    public BlobController(IBlobService blobService)
    {
        this.blobService = blobService;
    }

    [HttpGet]
    public async Task<IActionResult> Manage(string containerName)
    {
        var blobsObj = await blobService.GetAllBlobsAsync(containerName);
        return this.View(blobsObj);
    }

    [HttpGet]
    public IActionResult AddFile(string containerName)
    {
        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> AddFile(string containerName, Blob blob, IFormFile file)
    {
        if (file.Length < 1)
        {
            return this.View();
        }

        var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        var result = await blobService.UploadBlobAsync(fileName, file, containerName, blob);
        if (result)
        {
            return this.RedirectToAction("Index", "Container");
        }

        return this.View(result);
    }

    [HttpGet]
    public async Task<IActionResult> ViewFile(string name, string containerName)
    {
        return this.Redirect(await this.blobService.GetBlobAsync(name, containerName));
    }

    public async Task<IActionResult> DeleteFile(string name, string containerName)
    {
        await this.blobService.DeleteBlobAsync(name, containerName);
        return this.RedirectToAction("Index", "Home");
    }
}
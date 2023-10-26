using System.ComponentModel;
using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers;

public class ContainerController : Controller
{
    private readonly IContainerService containerService;

    public ContainerController(IContainerService containerService)
    {
        this.containerService = containerService;
    }

    public async Task<IActionResult> Index()
    {
        var allContainers = await this.containerService.GetAllContainerAsync();
        return this.View(allContainers);
    }

    public async Task<IActionResult> Create()
    {
        return this.View(new Models.Container());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Models.Container container)
    {
        await this.containerService.CreateContainerAsync(container.Name);
        return this.RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string containerName)
    {
        await this.containerService.DeleteContainerAsync(containerName);
        return this.RedirectToAction(nameof(Index));
    }
}
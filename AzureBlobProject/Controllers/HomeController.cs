using AzureBlobProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AzureBlobProject.Services;

namespace AzureBlobProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IContainerService containerService;
        private readonly IBlobService blobService;

        public HomeController(ILogger<HomeController> logger, IContainerService containerService, IBlobService blobService)
        {
            _logger = logger;
            this.containerService = containerService;
            this.blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {
            return this.View(await this.containerService.GetAllContainerAndBlobsAsync());
        }

        public async Task<IActionResult> Images()
        {
            return this.View(await this.blobService.GetAllBlobsWithUriAsync("privatecontainer"));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
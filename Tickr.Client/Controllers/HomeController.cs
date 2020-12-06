namespace Tickr.Client.Controllers
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models;
    using Services;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TodoService _todoService;

        public HomeController(ILogger<HomeController> logger, TodoService todoService)
        {
            _logger = logger;
            _todoService = todoService;
        }

        public async  Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Fetching todo items from the server");
            var data = await _todoService.GetAllTodo(cancellationToken);
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

     
    }
}

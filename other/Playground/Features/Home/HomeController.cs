using Microsoft.AspNetCore.Mvc;

namespace Playground.Features.Home
{
    public class HomeController : Controller
    {
        public IActionResult Index() =>
            new RedirectResult("~/swagger");
    }
}

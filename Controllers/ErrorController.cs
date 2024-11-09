using Microsoft.AspNetCore.Mvc;

namespace EstateProManager.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult E404()
        {
            ViewBag.IDROLE = HttpContext.Session.GetString("IDROLE");
            return View("E404");
        }

        [HttpGet]
        public IActionResult E500()
        {
            ViewBag.IDROLE = HttpContext.Session.GetString("IDROLE");
            return View("E500");
        }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReceptionSystem.Models;
using System.Diagnostics;

namespace ReceptionSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // =====================================================
        // GET: HOME
        // =====================================================

        public IActionResult Index()
        {
            return View();
        }

        // =====================================================
        // ERROR
        // =====================================================

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId =
                        Activity.Current?.Id
                        ?? HttpContext.TraceIdentifier
                });
        }
    }
}


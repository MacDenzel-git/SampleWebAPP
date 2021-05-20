using Microsoft.AspNetCore.Mvc;
using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.Controllers
{
    public class DashboardController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var user = await StaticDataHandler.GetSessionDetails();
            if (user.IsSet)
            {
                return View();
            }
            return RedirectToAction("Login", "Account");
        }
    }
}

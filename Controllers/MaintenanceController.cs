using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{
    public class MaintenanceController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Maintenance/Maintenance.cshtml");
    }
    }
}

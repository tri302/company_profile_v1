using company_profile.Models;
using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{    
    public class ErrorController : Controller    
    {
        [Route("Error/NotFound")]
        public IActionResult NotFoundPage()
        {
            return View("NotFound");
        }

        [Route("Error/Forbidden")]
        public IActionResult Forbidden()
        {
            return View("Forbidden");
        }

        [Route("Error/ServerError")]
        public IActionResult ServerError()
        {
            var model = new ErrorViewModel
            {
                RequestId = Guid.NewGuid().ToString() // Membuat ID unik untuk error ini
            };
            return View(model);            
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            return statusCode switch
            {
                404 => View("NotFound"),
                403 => View("Forbidden"),
                _ => View("ServerError")
            };
        }

    }
}

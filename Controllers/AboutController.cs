using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{
    [Authorize]
    public class AboutController : Controller
    {
        private readonly IAboutRepo _repo;

        public AboutController(IAboutRepo repo)
        {
            _repo = repo;
        }               

        public async Task<IActionResult> Index()
        {
            var data = await _repo.GetAllAboutAsync();
            return View(data);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AboutVM aboutVM)
        {            
            if (ModelState.IsValid)
            {                
                if (ModelState.IsValid)
                {
                    await _repo.CreateAboutAsync(aboutVM);
                    TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(aboutVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _repo.GetAboutByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AboutVM aboutVM)
        {
            if (ModelState.IsValid)
            {                
                if (ModelState.IsValid)
                {
                    var success = await _repo.UpdateAboutAsync(aboutVM);
                    TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                    if (success) return RedirectToAction(nameof(Index));
                }
            }
            return View(aboutVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "Data tidak ditemukan." });
            }
            try
            {
                await _repo.DeleteAboutAsync(id);
                return Json(new { success = true, message = "Data berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }        
    }
}
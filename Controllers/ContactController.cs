using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly IContactRepo _repo;        

        public ContactController(IContactRepo repo)
        {
            _repo = repo;            
        }

        public async Task<IActionResult> Index()
        {
            var data = await _repo.GetAllContactAsync();
            return View(data);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactVM contactVM)
        {
            if (ModelState.IsValid)
            {
                await _repo.CreateContactAsync(contactVM);
                TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                return RedirectToAction(nameof(Index));
            }
            return View(contactVM);
        }        

        public async Task<IActionResult> Edit(int id)
        {
            var data = await _repo.GetContactByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactVM contactVM)
        {
            if (ModelState.IsValid)
            {
                var success = await _repo.UpdateContactAsync(contactVM);
                TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                if (success) return RedirectToAction(nameof(Index));
            }
            return View(contactVM);
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
                await _repo.DeleteContactAsync(id);
                return Json(new { success = true, message = "Data berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }        
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{
    [Authorize]
    public class OurServicesController : Controller
    {
        private readonly IOurServicesRepo _repo;
        private const int PageSize = 5;
        public OurServicesController(IOurServicesRepo repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int pageIndex = 1;
            var viewModel = await _repo.GetPagedDataAsync(pageIndex, PageSize);
            return View(viewModel);
        }

        //Update Method GetPaginatedData (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetPaginatedData(int pageNumber = 1)
        {
            if (pageNumber < 1) pageNumber = 1;
            var viewModel = await _repo.GetPagedDataAsync(pageNumber, PageSize);
            return PartialView("~/Views/OurServices/_OurServicesPartial.cshtml", viewModel);
        }
        
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OurServicesVM ourServicesVM)
        {
            try
            {
                // 3. Jika validasi helper lolos dan ModelState lainnya valid
                if (ModelState.IsValid)
                {
                    await _repo.CreateOurServicesAsync(ourServicesVM);
                    TempData["SuccessMessage"] = "Data berhasil disimpan!";
                    return RedirectToAction(nameof(Index));
                }

                // Jika gagal, kembali ke form dengan pesan error
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating our services: {ex.Message}");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var data = await _repo.GetOurServicesByIdAsync(id);
                if (data == null) return NotFound();
                return View(data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading our services for edit: {ex.Message}");
                return View("Error");
            }
            finally
            {
                // Optional: Any cleanup code can go here
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OurServicesVM ourServicesVM)
        {
            try
            {
                // Additional logging can be added here if needed       
                if (ModelState.IsValid)
                {
                    var success = await _repo.UpdateOurServicesAsync(ourServicesVM);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Data berhasil disimpan!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                return View(ourServicesVM);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating our services: {ex.Message}");
                return View("Error");
            }
            finally
            {
                // Optional: Any cleanup code can go here
            }
        }


        [HttpPost] // Gunakan form post untuk delete demi keamanan
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return Json(new { success = false, message = "Data tidak ditemukan" });
            try
            {
                await _repo.DeleteOurServicesAsync(id);
                return Json(new { success = true, message = "Our Services berhasil dihapus." });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting our services: {ex.Message}");
                // return Json(new { success = false, message = ex.Message });
                return Json(new { success = false, message = "Terjadi kesalahan saat menghapus our services." });
            }
            finally
            {
                // Optional: Any cleanup code can go here
            }
        }
    }
}
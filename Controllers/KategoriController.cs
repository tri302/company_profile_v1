using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{
    [Authorize]
    public class KategoriController : Controller
    {
        private readonly IKategoriRepo _repo;

        public KategoriController(IKategoriRepo repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var data = await _repo.GetAllKategoriAsync();
                return View(data);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog or NLog)
                System.Diagnostics.Debug.WriteLine($"Error loading Kategori: {ex.Message}");
                // Return an error view or handle appropriately
                return View("Error");
            }
            finally
            {
                // Optional: Any cleanup code can go here
            }
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KategoriVM kategoriVM)
        {
            try
            {
                // 3. Jika validasi helper lolos dan ModelState lainnya valid
                if (ModelState.IsValid)
                {
                    await _repo.CreateKategoriAsync(kategoriVM);
                    TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                    return RedirectToAction(nameof(Index));
                }

                // Jika gagal, kembali ke form dengan pesan error
                return View(kategoriVM);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating kategori: {ex.Message}");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var data = await _repo.GetKategoriByIdAsync(id);
                if (data == null) return NotFound();
                return View(data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading kategori for edit: {ex.Message}");
                return View("Error");
            }
            finally
            {
                // Optional: Any cleanup code can go here
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(KategoriVM kategoriVM)
        {
            try
            {
                // Additional logging can be added here if needed       
                if (ModelState.IsValid)
                {
                    var success = await _repo.UpdateKategoriAsync(kategoriVM);
                    TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                    return RedirectToAction(nameof(Index));
                }
                return View(kategoriVM);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating kategori: {ex.Message}");
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
                await _repo.DeleteKategoriAsync(id);
                return Json(new { success = true, message = "Kategori berhasil dihapus." });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting kategori: {ex.Message}");
                // return Json(new { success = false, message = ex.Message });
                return Json(new { success = false, message = "Terjadi kesalahan saat menghapus kategori." });
            }
            finally
            {
                // Optional: Any cleanup code can go here
            }
        }
    }
}
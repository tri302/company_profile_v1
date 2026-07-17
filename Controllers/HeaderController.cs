using Microsoft.AspNetCore.Mvc;
using company_profile.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace company_profile.Controllers
{
    [Authorize]
    public class HeaderController : Controller
    {
        private readonly IHeaderRepo _headerRepo;

        // Dependency Injection untuk memasukkan repositori ke Controller
        public HeaderController(IHeaderRepo headerRepo)
        {
            _headerRepo = headerRepo;
        }

        // =========================================================================
        // 1. GET: Header (INDEX)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var listData = await _headerRepo.GetAllAsync();
                return View(listData);
            }
            catch (Exception ex)
            {
                // Anda bisa melakukan logging error di sini menggunakan ILogger (opsional)
                TempData["ErrorMessage"] = $"Terjadi kesalahan saat memuat data: {ex.Message}";
                return View(System.Array.Empty<HeaderVM>());
            }
        }

        // =========================================================================
        // 2. GET: Header/Details/5
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("ID tidak valid.");
                }

                var data = await _headerRepo.GetByIdAsync(id);
                if (data == null)
                {
                    return NotFound($"Data Header dengan ID {id} tidak ditemukan.");
                }

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Gagal mengambil rincian data: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // =========================================================================
        // 3. GET: Header/Create
        // =========================================================================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // =========================================================================
        // 4. POST: Header/Create
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HeaderVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var isSuccess = await _headerRepo.CreateAsync(model);
                    if (isSuccess)
                    {
                        TempData["SuccessMessage"] = "Data Profile Header berhasil disimpan!";
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError(string.Empty, "Gagal menyimpan ke database. Silakan coba lagi.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Terjadi error sistem: {ex.Message}");
            }

            // Jika form tidak valid atau gagal simpan, kembalikan ke halaman form beserta data lama
            return View(model);
        }

        // =========================================================================
        // 5. GET: Header/Edit/5
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("ID tidak valid.");
                }

                var data = await _headerRepo.GetByIdAsync(id);
                if (data == null)
                {
                    return NotFound();
                }

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Gagal memuat form edit: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // =========================================================================
        // 6. POST: Header/Edit/5
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HeaderVM model)
        {
            try
            {
                if (id != model.ID)
                {
                    return BadRequest("Mismatched ID antara URL dengan data form.");
                }

                if (ModelState.IsValid)
                {
                    var isSuccess = await _headerRepo.UpdateAsync(model);
                    if (isSuccess)
                    {
                        TempData["SuccessMessage"] = "Perubahan data Profile Header berhasil disimpan!";
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError(string.Empty, "Gagal memperbarui data. Data mungkin sudah terhapus.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Terjadi error sistem saat memperbarui: {ex.Message}");
            }

            return View(model);
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
                await _headerRepo.DeleteAsync(id);
                return Json(new { success = true, message = "Data berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
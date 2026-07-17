using company_profile;
using company_profile.PagenationVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class AchievementsController : Controller
{
    private readonly IAchievementsRepo _achievementsRepo;    
    private const int PageSize = 5;

    public AchievementsController(IAchievementsRepo achievementsRepo)
    {
        _achievementsRepo = achievementsRepo;        
    }

    // Endpoint AJAX untuk memuat tabel parsial
    [HttpGet]
    public async Task<IActionResult> GetPaginatedData(int pageNumber = 1)
    {
        var items = await _achievementsRepo.GetPagedDataAsync(pageNumber, PageSize);
        var totalCount = await _achievementsRepo.CountAsync();

        var viewModel = new AchievementsListViewModel
        {
            Items = items,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
        };

        // Mengembalikan Partial View agar bisa dirender ke dalam container tabel
        return PartialView("~/Views/Achievements/_AchievementsTablePartial.cshtml", viewModel);
    }

    // GET
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            int initialPage = 1;

            // Cukup ambil data yang diperlukan saja
            var items = await _achievementsRepo.GetPagedDataAsync(initialPage, PageSize);
            var totalCount = await _achievementsRepo.CountAsync();

            var viewModel = new AchievementsListViewModel
            {
                Items = items,
                CurrentPage = initialPage,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            // Log error di sini menggunakan _logger
            Console.WriteLine(ex.Message);
            TempData["ErrorMessage"] = "Terjadi kesalahan saat memuat data";
            return View(new AchievementsListViewModel { Items = new List<CounterVM>() });
        }
    }

    // GET: Achievements
    //public async Task<IActionResult> Index()
    //{
    //    var data = await _achievementsRepo.GetAllAsync();
    //    return View(data);
    //}

    // GET: Achievements/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var achievement = await _achievementsRepo.GetByIdAsync(id);
        if (achievement == null)
        {
            TempData["ErrorMessage"] = "Data achievement tidak ditemukan.";
            return RedirectToAction(nameof(Index));
        }
        return View(achievement);
    }

    // GET: Achievements/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Achievements/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CounterVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            bool isSuccess = await _achievementsRepo.CreateAsync(model);
            if (isSuccess)
            {
                TempData["SuccessMessage"] = "Data achievement baru berhasil disimpan!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Gagal menyimpan data ke database. Silakan coba lagi.");
        }
        catch (Exception ex)
        {
            // Log exception disini jika Anda menggunakan logger (misal: _logger.LogError(ex, "..."))
            ModelState.AddModelError("", $"Terjadi kesalahan sistem: {ex.Message}");
        }

        return View(model);
    }

    // GET: Achievements/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var achievement = await _achievementsRepo.GetByIdAsync(id);
        if (achievement == null)
        {
            TempData["ErrorMessage"] = "Data yang ingin diubah tidak ditemukan.";
            return RedirectToAction(nameof(Index));
        }
        return View(achievement);
    }

    // POST: Achievements/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CounterVM model)
    {
        if (id != model.Id)
        {
            TempData["ErrorMessage"] = "ID Data tidak cocok.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            bool isSuccess = await _achievementsRepo.UpdateAsync(model);
            if (isSuccess)
            {
                TempData["SuccessMessage"] = "Data achievement berhasil diperbarui!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Gagal memperbarui data. Kemungkinan data sudah dihapus oleh pengguna lain.");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Gagal memperbarui data karena kesalahan sistem: {ex.Message}");
        }

        return View(model);
    }

    // POST: Achievements/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken] // Tetap aman dari CSRF
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            bool isSuccess = await _achievementsRepo.DeleteAsync(id);
            if (isSuccess)
            {
                return Json(new { success = true, message = "Data achievement berhasil dihapus!" });
            }
            else
            {
                return Json(new { success = false, message = "Gagal menghapus data. Data mungkin sudah tidak ada." });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Gagal menghapus data karena kesalahan sistem: {ex.Message}" });
        }
    }
}
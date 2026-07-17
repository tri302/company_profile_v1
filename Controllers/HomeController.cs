using company_profile.Helper;
using company_profile.ViewModels.HomeVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class HomeController : Controller
{
    private readonly IHomeRepo _homeRepo;
    private readonly ILogger<HomeController> _logger;
    private const int PageSize = 5;
    public HomeController(IHomeRepo homeRepo, ILogger<HomeController> logger)
    {
        _homeRepo = homeRepo;
        _logger = logger;
    }

    // Endpoint khusus untuk cron-job.org
    [HttpGet("/ping-endpoint")]
    public IActionResult Ping()
    {
        _logger.LogInformation("Web diping oleh cron-job.org pada: {Time}", System.DateTime.Now);
        // Mengembalikan respon teks sederhana 
        return Content("Aplikasi Aktif!", "text/plain");
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        try
        {
            var GetDetails = _homeRepo.DetailsAsync(id).Result; // Mengambil data dengan ID yang diberikan untuk ditampilkan di halaman Details
            if (GetDetails == null)
            {
                TempData["ErrorMessage"] = "Data tidak ditemukan.";
                return RedirectToAction(nameof(Index));
            }
            return View(GetDetails);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Gagal memuat data detail: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // ==========================================
    // 1. HALAMAN UTAMA / DASHBOARD ADMIN (INDEX)
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int pageIndex = 1;
        var viewModel = await _homeRepo.GetPagedDataAsync(pageIndex, PageSize);
        return View(viewModel);
    }

    //Update Method GetPaginatedData (AJAX)
    [HttpGet]
    public async Task<IActionResult> GetPaginatedData(int pageNumber = 1)
    {
        if (pageNumber < 1) pageNumber = 1;
        var viewModel = await _homeRepo.GetPagedDataAsync(pageNumber, PageSize);
        return PartialView("~/Views/Home/_HomePartial.cshtml", viewModel);
    }

    //[HttpGet]
    //public async Task<IActionResult> Index()
    //{
    //    try
    //    {
    //        var listData = await _homeRepo.GetAllAsync();
    //        return View(listData);
    //    }
    //    catch (Exception ex)
    //    {
    //        // Log exception di sini jika Anda menggunakan logger (misal: _logger.LogError)
    //        TempData["ErrorMessage"] = $"Gagal memuat data dari server. Silakan coba beberapa saat lagi. {ex.Message}";
    //        return View(Array.Empty<HomeVM>());
    //    }
    //}

    // ==========================================
    // 2. AMBIL HALAMAN TAMBAH DATA (CREATE - GET)
    // ==========================================
    public IActionResult Create()
    {
        return View(new HomeVM());
    }

    // ==========================================
    // 3. PROSES SIMPAN DATA BARU (CREATE - POST)
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SanitizeHtml] // Otomatis membersihkan konten sebelum masuk ke method
    public async Task<IActionResult> Create(HomeVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var isSaved = await _homeRepo.InsertAsync(model);

            if (isSaved)
            {
                TempData["SuccessMessage"] = "Data konten beranda berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }

            // Jika gagal karena validasi internal gambar di Repository (ukuran > 2MB / ekstensi salah)
            ModelState.AddModelError("", "Gagal menyimpan data. Pastikan file berupa gambar dan berukuran maksimal 2MB.");
            return View(model);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Terjadi kesalahan sistem saat menyimpan data: {ex.Message}");
            return View(model);
        }
    }

    // ==========================================
    // 4. AMBIL HALAMAN UBAH DATA (EDIT - GET)
    // ==========================================
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var data = await _homeRepo.GetByIdAsync(id);
            if (data == null)
            {
                TempData["ErrorMessage"] = "Data yang ingin diubah tidak ditemukan.";
                return RedirectToAction(nameof(Index));
            }
            return View(data);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Terjadi kesalahan saat mengambil data untuk diedit. {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // ==========================================
    // 5. PROSES SIMPAN PERUBAHAN DATA (EDIT - POST)
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SanitizeHtml] // Otomatis membersihkan konten sebelum masuk ke method
    public async Task<IActionResult> Edit(HomeVM model)
    {        
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {           
            var isUpdated = await _homeRepo.UpdateAsync(model);

            if (isUpdated)
            {
                TempData["SuccessMessage"] = "Perubahan data konten beranda berhasil disimpan!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Terjadi kesalahan sistem saat memperbarui data: {ex.Message}");
            return View(model);
        }
    }

    // ==========================================
    // 6. PROSES HAPUS DATA (DELETE)
    // ==========================================
    [HttpPost] // Pastikan ada attribute ini karena AJAX kamu menggunakan type: 'POST'
    public async Task<IActionResult> Delete(int id)
    {
        try
        {            
            var isDeleted = await _homeRepo.DeleteAsync(id);

            if (isDeleted)
            {
                // Kembalikan objek JSON sukses ke AJAX
                return Json(new { success = true, message = "Data beserta berkas lampiran berhasil dihapus dari sistem!" });
            }
            else
            {
                // Kembalikan objek JSON gagal ke AJAX
                return Json(new { success = false, message = "Data gagal dihapus atau data tidak ditemukan di database." });
            }
        }
        catch (Exception ex)
        {
            // Kembalikan objek JSON error sistem ke AJAX
            return Json(new { success = false, message = $"Gagal menghapus data karena kesalahan sistem: {ex.Message}" });
        }
    }    
}

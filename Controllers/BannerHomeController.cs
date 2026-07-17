using Microsoft.AspNetCore.Mvc;
using company_profile.ViewModels;
using company_profile.Helper;
using Microsoft.AspNetCore.Authorization;

namespace company_profile.Controllers
{
    [Authorize]
    public class BannerHomeController : Controller
    {
        private readonly IBannerHomeRepo _bannerRepo;

        // Dependency Injection untuk Repository
        public BannerHomeController(IBannerHomeRepo bannerRepo)
        {
            _bannerRepo = bannerRepo;
        }

        // 1. GET: BannerHome (Halaman Utama / Daftar Banner)
        public async Task<IActionResult> Index()
        {
            try
            {
                var banners = await _bannerRepo.GetAllAsync();
                return View(banners);
            }
            catch (Exception ex)
            {
                // Log exception jika diperlukan (misal: Serilog / NLog)
                TempData["ErrorMessage"] = "Gagal mengambil data dari server: " + ex.Message;
                return View(new List<BannerHomeVM>());
            }
        }

        // 2. GET: BannerHome/Create (Halaman Form Tambah)
        public IActionResult Create()
        {
            return View();
        }

        // 3. POST: BannerHome/Create (Proses Simpan Data Baru)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SanitizeHtml] // Otomatis membersihkan konten sebelum masuk ke method
        public async Task<IActionResult> Create(BannerHomeVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Jika user mengunggah berkas gambar
                if (model.file != null && model.file.Length > 0)
                {
                    // Fungsi UploadImageAsync di repo sudah otomatis memvalidasi file max 2MB & ekstensi
                    string imageUrl = await _bannerRepo.UploadImageAsync(model.file);
                    model.UrlGambar = imageUrl;
                }

                await _bannerRepo.InsertAsync(model);
                await _bannerRepo.SaveAsync();

                TempData["SuccessMessage"] = "Data banner berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }
            catch (BadImageFormatException ex)
            {
                // Menangkap error spesifik validasi gambar (Ukuran > 2MB atau Ekstensi salah)
                ModelState.AddModelError("file", ex.Message);
            }
            catch (Exception ex)
            {
                // Menangkap error umum (Database down, Storage penuh, dll)
                ModelState.AddModelError("", "Terjadi kesalahan sistem saat menyimpan data: " + ex.Message);
            }

            return View(model);
        }

        // 4. GET: BannerHome/Edit/5 (Halaman Form Ubah)
        // GET: BannerHome/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var banner = await _bannerRepo.GetByIdAsync(id);
                if (banner == null)
                {
                    TempData["ErrorMessage"] = "Data banner tidak ditemukan.";
                    return RedirectToAction(nameof(Index));
                }

                //Mapping dari Entity ke ViewModel agar data muncul di View
                var viewModel = new BannerHomeVM
                {
                    Id = banner.Id,
                    JudulTextAnimatedOnBanner = banner.JudulTextAnimatedOnBanner,
                    IsiTextAnimatedOnBanner = banner.IsiTextAnimatedOnBanner,
                    // Pemetaan Kolom Multi-Bahasa Baru
                    JudulTextAnimatedOnBanner_ID = banner.JudulTextAnimatedOnBanner_ID,
                    JudulTextAnimatedOnBanner_EN = banner.JudulTextAnimatedOnBanner_EN,
                    IsiTextAnimatedOnBanner_ID = banner.IsiTextAnimatedOnBanner_ID,
                    IsiTextAnimatedOnBanner_EN = banner.IsiTextAnimatedOnBanner_EN,
                    
                    UrlGambar = banner.UrlGambar
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Gagal memuat data: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: BannerHome/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SanitizeHtml] // Otomatis membersihkan konten sebelum masuk ke method
        public async Task<IActionResult> Edit(int id, BannerHomeVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Jika user mengunggah berkas gambar baru
                if (model.file != null && model.file.Length > 0)
                {
                    var existingData = await _bannerRepo.GetByIdAsync(model.Id);
                    if (existingData != null && !string.IsNullOrEmpty(existingData.UrlGambar))
                    {
                        // Hapus file fisik gambar lama dari server
                        _bannerRepo.DeleteOldImage(existingData.UrlGambar);
                    }

                    // Upload gambar baru
                    string newImageUrl = await _bannerRepo.UploadImageAsync(model.file);
                    model.UrlGambar = newImageUrl;
                }
                else
                {
                    // PERBAIKAN: Jika tidak ganti gambar, pastikan model tetap membawa UrlGambar lama dari database
                    var existingData = await _bannerRepo.GetByIdAsync(model.Id);
                    if (existingData != null)
                    {
                        model.UrlGambar = existingData.UrlGambar;
                    }
                }               

                await _bannerRepo.UpdateAsync(model);
                await _bannerRepo.SaveAsync();

                TempData["SuccessMessage"] = "Data banner berhasil diperbarui!";
                return RedirectToAction(nameof(Index));
            }
            catch (BadImageFormatException ex)
            {
                ModelState.AddModelError("file", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Gagal memperbarui data: " + ex.Message);
            }

            return View(model);
        }

        // 6. POST/GET: BannerHome/Delete/5 (Proses Hapus Data)
        // Disarankan menggunakan POST/HttpPost demi keamanan (mencegah eksploitasi URL penjelajah)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {                
                // Panggil fungsi delete di repo yang juga otomatis menghapus file gambar di dalam folder wwwroot
                await _bannerRepo.DeleteAsync(id);
                await _bannerRepo.SaveAsync();

                // Kembalikan respons JSON sukses
                return Json(new { success = true, message = "Data banner berhasil dihapus!" });
            }
            catch (Exception ex)
            {
                // Kembalikan respons JSON error
                return Json(new { success = false, message = "Gagal menghapus data: " + ex.Message });
            }
        }
    }
}
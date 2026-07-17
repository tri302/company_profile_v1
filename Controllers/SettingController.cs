using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace company_profile.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;

        public SettingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSeo([Bind(Prefix = "SeoMetadata")] SeoMetadata model)
        {
            try
            {
                // 1. Cari data yang sudah ada di database
                var entity = await _context.SeoMetadata.FindAsync(model.Id);
                if (entity == null) return Json(new { success = false, message = "Data tidak ditemukan." });

                // 2. Update data teks (selalu dilakukan)
                entity.MetaTitle = model.MetaTitle;
                entity.MetaDescription = model.MetaDescription;
                entity.MetaKeywords = model.MetaKeywords;
                entity.CanonicalUrl = model.CanonicalUrl;

                // 3. Update gambar (HANYA jika user mengupload file baru)
                if (model.NewFoto != null && model.NewFoto.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.NewFoto.FileName);
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/");

                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.NewFoto.CopyToAsync(fileStream);
                    }

                    // Hapus file lama jika ada
                    if (!string.IsNullOrEmpty(entity.OgImageUrl))
                    {
                        DeleteOldImage(entity.OgImageUrl);
                    }

                    entity.OgImageUrl = "/uploads/" + uniqueFileName;
                }

                // 4. Simpan perubahan ke database
                _context.SeoMetadata.Update(entity);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "SEO berhasil diperbarui!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Terjadi kesalahan: " + ex.Message });
            }
        }

        public IActionResult Index()
        {
            var viewModel = new SettingsViewModel
            {
                LogoSetting = _context.LogoSettings.Where(x => x.Tipe == "Logo").FirstOrDefault() ?? new LogoSetting(),
                SettingFavicon = _context.LogoSettings.Where(x => x.Tipe == "Favicon").FirstOrDefault() ?? new LogoSetting(),
                EmailSetting = _context.EmailSettings.FirstOrDefault() ?? new EmailSetting(),
                PageBanners = _context.PageBanners.ToList(), // Ambil semua data banner halaman     
                Theme = _context.Themes.FirstOrDefault() ?? new Theme { HeaderColor = "#0948B6", FooterColor = "#0948B6" },
                SeoMetadata = _context.SeoMetadata.FirstOrDefault() ?? new SeoMetadata()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLogo(SettingsViewModel model)
        {
            try
            {
                if (model.LogoSetting.NewLogo != null && model.LogoSetting.NewLogo.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.LogoSetting.NewLogo.FileName;
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/");

                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.LogoSetting.NewLogo.CopyToAsync(fileStream);
                    }

                    var entity = await _context.LogoSettings.FindAsync(model.LogoSetting.Id);
                    if (entity != null)
                    {
                        string? oldImageName = entity.UrlGambar;
                        DeleteOldImage(oldImageName);

                        entity.Tipe = "Logo";
                        entity.UrlGambar = "/uploads/" + uniqueFileName;
                        _context.LogoSettings.Update(entity);
                        await _context.SaveChangesAsync();
                    }

                    return Json(new { success = true, message = "Logo berhasil diperbarui!" });
                }

                return Json(new { success = false, message = "Silakan pilih file logo terlebih dahulu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFavicon([Bind(Prefix = "SettingFavicon")] LogoSetting model)
        {
            try
            {
                if (model.NewFavicon != null && model.NewFavicon.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.NewFavicon.FileName;
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/");

                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.NewFavicon.CopyToAsync(fileStream);
                    }

                    // Ambil entitas lama dari database untuk mendapatkan nama file favicon lama sebelum di-update
                    var entity = await _context.LogoSettings.FindAsync(model.Id);
                    if (entity != null)
                    {
                        DeleteOldImage(entity.UrlGambar);
                        entity.UrlGambar = "/uploads/" + uniqueFileName; // atau "/uploads/" + uniqueFileName jika ingin seragam dengan logo
                        entity.Tipe = "Favicon";
                        _context.LogoSettings.Update(entity);
                    }
                    else
                    {
                        model.UrlGambar = "/uploads/" + uniqueFileName;
                        model.Tipe = "Favicon";
                        _context.LogoSettings.Update(model);
                    }
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Favicon berhasil diperbarui!" });
                }

                return Json(new { success = false, message = "Silakan pilih file favicon terlebih dahulu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmail([Bind(Prefix = "EmailSetting")] EmailSetting model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Validasi data gagal. Cek kembali form Anda." });
            }
            _context.EmailSettings.Update(model);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Pengaturan SMTP Email berhasil disimpan!" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerCtaPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "Cta Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerAboutPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "About Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerTestimonialPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "Testimonial Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerNewsPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "News Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerEventPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "Event Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerContactPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "Contact Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerSearchPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "Search Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerPrivacyPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "Privacy Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerVerifySubscriberPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "Verify Subscriber Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerFAQPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "FAQ Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerServicePage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "Service Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerPortfolioPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "Portfolio Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerTeamPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "Team Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerPricingPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "Pricing Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerPhotoGalleryPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "Photo Gallery Page");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBannerTermsPage(PageBannerSetting model)
        {
            return await ProcessBannerUpdate(model, "Terms Page");
        }

        [HttpPost]
        public async Task<IActionResult> SaveSettings([Bind(Prefix = "Theme")] Theme model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Ambil entity data pengaturan dari DB
                    var settings = await _context.Themes.FirstOrDefaultAsync();
                    if (settings == null)
                    {
                        // Antisipasi jika data record pertama belum ada di database sama sekali
                        settings = new Theme();
                        _context.Themes.Add(settings);
                    }

                    // 2. Update nilainya
                    settings.HeaderColor = model.HeaderColor;
                    settings.FooterColor = model.FooterColor;

                    await _context.SaveChangesAsync();

                    // KUNCI PERBAIKAN: Kembalikan objek JSON sukses untuk AJAX
                    return Json(new { success = true, message = "Pengaturan tema berhasil disimpan!" });
                }
                catch (Exception ex)
                {
                    // Kembalikan JSON gagal jika ada kendala database (misal: timeout/koneksi putus)
                    return Json(new { success = false, message = "Gagal menyimpan ke database: " + ex.Message });
                }
            }

            // Jika validasi model state gagal (misalnya input tidak sesuai format)
            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { success = false, message = "Validasi gagal: " + errors });
        }

        // Helper reusable method untuk memproses update banner guna menghindari penulisan kode berulang (DRY)
        private async Task<IActionResult> ProcessBannerUpdate(PageBannerSetting model, string pageName)
        {
            try
            {
                if (model.NewBanner != null && model.NewBanner.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.NewBanner.FileName;
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/banners/");

                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.NewBanner.CopyToAsync(fileStream);
                    }

                    // Ambil entitas lama untuk menghapus banner fisik lama
                    var entity = await _context.PageBanners.FindAsync(model.Id);
                    if (entity != null)
                    {
                        // Hapus file lama (ditambahkan prefix sub-folder banners/)
                        DeleteOldImage("banners/" + entity.BannerPath);

                        entity.BannerPath = uniqueFileName;
                        entity.PageName = pageName;
                        _context.PageBanners.Update(entity);
                    }
                    else
                    {
                        model.BannerPath = uniqueFileName;
                        model.PageName = pageName;
                        _context.PageBanners.Update(model);
                    }

                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = $"Banner {pageName} berhasil diperbarui!" });
                }

                return Json(new { success = false, message = "Silakan pilih file banner terlebih dahulu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // Helper delete oldImage
        private void DeleteOldImage(string? imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                // 1. Bersihkan prefix URL/path agar menyisakan path relatif folder internal wwwroot/uploads
                string cleanPath = imagePath.Replace("/uploads/", "").Replace("uploads/", "");

                // Ambil nama filenya saja untuk pengecekan proteksi default file
                string imageName = Path.GetFileName(cleanPath);

                // 2. Proteksi agar file default bawaan sistem tidak ikut terhapus
                if (imageName.Equals("default-banner.jpg", StringComparison.OrdinalIgnoreCase) ||
                    imageName.Equals("no-image.png", StringComparison.OrdinalIgnoreCase) ||
                    string.IsNullOrEmpty(imageName))
                {
                    return; // Keluar dari fungsi, jangan hapus
                }

                // 3. Gabungkan secara dinamis (mendukung file langsung di /uploads/ atau di sub-folder seperti /uploads/banners/)
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", cleanPath);

                // 4. Eksekusi penghapusan jika file fisik benar-benar ada di server
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }
    }
}
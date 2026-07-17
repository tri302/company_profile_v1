using company_profile.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly INewsRepo _repo;
        private readonly IKategoriRepo _kategori;
        private readonly IWebHostEnvironment _env;
        private readonly IImageCleanupService _imageCleanupService;        
        private const int PageSize = 5;

        public NewsController(INewsRepo repo, IKategoriRepo kategori, IWebHostEnvironment env, IImageCleanupService imageCleanupService)
        {
            _repo = repo;
            _kategori = kategori;
            _env = env;
            _imageCleanupService = imageCleanupService;            
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int pageIndex = 1;            
            var viewModel = await _repo.GetPagedDataAsync(pageIndex, PageSize);
            return View(viewModel);
        }

        //Update Method GetPaginatedData (AJAX) Admin
        [HttpGet]
        public async Task<IActionResult> GetPaginatedData(int pageNumber = 1)
        {            
            if (pageNumber < 1) pageNumber = 1;            
            var viewModel = await _repo.GetPagedDataAsync(pageNumber, PageSize);            
            return PartialView("~/Views/News/_NewsPartial.cshtml", viewModel);
        }
       

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            // Mendapatkan folder root project (satu tingkat di atas wwwroot)
            var rootPath = _env.ContentRootPath;
            var uploadFolder = Path.Combine(rootPath, "uploads", "ckeditor", "images");

            // Pastikan folder ada
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await upload.CopyToAsync(stream);
            }

            return Json(new
            {
                uploaded = 1,
                url = Url.Action("GetImage", "News", new { fileName = fileName })
            });
        }

        [HttpGet]
        public IActionResult GetImage(string fileName)
        {
            var uploadFolder = Path.Combine(_env.ContentRootPath, "uploads", "ckeditor", "images");
            var filePath = Path.Combine(uploadFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            provider.TryGetContentType(filePath, out string contentType);

            return PhysicalFile(filePath, contentType ?? "application/octet-stream");
        }        

        [HttpGet]
        public async Task<IActionResult> Create() // Tambahkan async Task
        {
            // Gunakan await agar yang tersimpan adalah datanya, bukan "Task"-nya
            ViewBag.KategoriList = await _kategori.GetAllKategoriAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SanitizeHtml] // Otomatis membersihkan konten sebelum masuk ke method
        public async Task<IActionResult> Create(NewsVM newsVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _repo.CreateNewsAsync(newsVM);
                    TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                    return RedirectToAction(nameof(Index));
                }

                // Jika gagal/validasi salah, ambil ulang data kategori dengan await
                ViewBag.KategoriList = await _kategori.GetAllKategoriAsync();
                return View(newsVM);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating news: {ex.Message}");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Ambil data dari tabel Kategori untuk dropdown
                ViewBag.KategoriList = await _kategori.GetAllKategoriAsync();
                var data = await _repo.GetNewsByIdAsync(id);
                if (data == null) return NotFound();
                return View(data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading news for edit: {ex.Message}");
                return View("Error");
            }
            finally
            {
                // Optional: Any cleanup code can go here
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SanitizeHtml] // Otomatis membersihkan konten sebelum masuk ke method
        public async Task<IActionResult> Edit(NewsVM newsVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 1. Dapatkan data lama dari database
                    var oldNews = await _repo.GetNewsByIdAsync(newsVM.IDNews);
                    if (oldNews == null) return NotFound();

                    // 2. Bandingkan gambar lama dan baru                    
                    _imageCleanupService.DeleteRemovedImages(oldNews.IsiContent_ID, newsVM.IsiContent_ID);                    
                    _imageCleanupService.DeleteRemovedImages(oldNews.IsiContent_EN, newsVM.IsiContent_EN);

                    //_imageCleanupService.DeleteRemovedImages(oldNews.IsiContent, newsVM.IsiContent);

                    var success = await _repo.UpdateNewsAsync(newsVM);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Data berhasil disimpan!";
                        // Jika berhasil, harusnya masuk ke sini
                        return RedirectToAction(nameof(Index));
                    }

                    // Jika UpdateNewsAsync mengembalikan false (data tidak ditemukan di DB)
                    ModelState.AddModelError("", "Gagal memperbarui data. Data mungkin sudah dihapus.");
                }

                // JIKA GAGAL (Validasi error atau update gagal):
                // Ambil kembali list kategori agar dropdown tidak kosong/error
                ViewBag.KategoriList = await _kategori.GetAllKategoriAsync();

                // Kembalikan ke View "Edit" (default), bukan "Index"
                // Ini supaya user bisa memperbaiki input yang salah
                return View(newsVM);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating news: {ex.Message}");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {            
            if (id == 0) return Json(new { success = false, message = "Data tidak ditemukan" });
            
            var oldNews = await _repo.GetNewsByIdAsync(id);
            if (oldNews == null) return NotFound();

            // Cukup panggil ini untuk menghapus semua gambar
            _imageCleanupService.DeleteAllImagesInContent(oldNews.IsiContent_ID);
            _imageCleanupService.DeleteAllImagesInContent(oldNews.IsiContent_EN);

            try
            {
                await _repo.DeleteNewsAsync(id);
                return Json(new { success = true, message = "News dan gambar terkait berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Terjadi kesalahan saat menghapus news.", ex.Message });
            }
        }

    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using company_profile.PagenationVM;

namespace company_profile.Controllers
{
    [Authorize]
    public class SliderFooterController : Controller
    {
        private readonly ISliderFooterRepo _sliderRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;        
        private const int PageSize = 5;
        public SliderFooterController(ISliderFooterRepo sliderRepo, IWebHostEnvironment webHostEnvironment)
        {
            _sliderRepo = sliderRepo;
            _webHostEnvironment = webHostEnvironment;            
        }

        // Endpoint AJAX untuk memuat tabel parsial
        [HttpGet]
        public async Task<IActionResult> GetPaginatedData(int pageNumber = 1)
        {
            var items = await _sliderRepo.GetPagedDataAsync(pageNumber, PageSize);
            var totalCount = await _sliderRepo.CountAsync();

            var viewModel = new SliderFooterListViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
            };

            // Mengembalikan Partial View agar bisa dirender ke dalam container tabel
            return PartialView("~/Views/SliderFooter/_SliderFooterHomePartial.cshtml", viewModel);
        }

        // GET: WhyChooseUs
        public async Task<IActionResult> Index()
        {
            try
            {
                int initialPage = 1;

                // Cukup ambil data yang diperlukan saja
                var items = await _sliderRepo.GetPagedDataAsync(initialPage, PageSize);
                var totalCount = await _sliderRepo.CountAsync();

                var viewModel = new SliderFooterListViewModel
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
                return View(new SliderFooterListViewModel { Items = new List<SliderFooterHomeVM>() });
            }
        }

        // GET: SliderFooter
        //public async Task<IActionResult> Index()
        //{
        //    var listSlider = await _sliderRepo.GetAllAsync();
        //    return View(listSlider);
        //}

        // GET: SliderFooter/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SliderFooter/Create        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderFooterHomeVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.UploadGambar != null)
                {
                    // PROSES FIX 1: Tambahkan 'await' di sini
                    string uniqueFileName = await ProcessUploadedFileAsync(model.UploadGambar);
                    model.UrlImage = uniqueFileName;

                    bool isSaved = await _sliderRepo.InsertAsync(model);
                    if (isSaved)
                    {
                        TempData["SuccessMessage"] = "Slider baru berhasil ditambahkan!";
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError("", "Gagal menyimpan data ke database.");
                }
                else
                {
                    ModelState.AddModelError("UploadGambar", "File gambar wajib diunggah.");
                }
            }
            return View(model);
        }

        // GET: SliderFooter/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var slider = await _sliderRepo.GetByIdAsync(id);
            if (slider == null)
            {
                return NotFound();
            }
            return View(slider);
        }

        // POST: SliderFooter/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SliderFooterHomeVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (model.UploadGambar != null)
                {
                    if (!string.IsNullOrEmpty(model.UrlImage))
                    {
                        DeleteExistingFile(model.UrlImage);
                    }

                    // PROSES FIX 2: Tambahkan 'await' juga di bagian Edit jika ada
                    string uniqueFileName = await ProcessUploadedFileAsync(model.UploadGambar);
                    model.UrlImage = uniqueFileName;
                }

                bool isUpdated = await _sliderRepo.UpdateAsync(model);
                if (isUpdated)
                {
                    TempData["SuccessMessage"] = "Data slider berhasil diperbarui!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Gagal memperbarui data di database.");
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
                await _sliderRepo.DeleteAsync(id);
                return Json(new { success = true, message = "Data berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        #region Helper Methods (Fungsi Pembantu File)

        private async Task<string> ProcessUploadedFileAsync(IFormFile file)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "sliders");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                // Menggunakan await CopyToAsync agar tidak mengunci (block) thread utama
                await file.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }

        private void DeleteExistingFile(string fileName)
        {
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "sliders", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        #endregion
    }
}
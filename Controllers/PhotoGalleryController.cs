using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{
    [Authorize]
    public class PhotoGalleryController : Controller
    {
        private readonly IPhotoGaleryRepo _repo;
        private const int PageSize = 8;
        public PhotoGalleryController(IPhotoGaleryRepo repo)
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
            return PartialView("~/Views/PhotoGallery/_PhotoGalleryPartial.cshtml", viewModel);
        }        

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhotoGalleryVM photoGalleryVM)
        {
            try
            {                
                if (ModelState.IsValid)
                {                    
                    await _repo.CreatePhotoGaleryAsync(photoGalleryVM);
                    TempData["SuccessMessage"] = "Data berhasil disimpan!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UploadGambar", $"Terjadi kesalahan saat mengunggah foto: {ex.Message}");
            }
            return View(photoGalleryVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _repo.GetPhotoGaleryByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PhotoGalleryVM photoGalleryVM)
        {
            try
            {            
                if (ModelState.IsValid)
                {                    
                    var success = await _repo.UpdatePhotoGaleryAsync(photoGalleryVM);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Data berhasil disimpan!";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UploadGambar", $"Terjadi kesalahan saat mengunggah foto: {ex.Message}");
            }
            return View(photoGalleryVM);
        }

        // Gunakan form post untuk delete demi keamanan
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "Data tidak ditemukan." });
            }
            try
            {
                await _repo.DeletePhotoGaleryAsync(id);
                return Json(new { success = true, message = "Data berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }        
}
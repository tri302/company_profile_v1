using company_profile.PagenationVM;
using company_profile.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{
    [Authorize]
    public class SpecialFeaturesController : Controller
    {
        private readonly ISpecialFeaturesRepo _repo;
        private const int PageSize = 5;

        public SpecialFeaturesController(ISpecialFeaturesRepo repo)
        {
            _repo = repo;            
        }

        // Endpoint AJAX untuk memuat tabel parsial
        [HttpGet]
        public async Task<IActionResult> GetPaginatedData(int pageNumber = 1)
        {
            var items = await _repo.GetPagedDataAsync(pageNumber, PageSize);
            var totalCount = await _repo.CountAsync();

            var viewModel = new SpecialFeaturesListViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
            };

            // Mengembalikan Partial View agar bisa dirender ke dalam container tabel
            return PartialView("~/Views/SpecialFeatures/_SpecialFeaturesPartial.cshtml", viewModel);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                int initialPage = 1;

                // Cukup ambil data yang diperlukan saja
                var items = await _repo.GetPagedDataAsync(initialPage, PageSize);
                var totalCount = await _repo.CountAsync();

                var viewModel = new SpecialFeaturesListViewModel
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
                return View(new SpecialFeaturesListViewModel { Items = new List<SpecialFeaturesVM>() });
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    var data = await _repo.GetAllSpecialFeaturesAsync();
        //    return View(data);
        //}

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialFeaturesVM _specialFeaturesVM)
        {
            if (ModelState.IsValid)
            {
                await _repo.CreateSpecialFeaturesAsync(_specialFeaturesVM);
                TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                return RedirectToAction(nameof(Index));
            }
            return View(_specialFeaturesVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _repo.GetSpecialFeaturesByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SpecialFeaturesVM model)
        {
            if (ModelState.IsValid)
            {
                var success = await _repo.UpdateSpecialFeaturesAsync(model);
                TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                if (success) return RedirectToAction(nameof(Index));
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
                await _repo.DeleteSpecialFeaturesAsync(id);
                return Json(new { success = true, message = "Data berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }        
    }
}
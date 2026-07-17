using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{
    [Authorize]
    public class PricingController : Controller
    {
        private readonly IPricingRepo _repo;
        private const int PageSize = 5;
        public PricingController(IPricingRepo repo)
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

        //Method GetPaginatedData (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetPaginatedData(int pageNumber = 1)
        {
            if (pageNumber < 1) pageNumber = 1;
            var viewModel = await _repo.GetPagedDataAsync(pageNumber, PageSize);
            return PartialView("~/Views/Pricing/_PricingPartial.cshtml", viewModel);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PricingVM pricingVM)
        {
            if (ModelState.IsValid)
            {
                await _repo.CreatePricingAsync(pricingVM);
                TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                return RedirectToAction(nameof(Index));
            }
            return View(pricingVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _repo.GetPricingByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PricingVM pricingVM)
        {
            if (ModelState.IsValid)
            {
                var success = await _repo.UpdatePricingAsync(pricingVM);
                if (success)
                {
                    TempData["SuccessMessage"] = "Data berhasil disimpan!";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(pricingVM);
        }

        [HttpPost] // Gunakan form post untuk delete demi keamanan
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "Data tidak ditemukan." });
            }
            try
            {
                await _repo.DeletePricingAsync(id);
                return Json(new { success = true, message = "Data berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
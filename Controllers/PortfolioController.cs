using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{
    [Authorize]
    public class PortfolioController : Controller
    {
        private readonly IPortfolioRepo _repo;
        private const int PageSize = 8;
        public PortfolioController(IPortfolioRepo repo)
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
            return PartialView("~/Views/Portfolio/_PortfolioPartial.cshtml", viewModel);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PortfolioVM portfolioVM)
        {
            // 1. Parsing manual dari raw input form
            string rawStart = Request.Form["ProjectStartDate"];
            string rawEnd = Request.Form["ProjectEndDate"];

            if (DateTime.TryParseExact(rawStart, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime startDate))
            {
                portfolioVM.ProjectStartDate = startDate;
            }

            if (DateTime.TryParseExact(rawEnd, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime endDate))
            {
                portfolioVM.ProjectEndDate = endDate;
            }

            // 2. Hapus error validasi lama agar ModelState menjadi valid
            ModelState.Remove(nameof(portfolioVM.ProjectStartDate));
            ModelState.Remove(nameof(portfolioVM.ProjectEndDate));

            // 1. Validasi Ukuran File Utama
            if (portfolioVM.UploadGambar != null && portfolioVM.UploadGambar.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("UploadGambar", "Ukuran gambar utama maksimal 5MB.");
            }

            // 2. Validasi Ukuran File Gallery
            if (portfolioVM.UploadFotoProject != null)
            {
                foreach (var file in portfolioVM.UploadFotoProject)
                {
                    if (file.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("UploadFotoProject", $"File {file.FileName} melebihi 5MB.");
                    }
                }
            }

            // 3. Sekarang cek validasi
            if (ModelState.IsValid)
            {
                await _repo.CreatePortfolioAsync(portfolioVM);
                TempData["SuccessMessage"] = "Data berhasil disimpan!";
                return RedirectToAction(nameof(Index));
            }

            // Jika masih tidak valid, kembalikan ke view
            return View(portfolioVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _repo.GetPortfolioForEditAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PortfolioVMEdit portfolioVM)
        {
            // 1. Parsing manual dari raw input form
            string rawStart = Request.Form["ProjectStartDate"];
            string rawEnd = Request.Form["ProjectEndDate"];

            if (DateTime.TryParseExact(rawStart, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime startDate))
            {
                portfolioVM.ProjectStartDate = startDate;
            }

            if (DateTime.TryParseExact(rawEnd, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime endDate))
            {
                portfolioVM.ProjectEndDate = endDate;
            }

            // 2. Hapus error validasi lama agar ModelState menjadi valid
            ModelState.Remove(nameof(portfolioVM.ProjectStartDate));
            ModelState.Remove(nameof(portfolioVM.ProjectEndDate));

            // 1. Validasi Ukuran File Utama
            if (portfolioVM.UploadGambar != null && portfolioVM.UploadGambar.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("UploadGambar", "Ukuran gambar utama maksimal 5MB.");
            }

            // 2. Validasi Ukuran File Gallery
            if (portfolioVM.UploadFotoProject != null)
            {
                foreach (var file in portfolioVM.UploadFotoProject)
                {
                    if (file.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("UploadFotoProject", $"File {file.FileName} melebihi 5MB.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var success = await _repo.UpdatePortfolioAsync(portfolioVM);
                TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                if (success) return RedirectToAction(nameof(Index));
            }
            return View(portfolioVM);
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
                await _repo.DeletePortfolioAsync(id);
                return Json(new { success = true, message = "Data berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
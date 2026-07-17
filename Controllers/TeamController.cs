using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{
    [Authorize]
    public class TeamController : Controller
    {
        private readonly ITeamRepo _repo;
        private const int PageSize = 5;
        public TeamController(ITeamRepo repo)
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
            return PartialView("~/Views/Team/_TeamPartial.cshtml", viewModel);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamVM teamVM)
        {
            if (!ModelState.IsValid) return View(teamVM);            
            await _repo.CreateTeamAsync(teamVM);
            TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _repo.GetTeamByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TeamVM teamVM)
        {            
            if (ModelState.IsValid)
            {
                var success = await _repo.UpdateTeamAsync(teamVM);
                if (success)
                {
                    TempData["SuccessMessage"] = "Data berhasil disimpan!";
                    return RedirectToAction(nameof(Index));
                }

                // Opsional: Jika repository gagal update karena ID tidak ditemukan atau error DB
                ModelState.AddModelError("", "Terjadi kesalahan saat mengupdate data ke database.");
            }

            // Jika sampai sini, berarti ada yang tidak valid
            return View(teamVM);
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
                await _repo.DeleteTeamAsync(id);
                return Json(new { success = true, message = "Data berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
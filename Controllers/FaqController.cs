using company_profile.PagenationVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FaqViewModel = company_profile.FaqVM;

namespace company_profile.Controllers
{
    [Authorize]
    public class FaqController : Controller
    {
        private readonly IFaqRepo _repo;        
        private const int PageSize = 5;
        public FaqController(IFaqRepo repo)
        {
            _repo = repo;            
        }

        // Endpoint AJAX untuk memuat tabel parsial
        [HttpGet]
        public async Task<IActionResult> GetPaginatedData(int pageNumber = 1)
        {
            var items = await _repo.GetPagedDataAsync(pageNumber, PageSize);
            var totalCount = await _repo.CountAsync();

            var viewModel = new FaqListViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
            };

            // Mengembalikan Partial View agar bisa dirender ke dalam container tabel
            return PartialView("~/Views/Faq/_FaqPartial.cshtml", viewModel);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                int initialPage = 1;

                // Cukup ambil data yang diperlukan saja
                var items = await _repo.GetPagedDataAsync(initialPage, PageSize);
                var totalCount = await _repo.CountAsync();

                var viewModel = new FaqListViewModel
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
                return View(new FaqListViewModel { Items = new List<FaqViewModel>() });
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    try
        //    {
        //        var data = await _repo.GetAllFaqAsync();
        //        return View(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception (you can use a logging framework like Serilog or NLog)
        //        System.Diagnostics.Debug.WriteLine($"Error loading events: {ex.Message}");
        //        // Return an error view or handle appropriately
        //        return View("Error");
        //    }
        //    finally
        //    {
        //        // Optional: Any cleanup code can go here
        //    }
        //}

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FaqVM faqVM)
        {
            try
            {
                // 3. Jika validasi helper lolos dan ModelState lainnya valid
                if (ModelState.IsValid)
                {
                    await _repo.CreateFaqAsync(faqVM);
                    TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                    return RedirectToAction(nameof(Index));
                }

                // Jika gagal, kembali ke form dengan pesan error
                return View(faqVM);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating faq: {ex.Message}");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var data = await _repo.GetFaqByIdAsync(id);
                if (data == null) return NotFound();
                return View(data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading faq for edit: {ex.Message}");
                return View("Error");
            }
            finally
            {
                // Optional: Any cleanup code can go here
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FaqVM faqVM)
        {
            try
            {
                // Additional logging can be added here if needed       
                if (ModelState.IsValid)
                {
                    var success = await _repo.UpdateFaqAsync(faqVM);
                    TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                    if (success) return RedirectToAction(nameof(Index));
                }
                return View(faqVM);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating faq: {ex.Message}");
                return View("Error");
            }
            finally
            {
                // Optional: Any cleanup code can go here
            }
        }


        [HttpPost] // Gunakan form post untuk delete demi keamanan
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return Json(new { success = false, message = "Data tidak ditemukan" });
            try
            {
                await _repo.DeleteFaqAsync(id);
                return Json(new { success = true, message = "Faq berhasil dihapus." });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting faq: {ex.Message}");
                // return Json(new { success = false, message = ex.Message });
                return Json(new { success = false, message = "Terjadi kesalahan saat menghapus faq." });
            }
            finally
            {
                // Optional: Any cleanup code can go here
            }
        }
    }
}
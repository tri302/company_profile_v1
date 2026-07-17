using company_profile.PagenationVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventViewModel = company_profile.EventsVM;

namespace company_profile.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly IEventsRepo _repo;        
        private const int PageSize = 5;
        public EventsController(IEventsRepo repo)
        {
            _repo = repo;            
        }

        // Endpoint AJAX untuk memuat tabel parsial
        [HttpGet]
        public async Task<IActionResult> GetPaginatedData(int pageNumber = 1)
        {
            var items = await _repo.GetPagedDataAsync(pageNumber, PageSize);
            var totalCount = await _repo.CountAsync();

            var viewModel = new EventsListViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
            };

            // Mengembalikan Partial View agar bisa dirender ke dalam container tabel
            return PartialView("~/Views/Events/_EventsPartial.cshtml", viewModel);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                int initialPage = 1;

                // Cukup ambil data yang diperlukan saja
                var items = await _repo.GetPagedDataAsync(initialPage, PageSize);
                var totalCount = await _repo.CountAsync();

                var viewModel = new EventsListViewModel
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
                return View(new EventsListViewModel { Items = new List<EventViewModel>() });
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    try
        //    {
        //        var data = await _repo.GetAllEventsAsync();
        //        return View(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Mengembalikan teks mentah berisi pesan error aslinya
        //        return Content($"DEBUG ERROR: {ex.Message} | INNER: {ex.InnerException?.Message} | STACK: {ex.StackTrace}");
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
        public async Task<IActionResult> Create(EventsVM eventsVM)
        {
            // Cek ukuran file secara manual di controller
            if (eventsVM.UploadGambar != null && eventsVM.UploadGambar.Length > 5 * 1024 * 1024) // 5MB
            {
                ModelState.AddModelError("UploadGambar", "Ukuran file tidak boleh lebih dari 5MB.");
            }

            // 1. Parsing manual dari raw input form
            string rawStart = Request.Form["StartEvent"];
            string rawEnd = Request.Form["EndEvent"];

            if (DateTime.TryParseExact(rawStart, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime startDate))
            {
                eventsVM.StartEvent = startDate;
            }

            if (DateTime.TryParseExact(rawEnd, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime endDate))
            {
                eventsVM.EndEvent = endDate;
            }

            // 2. Hapus error validasi lama agar ModelState menjadi valid
            ModelState.Remove(nameof(eventsVM.StartEvent));
            ModelState.Remove(nameof(eventsVM.EndEvent));

            if (ModelState.IsValid)
            {
                await _repo.CreateEventsAsync(eventsVM);
                TempData["SuccessMessage"] = "Data berhasil disimpan!";
                return RedirectToAction(nameof(Index));
            }

            return View(eventsVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var data = await _repo.GetEventsByIdAsync(id);
                if (data == null) return NotFound();
                return View(data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading event for edit: {ex.Message}");
                return View("Error");
            }
            finally
            {
                // Optional: Any cleanup code can go here
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventsVM eventsVM)
        {
            // Cek ukuran file secara manual di controller
            if (eventsVM.UploadGambar != null && eventsVM.UploadGambar.Length > 5 * 1024 * 1024) // 5MB
            {
                ModelState.AddModelError("UploadGambar", "Ukuran file tidak boleh lebih dari 5MB.");
            }

            // 1. Parsing manual dari raw input form
            string rawStart = Request.Form["StartEvent"];
            string rawEnd = Request.Form["EndEvent"];

            if (DateTime.TryParseExact(rawStart, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime startDate))
            {
                eventsVM.StartEvent = startDate;
            }

            if (DateTime.TryParseExact(rawEnd, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime endDate))
            {
                eventsVM.EndEvent = endDate;
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _repo.UpdateEventsAsync(eventsVM);
                    TempData["SuccessMessage"] = "Data berhasil disimpan!"; // Pesan sukses
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating event: {ex.Message}");
                return View("Error");
            }

            return View(eventsVM);
        }


        [HttpPost] // Gunakan form post untuk delete demi keamanan
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return Json(new { success = false, message = "Data tidak ditemukan" });
            try
            {
                await _repo.DeleteEventsAsync(id);
                return Json(new { success = true, message = "Event berhasil dihapus." });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting event: {ex.Message}");
                // return Json(new { success = false, message = ex.Message });
                return Json(new { success = false, message = "Terjadi kesalahan saat menghapus event." });
            }
            finally
            {
                // Optional: Any cleanup code can go here
            }
        }
    }
}
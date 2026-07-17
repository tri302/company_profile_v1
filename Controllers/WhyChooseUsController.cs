using Microsoft.AspNetCore.Mvc;
using company_profile.ViewModels;
using Microsoft.EntityFrameworkCore;
using company_profile.Helper;
using Microsoft.AspNetCore.Authorization;
using company_profile.PagenationVM;

namespace company_profile.Controllers
{
    [Authorize]
    public class WhyChooseUsController : Controller
    {
        private readonly IWhyChooseUsRepo _whyChooseUsRepo;
        private readonly AppDbContext _context;        
        private const int PageSize = 5;
        public WhyChooseUsController(IWhyChooseUsRepo whyChooseUsRepo, AppDbContext context)
        {
            _whyChooseUsRepo = whyChooseUsRepo;
            _context = context;            
        }

        // Endpoint AJAX untuk memuat tabel parsial
        [HttpGet]
        public async Task<IActionResult> GetPaginatedData(int pageNumber = 1)
        {
            var items = await _whyChooseUsRepo.GetPagedDataAsync(pageNumber, PageSize);
            var totalCount = await _whyChooseUsRepo.CountAsync();

            var viewModel = new WhyChooseUsListViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
            };
            
            // Mengembalikan Partial View agar bisa dirender ke dalam container tabel
            return PartialView("~/Views/WhyChooseUs/_WhyChooseUsTablePartial.cshtml", viewModel);
        }

        // GET: WhyChooseUs
        public async Task<IActionResult> Index()
        {
            try
            {
                int initialPage = 1;

                // Cukup ambil data yang diperlukan saja
                var items = await _whyChooseUsRepo.GetPagedDataAsync(initialPage, PageSize);
                var totalCount = await _whyChooseUsRepo.CountAsync();

                var viewModel = new WhyChooseUsListViewModel
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
                return View(new WhyChooseUsListViewModel { Items = new List<WhyChooseUsVM>() });
            }
        }

        // GET: WhyChooseUs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WhyChooseUs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SanitizeHtml] // Otomatis membersihkan konten sebelum masuk ke method
        public async Task<IActionResult> Create(WhyChooseUsVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await _whyChooseUsRepo.AddAsync(model);
                if (await _whyChooseUsRepo.SaveChangesAsync())
                {
                    TempData["SuccessMessage"] = "Data berhasil di simpan !";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Gagal menyimpan data ke database.");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Terjadi kesalahan sistem saat menyimpan data: " + ex.Message;
            }
            return View(model);

        }
        // GET: WhyChooseUs/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var data = await _whyChooseUsRepo.GetByIdAsync(id);
                if (data == null)
                {
                    return NotFound();
                }
                return View(data);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Terjadi kesalahan saat mengambil data yang akan diubah.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: WhyChooseUs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SanitizeHtml] // Otomatis membersihkan konten sebelum masuk ke method
        public async Task<IActionResult> Edit(int id, WhyChooseUsVM model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _whyChooseUsRepo.UpdateAsync(model);
                if (await _whyChooseUsRepo.SaveChangesAsync())
                {
                    TempData["SuccessMessage"] = "Data berhasil diperbarui!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Gagal memperbarui data ke database.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Terjadi kesalahan sistem saat memperbarui data: " + ex.Message);
            }

            return View(model);
        }

        // GET/POST: WhyChooseUs/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var data = await _whyChooseUsRepo.GetByIdAsync(id);
                if (data == null)
                {
                    TempData["ErrorMessage"] = "Data tidak ditemukan atau sudah dihapus.";
                    return RedirectToAction(nameof(Index));
                }
                _whyChooseUsRepo.DeleteAsync(id);
                if (await _whyChooseUsRepo.SaveChangesAsync())
                {
                    TempData["SuccessMessage"] = "Data keunggulan berhasil dihapus!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Gagal menghapus data.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Terjadi kesalahan sistem saat menghapus data: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
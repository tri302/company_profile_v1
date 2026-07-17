using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
namespace company_profile.Controllers;

public class CompanyController : Controller
{
    private readonly IAboutRepo _repo;
    private readonly ITeamRepo _ITeamRepo;
    private readonly IContactRepo _IContactRepo;
    private readonly IEventsRepo _IEventsRepo;
    private readonly IFaqRepo _IFaqRepo;
    private readonly INewsRepo _INewsRepo;
    private readonly IOurServicesRepo _IOurServicesRepo;
    private readonly IPhotoGaleryRepo _IPhotoGaleryRepo;
    private readonly IPortfolioRepo _IPortfolioRepo;
    private readonly IPricingRepo _IPricingRepo;
    private readonly ITestimonialRepo _ITestimonialRepo;
    private readonly IIndexHome _repoIndexHome;
    private readonly AppDbContext _context;
    private const int PageSize = 5;
    private const int PageSizePortfolio = 6;
    private const int PageSizeService = 6;
    private const int PageSizePhotoGallery = 6;
    private const int PageSizePricing = 3;
    private const int PageSizeTeam = 8;
    public CompanyController
    (
    IAboutRepo repo,
    ITeamRepo teamRepo,
    IContactRepo contactRepo,
    IEventsRepo eventsRepo,
    IFaqRepo faqRepo,
    INewsRepo newsRepo,
    IOurServicesRepo ourServicesRepo,
    IPhotoGaleryRepo photoGaleryRepo,
    IPortfolioRepo portfolioRepo,
    IPricingRepo pricingRepo,
    ITestimonialRepo testimonialRepo,
    IIndexHome repoIndexHome,
    AppDbContext context
    )
    {
        _repo = repo;
        _ITeamRepo = teamRepo;
        _IContactRepo = contactRepo;
        _IEventsRepo = eventsRepo;
        _IFaqRepo = faqRepo;
        _INewsRepo = newsRepo;
        _IOurServicesRepo = ourServicesRepo;
        _IPhotoGaleryRepo = photoGaleryRepo;
        _IPortfolioRepo = portfolioRepo;
        _IPricingRepo = pricingRepo;
        _ITestimonialRepo = testimonialRepo;
        _repoIndexHome = repoIndexHome;
        _context = context;
    }

    public async Task<IActionResult> Search(string search_string, string category, int pageNumber = 1)
    {
        string currentLang = CultureInfo.CurrentCulture.Name;
        bool isId = currentLang.StartsWith("id");
        int pageSize = 5; // Tentukan jumlah berita per halaman

        // 1. Ambil data berita dan kategori secara lengkap terlebih dahulu
        var allNews = await _context.News
            .Include(n => n.Kategori)
            .OrderByDescending(n => n.CreateAt)
            .ToListAsync();

        var kategoriList = await _context.Kategoris.ToListAsync();

        // 2. Filter data
        var filteredNews = allNews.AsEnumerable();

        if (!string.IsNullOrEmpty(search_string))
        {
            var terms = search_string.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            filteredNews = filteredNews.Where(n =>
            {
                string judul = (isId ? (n.Judul_ID ?? "") : (n.Judul_EN ?? "")).ToLower();
                return terms.All(t => judul.Contains(t));
            });
        }

        if (!string.IsNullOrEmpty(category))
        {
            filteredNews = filteredNews.Where(n => n.Kategori != null &&
                (isId ? n.Kategori.NamaKategori_ID == category : n.Kategori.NamaKategori_EN == category));
        }

        var results = filteredNews.ToList();

        // 3. Ranking Relevansi
        if (!string.IsNullOrEmpty(search_string))
        {
            string search = search_string.ToLower().Trim();
            results = results.OrderByDescending(n =>
            {
                string judul = (isId ? (n.Judul_ID ?? "") : (n.Judul_EN ?? "")).ToLower();
                if (judul == search) return 100;
                if (judul.Contains(search)) return 50;
                return 0;
            }).ThenByDescending(n => n.CreateAt).ToList();
        }

        // Hitung total halaman berdasarkan hasil filter pencarian
        int totalRecords = results.Count;
        int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        if (pageNumber < 1) pageNumber = 1;
        if (pageNumber > totalPages && totalPages > 0) pageNumber = totalPages;

        // 4. Fallback jika kosong total
        if (!results.Any())
        {
            results = allNews.Take(5).ToList();
            ViewBag.Message = "Pencarian tidak ditemukan, menampilkan berita terbaru.";
            totalPages = 1;
            pageNumber = 1;
        }
        else
        {
            // Potong data hasil pencarian sesuai halaman aktif
            results = results.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        // 5. Mapping menggunakan helper method bawaan Anda
        var viewModel = MapToNewsVM(results, kategoriList, pageNumber, totalPages);

        // Simpan parameter pencarian ke ViewBag agar AJAX tahu kata kunci apa yang sedang aktif
        ViewBag.SearchString = search_string;
        ViewBag.Category = category;

        var mappedKategori = viewModel.ListKategoriBerita;
        ViewBag.CategoryList = new SelectList(mappedKategori, "NamaKategori", "NamaKategori", category);

        // Jika dipanggil via AJAX, return partial view saja
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_NewsPartialFE", viewModel);
        }

        return View("News", viewModel);
    }

    public async Task<IActionResult> GetPaginatedDataFE(string search_string, string category, int pageNumber = 1)
    {
        // Cukup panggil ulang fungsi Search karena fungsi di atas sudah mendeteksi request AJAX
        return await Search(search_string, category, pageNumber);
    }

    [HttpGet("Contact")] // Sebaiknya beri nama route agar jelas, misal: /Contact
    public async Task<IActionResult> GetAllContactAsync()
    {
        try
        {
            var data = await _IContactRepo.GetAllContactAsync();
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }
        catch (Exception e)
        {
            // Gunakan Console.WriteLine atau Logging untuk debug
            Console.WriteLine($"Error: {e.Message}");
            // Kembalikan view kosong atau halaman error khusus
            return View(new ContactVM { ListContact = new List<ContactVM>() });
        }
    }

    // Update Method GetPaginatedData (AJAX) NewsByCategory
    [HttpGet]
    public async Task<IActionResult> GetPaginatedDataFENewsByCategory(string namaKategori, int pageIndex = 1)
    {
        // Cegah null reference jika parameter tidak dikirim dari front-end
        if (string.IsNullOrEmpty(namaKategori))
        {
            namaKategori = ""; // Atau handle dengan return BadRequest() / Empty Partial View
        }
        if (pageIndex < 1) pageIndex = 1;

        var viewModel = await _INewsRepo.GetPagedDataFENewsByCategory(namaKategori, pageIndex, PageSize);

        return PartialView("~/Views/Company/_NewsByCategoryPartial.cshtml", viewModel);
    }

    [HttpGet("Company/NewsByCategory/{NamaKategori}")]
    public async Task<IActionResult> NewsByCategory(string NamaKategori, int pageIndex = 1)
    {
        try
        {
            if (pageIndex < 1) pageIndex = 1; // Validasi agar tidak minus atau 0

            // Masukkan pageIndex yang dinamis, bukan angka 1 hardcoded
            var data = await _INewsRepo.GetPagedDataFENewsByCategory(NamaKategori, pageIndex, PageSize);

            if (data == null)
            {
                return NotFound("Data berita tidak ditemukan.");
            }
            return View(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error di halaman News: {ex.Message}");
            TempData["ErrorMessage"] = "Maaf, terjadi kesalahan saat memuat berita terbaru.";
            return RedirectToAction("Index", "Home");
        }
    }

    // Agar parameter asp-route-id cocok dengan parameter di controller
    // pakai Attribute Routing
    [HttpGet("Company/NewsDetail/{IDNews}")]
    public async Task<IActionResult> NewsDetail(int IDNews)
    {
        try
        {
            // 1. Validasi awal: ID tidak boleh nol atau negatif
            if (IDNews <= 0)
            {
                return BadRequest("ID Berita tidak valid.");
            }

            // 2. Panggil repository
            var data = await _INewsRepo.GetDetailNewsByIdAsync(IDNews);

            // 3. Jika data tidak ditemukan di database
            if (data == null)
            {
                return NotFound();
            }

            // 4. Kirim data ke View
            return View(data);
        }
        catch (Exception ex)
        {
            // 5. Logging error untuk keperluan debugging
            // Jika sudah ada ILogger, gunakan _logger.LogError(ex, "Error detail news");
            Console.WriteLine($"Terjadi kesalahan pada NewsDetail: {ex.Message}");

            // Berikan respon yang user-friendly
            return StatusCode(500, "Terjadi kesalahan internal pada server.");
        }
    }

    [HttpGet("Company/PortfolioDetail/{id}")] // Menambahkan route agar URL lebih rapi
    public async Task<IActionResult> PortfolioDetail(int id)
    {
        try
        {
            var captcha = CaptchaHelper.Generate();
            // Simpan jawaban di Session
            HttpContext.Session.SetInt32("CaptchaResult", captcha.Result);
            ViewBag.CaptchaQuestion = captcha.Question;

            // 1. Validasi awal ID
            if (id <= 0)
            {
                return BadRequest("ID Portfolio tidak valid.");
            }

            // 2. Ambil data dari repository
            var portfolioDetail = await _IPortfolioRepo.GetPortfolioByIdAsync(id);

            // 3. Cek jika data tidak ditemukan
            if (portfolioDetail == null)
            {
                return NotFound("Portfolio tidak ditemukan.");
            }

            // 4. Kirim data ke View
            return View(portfolioDetail);
        }
        catch (Exception ex)
        {
            // 5. Log error untuk kebutuhan debug
            Console.WriteLine($"Error di PortfolioDetail: {ex.Message}");

            // Opsional: Kirim pesan error ke View menggunakan TempData
            TempData["ErrorMessage"] = "Maaf, terjadi kesalahan saat mengambil detail portfolio.";

            // Kembalikan ke halaman daftar portfolio jika error
            return RedirectToAction("Index");
        }
    }

    [HttpGet("Company/ServiceDetail/{id}")] // Tambahkan route agar URL konsisten
    public async Task<IActionResult> ServiceDetail(int id)
    {
        try
        {
            var captcha = CaptchaHelper.Generate();
            // Simpan jawaban di Session
            HttpContext.Session.SetInt32("CaptchaResult", captcha.Result);
            ViewBag.CaptchaQuestion = captcha.Question;
            // 1. Validasi awal: Pastikan ID valid
            if (id <= 0)
            {
                return BadRequest("ID Service tidak valid.");
            }

            // 2. Ambil data dari repository
            var serviceDetail = await _IOurServicesRepo.GetOurServicesByIdAsync(id);
            // 3. Jika data tidak ada di database
            if (serviceDetail == null)
            {
                return NotFound("Service tidak ditemukan.");
            }
            // 4. Sukses, tampilkan ke View
            return View(serviceDetail);
        }
        catch (Exception ex)
        {
            // 5. Log error untuk kamu (developer) cek di console
            Console.WriteLine($"Error di ServiceDetail: {ex.Message}");
            // Berikan feedback ke user
            TempData["ErrorMessage"] = "Gagal memuat detail layanan. Silakan coba lagi nanti.";
            // Balikin ke halaman daftar service (Index) kalau error
            return RedirectToAction("Index");
        }
    }

    [HttpGet("Company/EventDetail/{id}")]
    public async Task<IActionResult> EventDetail(int id)
    {
        try
        {
            // 1. Validasi awal: ID harus lebih besar dari 0
            if (id <= 0)
            {
                return BadRequest("ID Event tidak valid.");
            }

            // 2. Ambil data dari repository
            var eventDetail = await _IEventsRepo.GetEventsByIdAsync(id);

            // 3. Jika data tidak ditemukan di database
            if (eventDetail == null)
            {
                return NotFound("Event tidak ditemukan.");
            }

            // 4. Kirim data ke View
            return View(eventDetail);
        }
        catch (Exception ex)
        {
            // 5. Log error untuk kamu cek saat development
            Console.WriteLine($"Error di EventDetail: {ex.Message}");

            // Berikan pesan error yang ramah ke user
            TempData["ErrorMessage"] = "Maaf, terjadi kesalahan saat memuat detail event.";

            // Kembalikan ke halaman daftar event jika error
            return RedirectToAction("Index");
        }
    }

    [HttpGet("Company/TeamDetail/{idTeam}")] // Mengatur Route agar URL lebih informatif
    public async Task<IActionResult> DetailTeamMember(int idTeam)
    {
        try
        {
            // 1. Validasi awal: ID harus valid (di atas 0)
            if (idTeam <= 0)
            {
                return BadRequest("ID Anggota Tim tidak valid.");
            }

            // 2. Ambil data dari repository
            var teamMember = await _ITeamRepo.GetTeamByIdAsync(idTeam);

            // 3. Jika data tidak ditemukan di database
            if (teamMember == null)
            {
                return NotFound("Anggota tim tidak ditemukan.");
            }

            // 4. Sukses, tampilkan ke View
            return View(teamMember);
        }
        catch (Exception ex)
        {
            // 5. Log error untuk kamu (developer) pantau di console
            Console.WriteLine($"Error di DetailTeamMember: {ex.Message}");

            // Berikan pesan ramah ke user
            TempData["ErrorMessage"] = "Gagal memuat profil anggota tim.";

            // Kembalikan ke halaman daftar team (Index)
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Home()
    {
        var captcha = CaptchaHelper.Generate();
        // Simpan jawaban di Session
        HttpContext.Session.SetInt32("CaptchaResult", captcha.Result);
        ViewBag.CaptchaQuestion = captcha.Question;

        var indexHomeVM = await _repoIndexHome.GetAllAsync();
        return View(indexHomeVM);
    }

    [HttpGet("Company/About")]
    public async Task<IActionResult> About()
    {
        try
        {
            // 1. Ambil data tunggal (single row) untuk halaman About
            var data = await _repo.GetAboutSingleAsync();

            // 2. Jika data tidak ada di database
            if (data == null)
            {
                // Bisa return NotFound atau redirect ke Home jika data About wajib ada
                //return NotFound("Informasi About Us belum tersedia.");
                return View(null);
            }

            // 3. Sukses, kirim data ke View
            return View(data);
        }
        catch (Exception ex)
        {
            // 4. Log error untuk kebutuhan debugging
            Console.WriteLine($"Error di halaman About: {ex.Message}");

            // Berikan respon atau alihkan jika terjadi kegagalan teknis
            TempData["ErrorMessage"] = "Maaf, terjadi kesalahan teknis saat memuat profil perusahaan.";

            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet("Company/Team")]
    public async Task<IActionResult> Team()
    {
        try
        {
            // 1. Ambil data cukup satu kali saja dan simpan di variabel
            //var data = await _ITeamRepo.GetAllTeamAsync();
            int pageIndex = 1;
            var viewModel = await _ITeamRepo.GetPagedDataAsync(pageIndex, PageSizeTeam);

            // 2. Cek apakah data null atau kosong
            if (viewModel == null)
            {
                return NotFound("Data tim tidak ditemukan.");
            }

            // 3. Kirim variabel 'data' ke View
            return View(viewModel);
        }
        catch (Exception ex)
        {
            // 4. Log error untuk debugging
            Console.WriteLine($"Error di halaman Team: {ex.Message}");

            // Berikan pesan ramah dan redirect
            TempData["ErrorMessage"] = "Gagal memuat daftar tim kami.";
            return RedirectToAction("Index", "Home");
        }
    }
    //Method GetPaginatedData (AJAX) Team
    [HttpGet]
    public async Task<IActionResult> GetPaginatedDataTeam(int pageNumber = 1)
    {
        if (pageNumber < 1) pageNumber = 1;
        var viewModel = await _ITeamRepo.GetPagedDataAsync(pageNumber, PageSizeTeam);
        return PartialView("~/Views/Company/_TeamPartial.cshtml", viewModel);
    }
    [HttpGet("Company/Event")]
    public async Task<IActionResult> Event()
    {
        try
        {
            // 1. Ambil data dari repository            
            int initialPage = 1;

            // Cukup ambil data yang diperlukan saja
            var items = await _IEventsRepo.GetPagedDataAsync(initialPage, PageSize);
            var totalCount = await _IEventsRepo.CountAsync();

            var viewModel = new EventsVM
            {
                ListEvents = items,
                CurrentPage = initialPage,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
            };

            // 2. Cek jika data null
            if (viewModel == null)
            {
                return NotFound("Daftar event tidak ditemukan.");
            }

            // 3. Kirim data ke View
            return View(viewModel);
        }
        catch (Exception ex)
        {
            // 4. Log error untuk kebutuhan debugging
            Console.WriteLine($"Error di halaman Event: {ex.Message}");

            // Memberikan feedback jika terjadi kegagalan sistem
            TempData["ErrorMessage"] = "Terjadi kesalahan saat memuat daftar event.";

            // Alihkan ke Home agar user tidak terjebak di halaman error
            return RedirectToAction("Index", "Home");
        }
    }
    // Endpoint AJAX untuk memuat tabel parsial Event
    [HttpGet]
    public async Task<IActionResult> GetPaginatedDataEvent(int pageNumber = 1)
    {
        var items = await _IEventsRepo.GetPagedDataAsync(pageNumber, PageSize);
        var totalCount = await _IEventsRepo.CountAsync();

        var viewModel = new EventsVM
        {
            ListEvents = items,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
        };

        // Mengembalikan Partial View agar bisa dirender ke dalam container tabel
        return PartialView("~/Views/Company/_EventPartial.cshtml", viewModel);
    }
    [HttpGet("Company/PhotoGallery")]
    public async Task<IActionResult> PhotoGalery()
    {
        try
        {
            // 1. Ambil data galeri foto dari repository            
            int pageIndex = 1;
            var data = await _IPhotoGaleryRepo.GetPagedDataAsync(pageIndex, PageSizePhotoGallery);

            // 2. Cek jika data null
            if (data == null)
            {
                return NotFound("Galeri foto tidak ditemukan.");
            }

            // 3. Sukses, kirim data ke View
            return View(data);
        }
        catch (Exception ex)
        {
            // 4. Log error untuk debugging di console
            Console.WriteLine($"Error di PhotoGalery: {ex.Message}");

            // Berikan pesan error yang user-friendly
            TempData["ErrorMessage"] = "Gagal memuat galeri foto.";

            // Alihkan kembali ke halaman utama jika terjadi error sistem
            return RedirectToAction("Index", "Home");
        }
    }
    //Update Method GetPaginatedData (AJAX) PhotoGalery
    [HttpGet]
    public async Task<IActionResult> GetPaginatedDataPhotoGallery(int pageNumber = 1)
    {
        if (pageNumber < 1) pageNumber = 1;
        var viewModel = await _IPhotoGaleryRepo.GetPagedDataAsync(pageNumber, PageSizePhotoGallery);
        return PartialView("~/Views/Company/_PhotoGaleryPartial.cshtml", viewModel);
    }
    [HttpGet("Company/Testimonial")]
    public async Task<IActionResult> Testimonial()
    {
        try
        {
            // 1. Ambil semua data testimoni dari repository            
            int pageIndex = 1;
            var data = await _ITestimonialRepo.GetPagedDataAsync(pageIndex, PageSizePhotoGallery);

            // 2. Cek jika data yang dikembalikan null
            if (data == null)
            {
                return NotFound("Data testimoni tidak ditemukan.");
            }

            // 3. Sukses, kirim data ke View
            return View(data);
        }
        catch (Exception ex)
        {
            // 4. Log error untuk kebutuhan development
            Console.WriteLine($"Error di halaman Testimonial: {ex.Message}");

            // Berikan notifikasi error ke user
            TempData["ErrorMessage"] = "Gagal memuat data testimoni pelanggan.";

            // Alihkan ke Home agar flow user tidak terputus
            return RedirectToAction("Index", "Home");
        }
    }
    //Update Method GetPaginatedData (AJAX) Testimonial
    [HttpGet]
    public async Task<IActionResult> GetPaginatedDataTestimonial(int pageNumber = 1)
    {
        if (pageNumber < 1) pageNumber = 1;
        var viewModel = await _ITestimonialRepo.GetPagedDataAsync(pageNumber, PageSizePhotoGallery);
        return PartialView("~/Views/Company/_TestimonialPartial.cshtml", viewModel);
    }
    [HttpGet("Company/Faq")]
    public async Task<IActionResult> Faq()
    {
        try
        {
            // 1. Ambil semua data FAQ dari repository            
            int initialPage = 1;

            // Cukup ambil data yang diperlukan saja
            var data = await _IFaqRepo.GetPagedDataAsync(initialPage, PageSize);
            var totalCount = await _IFaqRepo.CountAsync();

            var viewModel = new FaqVM
            {
                ListFaqs = data,
                CurrentPage = initialPage,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
            };

            // 2. Cek jika data yang dikembalikan null
            if (data == null)
            {
                return NotFound("Data FAQ tidak ditemukan.");
            }

            // 3. Kirim data ke View
            return View(viewModel);
        }
        catch (Exception ex)
        {
            // 4. Log error ke console untuk debugging
            Console.WriteLine($"Error di halaman FAQ: {ex.Message}");

            // Berikan notifikasi error yang user-friendly
            TempData["ErrorMessage"] = "Maaf, terjadi kesalahan saat memuat daftar pertanyaan.";

            // Alihkan ke Home jika terjadi error teknis
            return RedirectToAction("Index", "Home");
        }
    }
    // Endpoint AJAX untuk memuat tabel parsial Faq
    [HttpGet]
    public async Task<IActionResult> GetPaginatedDataFaq(int pageNumber = 1)
    {
        var items = await _IFaqRepo.GetPagedDataAsync(pageNumber, PageSize);
        var totalCount = await _IFaqRepo.CountAsync();

        var viewModel = new FaqVM
        {
            ListFaqs = items,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
        };

        // Mengembalikan Partial View agar bisa dirender ke dalam container tabel
        return PartialView("~/Views/Company/_FaqPartial.cshtml", viewModel);
    }
    [HttpGet("Company/Service")]
    public async Task<IActionResult> Service()
    {
        try
        {
            // 1. Ambil semua data layanan dari repository
            //var data = await _IOurServicesRepo.GetAllOurServicesAsync();
            int pageIndex = 1;
            var data = await _IOurServicesRepo.GetPagedDataAsync(pageIndex, PageSizeService);

            // 2. Validasi jika data tidak ditemukan (null)
            if (data == null)
            {
                return NotFound("Daftar layanan tidak ditemukan.");
            }

            // 3. Sukses, kirim data ke View
            return View(data);
        }
        catch (Exception ex)
        {
            // 4. Log error ke console agar bisa kamu cek saat development
            Console.WriteLine($"Error di halaman Service: {ex.Message}");

            // Berikan pesan error yang sopan ke pengunjung
            TempData["ErrorMessage"] = "Maaf, terjadi masalah saat memuat daftar layanan kami.";

            // Alihkan ke Home agar user tidak melihat halaman error sistem
            return RedirectToAction("Index", "Home");
        }
    }
    //Update Method GetPaginatedData (AJAX) Service
    [HttpGet]
    public async Task<IActionResult> GetPaginatedDataService(int pageNumber = 1)
    {
        if (pageNumber < 1) pageNumber = 1;
        var viewModel = await _IOurServicesRepo.GetPagedDataAsync(pageNumber, PageSizeService);
        return PartialView("~/Views/Company/_ServicePartial.cshtml", viewModel);
    }
    [HttpGet("Company/Pricing")]
    public async Task<IActionResult> Pricing()
    {
        try
        {
            // 1. Ambil semua data paket harga dari repository            
            int pageIndex = 1;
            var viewModel = await _IPricingRepo.GetPagedDataAsync(pageIndex, PageSizePricing);

            // 2. Cek jika data yang dikembalikan null
            if (viewModel == null)
            {
                return NotFound("Data paket harga tidak ditemukan.");
            }

            // 3. Sukses, kirim data ke View
            return View(viewModel);
        }
        catch (Exception ex)
        {
            // 4. Log error untuk kamu cek saat development
            Console.WriteLine($"Error di halaman Pricing: {ex.Message}");

            // Berikan pesan ramah kepada calon pelanggan
            TempData["ErrorMessage"] = "Maaf, sistem kami sedang mengalami kendala saat memuat daftar harga.";

            // Alihkan ke Home agar flow user tetap terjaga
            return RedirectToAction("Index", "Home");
        }
    }
    //Update Method GetPaginatedData (AJAX) Pricing
    [HttpGet]
    public async Task<IActionResult> GetPaginatedDataPricing(int pageNumber = 1)
    {
        if (pageNumber < 1) pageNumber = 1;
        var viewModel = await _IPricingRepo.GetPagedDataAsync(pageNumber, PageSizePricing);
        return PartialView("~/Views/Company/_PricingPartial.cshtml", viewModel);
    }
    [HttpGet("Company/Portfolio")]
    public async Task<IActionResult> Portfolio()
    {
        try
        {
            // 1. Ambil semua data portfolio dari repository            
            int pageIndex = 1;
            var data = await _IPortfolioRepo.GetPagedDataAsync(pageIndex, PageSizePortfolio);


            // 2. Cek jika data null
            if (data == null)
            {
                return NotFound("Daftar portfolio tidak ditemukan.");
            }

            // 3. Sukses, kirim data ke View
            return View(data);
        }
        catch (Exception ex)
        {
            // 4. Log error untuk kamu cek saat development
            Console.WriteLine($"Error di halaman Portfolio: {ex.Message}");

            // Berikan notifikasi error yang ramah
            TempData["ErrorMessage"] = "Maaf, terjadi kendala saat memuat daftar proyek kami.";

            // Alihkan ke Home agar flow user tetap aman
            return RedirectToAction("Index", "Home");
        }
    }
    //Method GetPaginatedData (AJAX) Portfolio
    [HttpGet]
    public async Task<IActionResult> GetPaginatedDataPortfolio(int pageNumber = 1)
    {
        if (pageNumber < 1) pageNumber = 1;
        var viewModel = await _IPortfolioRepo.GetPagedDataAsync(pageNumber, PageSizePortfolio);
        return PartialView("~/Views/Company/_PortfolioPartial.cshtml", viewModel);
    }
    // Update Method GetPaginatedData (AJAX) News
    //[HttpGet]
    //public IActionResult GetPaginatedDataFE(int pageNumber = 1)
    //{
    //    if (pageNumber < 1) pageNumber = 1;

    //    // Panggil method sinkron (.GetPagedDataFE)
    //    var viewModel = _INewsRepo.GetPagedDataFE(pageNumber, PageSize);

    //    // Kembalikan PartialView saja tanpa Layout
    //    return PartialView("~/Views/Company/_NewsPartialFE.cshtml", viewModel);
    //}

    [HttpGet("Company/News")]
    public IActionResult News()
    {
        try
        {
            int pageIndex = 1;

            // Panggil data paged secara sinkron untuk load pertama kali
            var data = _INewsRepo.GetPagedDataFE(pageIndex, PageSize);

            if (data == null)
            {
                return NotFound("Data berita tidak ditemukan.");
            }
            return View(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error di halaman News: {ex.Message}");
            TempData["ErrorMessage"] = "Maaf, terjadi kesalahan saat memuat berita terbaru.";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet("Company/Contact")]
    public async Task<IActionResult> Contact()
    {
        try
        {
            var captcha = CaptchaHelper.Generate();
            // Simpan jawaban di Session
            HttpContext.Session.SetInt32("CaptchaResult", captcha.Result);
            ViewBag.CaptchaQuestion = captcha.Question;

            // 1. Ambil data kontak dari repository
            var data = await _IContactRepo.GetAllContactIndexAsync();

            // 2. Cek jika data null
            if (data == null)
            {
                return NotFound("Informasi kontak tidak ditemukan.");
            }

            // 3. Sukses, kirim data ke View
            return View(data);
        }
        catch (Exception ex)
        {
            // 4. Log error untuk kamu cek saat development
            Console.WriteLine($"Error di halaman Contact: {ex.Message}");

            // Berikan notifikasi error yang ramah
            TempData["ErrorMessage"] = "Maaf, kami sedang mengalami kendala teknis saat memuat informasi kontak.";

            // Alihkan ke Home agar user tidak terjebak di halaman error
            return RedirectToAction("Index", "Home");
        }
    }
    private NewsVM MapToNewsVM(List<News> newsList, List<Kategori> kategoriList, int pageIndex = 1, int totalPages = 1)
    {
        string currentLang = CultureInfo.CurrentCulture.Name;

        var mappedNews = newsList.Select(n => new NewsVM
        {
            IDNews = n.IDNews,
            UrlGambar = n.UrlGambar,
            Judul = currentLang.StartsWith("id") ? n.Judul_ID : n.Judul_EN,
            IsiContent = currentLang.StartsWith("id") ? n.IsiContent_ID : n.IsiContent_EN,
            Judul_ID = n.Judul_ID,
            Judul_EN = n.Judul_EN,
            IsiContent_ID = n.IsiContent_ID,
            IsiContent_EN = n.IsiContent_EN,
            CreateAt = n.CreateAt,
            Kategori = n.Kategori != null ? new Kategori
            {
                IDKategori = n.Kategori.IDKategori,
                NamaKategori = currentLang.StartsWith("id") ? n.Kategori.NamaKategori_ID : n.Kategori.NamaKategori_EN
            } : null
        }).ToList();

        var mappedKategori = kategoriList.Select(k => new KategoriVM
        {
            IDKategori = k.IDKategori,
            NamaKategori_ID = k.NamaKategori_ID,
            NamaKategori_EN = k.NamaKategori_EN,
            NamaKategori = currentLang.StartsWith("id") ? k.NamaKategori_ID : k.NamaKategori_EN
        }).ToList();

        // Ambil 5 berita terbaru dari database untuk Recent Posts
        var recentNewsRaw = _context.News.OrderByDescending(n => n.CreateAt).Take(5).ToList();
        var mappedRecent = recentNewsRaw.Select(n => new NewsVM
        {
            IDNews = n.IDNews,
            UrlGambar = n.UrlGambar,
            Judul = currentLang.StartsWith("id") ? n.Judul_ID : n.Judul_EN,
            CreateAt = n.CreateAt
        }).ToList();

        return new NewsVM
        {
            ListBerita = mappedNews,
            ListKategoriBerita = mappedKategori,
            ListRecentPostsBerita = mappedRecent,
            CurrentPage = pageIndex,
            TotalPages = totalPages
        };
    }

}

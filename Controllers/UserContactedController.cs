using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{    
    public class UserContactedController : Controller
    {
        private readonly IUserContactedRepo _repo;
        private const int PageSize = 5;
        public UserContactedController(IUserContactedRepo repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult VerifyCaptcha(int input)
        {
            int? sessionResult = HttpContext.Session.GetInt32("CaptchaResult");
            if (sessionResult.HasValue && input == sessionResult.Value)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Jawaban Anda salah, coba lagi." });
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
            return PartialView("~/Views/UserContacted/_UserContactedPartial.cshtml", viewModel);
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactVM mainVM, int userCaptcha)
        {
            // Hapus manual aturan validasi data info perusahaan agar ModelState kembali bersih/True
            ModelState.Remove("Alamat");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("EmailAddress");
            ModelState.Remove("Alamat_ID");
            ModelState.Remove("Alamat_EN");
            
            // Ambil object form bersarangnya
            var contactVM = mainVM.ContactForm;
            int? actualResult = HttpContext.Session.GetInt32("CaptchaResult");
            if (actualResult == null || userCaptcha != actualResult)
            {
                var newCaptcha = CaptchaHelper.Generate();
                HttpContext.Session.SetInt32("CaptchaResult", newCaptcha.Result);
                return Json(new
                {
                    success = false,
                    errors = new { userCaptcha = "Captcha salah!" },
                    newCaptchaQuestion = newCaptcha.Question
                });
            }

            if (!ModelState.IsValid)
            {
                // Ambil list error validasi
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                );

                var newCaptcha = CaptchaHelper.Generate();
                HttpContext.Session.SetInt32("CaptchaResult", newCaptcha.Result);
                return Json(new { success = false, errors = errors, newCaptchaQuestion = newCaptcha.Question });
            }

            // Simpan ke database menggunakan objek yang sudah diekstrak
            await _repo.CreateContactAsync(contactVM);

            var freshCaptcha = CaptchaHelper.Generate();
            HttpContext.Session.SetInt32("CaptchaResult", freshCaptcha.Result);

            return Json(new
            {
                success = true,
                message = "Pesan berhasil dikirim..",
                newCaptchaQuestion = freshCaptcha.Question
            });
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
                await _repo.DeleteContactAsync(id);
                return Json(new { success = true, message = "Data berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            // 1. Cek jika id tidak valid
            if (id <= 0)
            {
                return NotFound(); // Lebih baik return 404 untuk halaman Detail
            }

            try
            {
                // 2. TAMPUNG HASILNYA ke dalam variabel 'data'
                var data = await _repo.GetContactByIdAsync(id);

                // 3. Cek jika data tidak ditemukan di database
                if (data == null)
                {
                    return NotFound();
                }

                // 4. KIRIM 'data' ke dalam View
                return View(data);
            }
            catch (Exception ex)
            {
                // Jika terjadi error koneksi database dll
                return StatusCode(500, "Terjadi kesalahan internal: " + ex.Message);
            }
        }

    }
}
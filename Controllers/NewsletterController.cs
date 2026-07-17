using company_profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyProfile.Controllers
{
    [Authorize]
    public class NewsletterController : Controller
    {
        private readonly INewsletterRepo _Repo;

        public NewsletterController(INewsletterRepo newsletterRepo)
        {
            _Repo = newsletterRepo;
        }


        // Jalur POST: Saat user klik tombol kirim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(NewsletterVM model)
        {
            // Validasi server-side
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Format email tidak valid." });
            }

            try
            {                
                await _Repo.AddNewsletterAsync(model);
                return Ok(new { success = true, message = "Terima kasih! Anda berhasil berlangganan." });
            }
            catch (Exception ex)
            {
                // Log error jika diperlukan
                return StatusCode(500, new { success = false, message = "Terjadi kesalahan pada server. " + ex.Message });
            }
        }
    }
}
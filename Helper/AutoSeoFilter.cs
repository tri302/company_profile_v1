
using company_profile; // Sesuaikan dengan namespace model kamu
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

public class AutoSeoFilter : IActionFilter
{
    private readonly AppDbContext _context; // Ganti ApplicationDbContext dengan nama DbContext kamu

    // 1. Inject DbContext melalui Constructor
    public AutoSeoFilter(AppDbContext context)
    {
        _context = context;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Berjalan SEBELUM action dieksekusi
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Berjalan SESUDAH action dieksekusi
        if (context.Result is ViewResult viewResult)
        {
            // Cek apakah action controller sudah mengisi ViewData["SEO"] secara spesifik
            // (Misal: halaman artikel blog mungkin punya SEO-nya sendiri)
            if (!viewResult.ViewData.ContainsKey("SEO"))
            {
                // 2. Ambil data SEO global dari database (berdasarkan gambar, kita ambil Id = 1)
                var globalSeo = _context.SeoMetadata.FirstOrDefault(s => s.Id == 1);

                if (globalSeo != null)
                {
                    // 3. Masukkan data dari DB ke ViewData
                    viewResult.ViewData["SEO"] = globalSeo;
                }
                else
                {
                    // Fallback (cadangan) JIKA tabel di database terhapus/kosong
                    viewResult.ViewData["SEO"] = new SeoMetadata
                    {
                        MetaTitle = "Default Title",
                        MetaDescription = "Default Description"
                    };
                }
            }
        }
    }
}
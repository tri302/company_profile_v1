using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace company_profile
{
    public class FaviconViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public FaviconViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Ambil semua data dari tabel logosettings
            var allSettings = await _db.LogoSettings.ToListAsync();

            // Cari yang Tipenya 'Favicon' dan 'Logo'
            var faviconData = allSettings.FirstOrDefault(x => x.Tipe == "Favicon");            

            var viewModel = new SiteHeaderViewModel
            {
                FaviconUrl = faviconData?.UrlGambar                
            };

            return View(viewModel);
        }
    }
}
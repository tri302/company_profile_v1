using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace company_profile
{
    public class LogoViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public LogoViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Ambil semua data dari tabel logosettings
            var allSettings = await _db.LogoSettings.ToListAsync();

            // Cari yang Tipenya 'Favicon' dan 'Logo'            
            var logoData = allSettings.FirstOrDefault(x => x.Tipe == "Logo");

            var viewModel = new SiteHeaderViewModel
            {                
                LogoUrl = logoData?.UrlGambar
            };

            return View(viewModel);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Pastikan untuk import ini agar bisa pakai Async
using System.Threading.Tasks;

namespace company_profile.Components
{
    public class BannerViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public BannerViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string pageName)
        {
            // Mencari banner yang PageName-nya cocok dengan parameter yang dikirim
            var model = await _context.PageBanners
                .FirstOrDefaultAsync(x => x.PageName == pageName);

            // Jika data tidak ditemukan, kamu bisa handle atau biarkan model bernilai null
            return View(model);
        }
    }
}
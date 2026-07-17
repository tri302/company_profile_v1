using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using company_profile; 

namespace company_profile.ViewComponents
{
    public class RecentPostsViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        // Inject DbContext Anda di sini
        public RecentPostsViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int numberToShow = 5)
        {
            string currentLang = CultureInfo.CurrentCulture.Name;

            // 1. Ambil data dari database
            var posts = await _context.News
                .OrderByDescending(n => n.CreateAt)
                .Take(numberToShow)
                .ToListAsync();

            // 2. Mapping ke ViewModel (PENTING: Gunakan .ToList() agar data terbuat)
            var recentPostsVM = posts.Select(k => new NewsVM
            {
                IDNews = k.IDNews, // Pastikan ID ini ada di ViewModel Anda
                Judul = currentLang.StartsWith("id") ? k.Judul_ID : k.Judul_EN,
                UrlGambar = k.UrlGambar // Pastikan property ini ada
            }).ToList();

            // 3. Kirim ke View
            return View(recentPostsVM);
        }
    }
}
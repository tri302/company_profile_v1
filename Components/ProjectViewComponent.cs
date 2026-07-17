using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using company_profile;

namespace company_profile.ViewComponents
{
    public class ProjectViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        // Inject DbContext Anda di sini
        public ProjectViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int numberToShow = 5)
        {
            //Fungsi filter multi bahasa
            string currentLang = CultureInfo.CurrentCulture.Name;

            var recentProjects = await _context.Portfolios
                .Include(p => p.PortfolioGambars)
                .OrderByDescending(p => p.IDPortfolio)
                .Take(numberToShow)
                .ToListAsync();
            var model = recentProjects.Select(p => new PortfolioVM
            {
                IDPortfolio = p.IDPortfolio,
                UrlGambar = p.UrlGambar,

                Judul = currentLang.StartsWith("id") ? p.Judul_ID : p.Judul_EN,
                IsiContentPortfolio = currentLang.StartsWith("id") ? p.IsiContentPortfolio_ID : p.IsiContentPortfolio_EN,
                ClientComment = currentLang.StartsWith("id") ? p.ClientComment_ID : p.ClientComment_EN,
            });
            return View(model);
        }
    }
}
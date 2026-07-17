using company_profile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace YourProjectNamespace.Components
{
    public class ThemeViewComponent : ViewComponent
    {
          private readonly AppDbContext _context;

        public ThemeViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Ambil data warna dari database
            var theme = await _context.Themes.FirstOrDefaultAsync();
            // Jika di database masih kosong, berikan warna default
            if (theme == null)
            {
                theme = new Theme 
                { 
                    HeaderColor = "0948b6", 
                    FooterColor = "0948b6" 
                };
            }
            // Pastikan format warna memiliki tanda '#' agar valid saat dibaca CSS
            if (!theme.HeaderColor.StartsWith("#")) theme.HeaderColor = "#" + theme.HeaderColor;
            if (!theme.FooterColor.StartsWith("#")) theme.FooterColor = "#" + theme.FooterColor;

            return View(theme);
        }
    }
}
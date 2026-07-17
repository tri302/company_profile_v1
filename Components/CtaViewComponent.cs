using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace company_profile.Components
{
    public class CtaViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public CtaViewComponent(AppDbContext context)
        {            
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _context.PageBanners.Where(b => b.PageName == "Cta Page").FirstOrDefaultAsync();
            return View(model);
        }
    }
}
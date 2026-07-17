using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Pastikan untuk import ini agar bisa pakai Async
using System.Threading.Tasks;

namespace company_profile.Components
{
    public class AddressViewComponent : ViewComponent
    {
        private readonly IContactRepo _context;

        public AddressViewComponent(IContactRepo context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _context.GetFirstContactAsync();
            return View(model);
        }
    }
}
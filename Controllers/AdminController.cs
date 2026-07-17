using company_profile.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace company_profile.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }
    public async Task<IActionResult> Dashboard()
    {
        // Menggunakan await dan CountAsync() agar benar-benar asynchronous
        var _TotalNewsByCategories = await _db.Kategoris.CountAsync();
        var _TotalNews = await _db.News.CountAsync();
        var _TotalEvents = await _db.Events.CountAsync();
        var _TotalTeamMembers = await _db.Teams.CountAsync();
        var _TotalClients = await _db.Portfolios.CountAsync();
        var _TotalServices = await _db.OurServices.CountAsync();
        var _TotalTestimonials = await _db.Testimonials.CountAsync();
        var _TotalPhotosGallery = await _db.PhotoGalleries.CountAsync();
        var _TotalPricingTables = await _db.Pricings.CountAsync();

        var data = new DashboardVM
        {
            TotalNewsByCategories = _TotalNewsByCategories,
            TotalNews = _TotalNews,
            TotalEvents = _TotalEvents,
            TotalTeamMembers = _TotalTeamMembers,
            TotalClients = _TotalClients,
            TotalServices = _TotalServices,
            TotalTestimonials = _TotalTestimonials,
            TotalPhotosGallery = _TotalPhotosGallery,
            TotalPricingTables = _TotalPricingTables
        };

        return View(data);
    }
}

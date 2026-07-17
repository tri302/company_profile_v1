using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace company_profile.Components
{

    public class CategoryListViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        public CategoryListViewComponent(AppDbContext context) => _context = context;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string currentLang = CultureInfo.CurrentCulture.Name;
            var categories = await _context.Kategoris.ToListAsync();
            // Lakukan mapping multi-bahasa di sini
            var mappedKategori = categories.Select(k => new KategoriVM
            {
                IDKategori = k.IDKategori,
                NamaKategori_ID = k.NamaKategori_ID,
                NamaKategori_EN = k.NamaKategori_EN,

                // FILTER MULTI-BAHASA UNTUK NAMA KATEGORI
                NamaKategori = currentLang.StartsWith("id") ? k.NamaKategori_ID : k.NamaKategori_EN
            }).ToList();
            return View(mappedKategori);
        }
    }
}


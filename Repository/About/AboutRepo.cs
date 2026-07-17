using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace company_profile
{
    public class AboutRepo : IAboutRepo
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public AboutRepo(AppDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public async Task<List<AboutVM>> GetAllAboutAsync()
        {
            var data = await _context.Abouts.ToListAsync();
            //Fungsi filter multi bahasa
            string currentLang = CultureInfo.CurrentCulture.Name;
            return data.Select(a => new AboutVM
            {
                IDAbout = a.IDAbout,
                IsiContent = currentLang.StartsWith("id") ? a.IsiContent_ID : a.IsiContent_EN,
                IsiContent_ID = a.IsiContent_ID,
                IsiContent_EN = a.IsiContent_EN,
                CreateAt = a.CreateAt,
            }).ToList();
        }

        public async Task<AboutVM?> GetAboutSingleAsync()
        {
            var a = await _context.Abouts.FirstOrDefaultAsync(); 
            string currentLang = CultureInfo.CurrentCulture.Name;
            if (a == null) return null;

            var aboutVM = new AboutVM
            {
                IDAbout = a.IDAbout,
                // IsiContent_ID = a.IsiContent_ID,
                // IsiContent_EN = a.IsiContent_EN,
                // 2. FILTER MULTI-BAHASA DI SINI
                // Jika currentLang diawali "en" (misal: "en-US"), pakai _EN. 
                // Jika bukan (misal: "id-ID"), pakai _ID.
                IsiContent = currentLang.StartsWith("id") ? a.IsiContent_ID : a.IsiContent_EN,
                CreateAt = a.CreateAt,
            };
            return aboutVM;
        }

        public async Task<AboutVM?> GetAboutByIdAsync(int idAbout)
        {
            var data = await _context.Abouts.FindAsync(idAbout);
            string currentLang = CultureInfo.CurrentCulture.Name;
            if (data == null) return null;

            return new AboutVM
            {
                IDAbout = data.IDAbout,
                IsiContent = currentLang.StartsWith("id") ? data.IsiContent_ID : data.IsiContent_EN,
                IsiContent_ID = data.IsiContent_ID,
                IsiContent_EN = data.IsiContent_EN,
                CreateAt = data.CreateAt,
            };
        }

        public async Task<AboutVM> CreateAboutAsync(AboutVM aboutVM)
        {
            var model = new About
            {
                IsiContent_ID = aboutVM.IsiContent_ID,
                IsiContent_EN = aboutVM.IsiContent_EN,
                CreateAt = DateTime.Now
            };

            _context.Abouts.Add(model);
            await _context.SaveChangesAsync();

            aboutVM.IDAbout = model.IDAbout;
            return aboutVM;
        }

        public async Task<bool> UpdateAboutAsync(AboutVM aboutVM)
        {
            var data = await _context.Abouts.FindAsync(aboutVM.IDAbout);
            if (data == null) return false;

            // Mapping property data baru (ID & EN)
            data.IsiContent_ID = aboutVM.IsiContent_ID;
            data.IsiContent_EN = aboutVM.IsiContent_EN;

            _context.Abouts.Update(data);
            return await _context.SaveChangesAsync() > 0;
        }

        // Delete tetap sama karena tidak butuh mapping property
        public async Task<bool> DeleteAboutAsync(int idAbout)
        {
            var data = await _context.Abouts.FindAsync(idAbout);
            if (data == null) return false;

            _context.Abouts.Remove(data);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
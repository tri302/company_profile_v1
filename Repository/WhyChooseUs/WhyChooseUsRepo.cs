using System.Globalization;
using company_profile.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace company_profile
{
    public class WhyChooseUsRepo : IWhyChooseUsRepo
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public WhyChooseUsRepo(AppDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }
        
        public async Task<WhyChooseUsVM?> GetByIdAsync(int id)
        {
            var w = await _context.WhyChooseUs.FindAsync(id);
            if (w == null) return null;
            string currentLang = CultureInfo.CurrentCulture.Name;
            return new WhyChooseUsVM
            {
                Id = w.Id,

                JudulWhyChooseUs = currentLang.StartsWith("id") ? w.JudulWhyChooseUs_ID : w.JudulWhyChooseUs_EN,
                IsiContentWhyChooseUs = currentLang.StartsWith("id") ? w.IsiContentWhyChooseUs_ID : w.IsiContentWhyChooseUs_EN,

                JudulWhyChooseUs_ID = w.JudulWhyChooseUs_ID,
                JudulWhyChooseUs_EN = w.JudulWhyChooseUs_EN,
                IsiContentWhyChooseUs_ID = w.IsiContentWhyChooseUs_ID,
                IsiContentWhyChooseUs_EN = w.IsiContentWhyChooseUs_EN,
                UrlGambar = w.UrlGambar
            };
        }

        public async Task AddAsync(WhyChooseUsVM vm)
        {
            var entity = new WhyChooseUs
            {
                JudulWhyChooseUs_ID = vm.JudulWhyChooseUs_ID,
                JudulWhyChooseUs_EN = vm.JudulWhyChooseUs_EN,
                IsiContentWhyChooseUs_ID = vm.IsiContentWhyChooseUs_ID,
                IsiContentWhyChooseUs_EN = vm.IsiContentWhyChooseUs_EN,
                UrlGambar = vm.UploadGambar != null ? await SaveImage(vm.UploadGambar) : ""
            };
            await _context.WhyChooseUs.AddAsync(entity);            
        }

        public async Task UpdateAsync(WhyChooseUsVM vm)
        {
            var entity = await _context.WhyChooseUs.FindAsync(vm.Id);
            if (entity != null)
            {
                entity.JudulWhyChooseUs_ID = vm.JudulWhyChooseUs_ID;
                entity.JudulWhyChooseUs_EN = vm.JudulWhyChooseUs_EN;
                entity.IsiContentWhyChooseUs_ID = vm.IsiContentWhyChooseUs_ID;
                entity.IsiContentWhyChooseUs_EN = vm.IsiContentWhyChooseUs_EN;

                if (vm.UploadGambar != null)
                {
                    DeleteOldImage(entity.UrlGambar);
                    entity.UrlGambar = await SaveImage(vm.UploadGambar);
                }
                _context.WhyChooseUs.Update(entity);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.WhyChooseUs.FindAsync(id);
            if (entity != null)
            {
                DeleteOldImage(entity.UrlGambar);
                _context.WhyChooseUs.Remove(entity);
            }
        }

        public async Task<bool> SaveChangesAsync() => (await _context.SaveChangesAsync()) > 0;

        // --- Helper Methods (Sesuaikan dengan path folder Anda) ---
        private async Task<string> SaveImage(IFormFile file)
        {
            string folder = Path.Combine(_webHost.WebRootPath, "uploads", "whychooseus");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            using (var fileStream = new FileStream(Path.Combine(folder, uniqueFileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return "/uploads/whychooseus/" + uniqueFileName;
        }

        private void DeleteOldImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            string fullPath = Path.Combine(_webHost.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        public async Task<IEnumerable<WhyChooseUsVM>> GetAllAsync()
        {
            return await GetQuery().ToListAsync();
        }        

        public async Task<IEnumerable<WhyChooseUsVM>> GetPagedDataAsync(int pageIndex, int pageSize)
        {
            return await GetQuery()
                .OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.WhyChooseUs.CountAsync();
        }

        private IQueryable<WhyChooseUsVM> GetQuery()
        {
            string currentLang = CultureInfo.CurrentCulture.Name;

            return _context.WhyChooseUs.Select(w => new WhyChooseUsVM
            {
                Id = w.Id,

                JudulWhyChooseUs = currentLang.StartsWith("id")
                    ? w.JudulWhyChooseUs_ID
                    : w.JudulWhyChooseUs_EN,

                IsiContentWhyChooseUs = currentLang.StartsWith("id")
                    ? w.IsiContentWhyChooseUs_ID
                    : w.IsiContentWhyChooseUs_EN,

                JudulWhyChooseUs_ID = w.JudulWhyChooseUs_ID,
                JudulWhyChooseUs_EN = w.JudulWhyChooseUs_EN,
                IsiContentWhyChooseUs_ID = w.IsiContentWhyChooseUs_ID,
                IsiContentWhyChooseUs_EN = w.IsiContentWhyChooseUs_EN,
                UrlGambar = w.UrlGambar
            });
        }

    }
}
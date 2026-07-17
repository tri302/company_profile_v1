using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using company_profile.ViewModels;
using company_profile.Models;

namespace company_profile
{
    public class BannerHomeRepo : IBannerHomeRepo
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BannerHomeRepo(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IEnumerable<BannerHomeVM>> GetAllAsync()
        {
            string currentLang = CultureInfo.CurrentCulture.Name;
            return await _context.BannerHome
                .Select(b => new BannerHomeVM
                {
                    Id = b.Id,

                    //Filter multi bahasa
                    JudulTextAnimatedOnBanner = currentLang.StartsWith("id") ? b.JudulTextAnimatedOnBanner_ID : b.JudulTextAnimatedOnBanner_EN,
                    IsiTextAnimatedOnBanner = currentLang.StartsWith("id") ? b.IsiTextAnimatedOnBanner_ID : b.IsiTextAnimatedOnBanner_EN,

                    // Pemetaan Kolom Multi-Bahasa Baru
                    JudulTextAnimatedOnBanner_ID = b.JudulTextAnimatedOnBanner_ID,
                    JudulTextAnimatedOnBanner_EN = b.JudulTextAnimatedOnBanner_EN,
                    IsiTextAnimatedOnBanner_ID = b.IsiTextAnimatedOnBanner_ID,
                    IsiTextAnimatedOnBanner_EN = b.IsiTextAnimatedOnBanner_EN,
                    UrlGambar = b.UrlGambar
                })
                .Take(3)
                .ToListAsync();
        }

        public async Task<BannerHomeVM?> GetByIdAsync(int id)
        {
            var banner = await _context.BannerHome.FindAsync(id);
            if (banner == null) return null;
            string currentLang = CultureInfo.CurrentCulture.Name;
            return new BannerHomeVM
            {
                Id = banner.Id,
                //Filter multi bahasa
                JudulTextAnimatedOnBanner = currentLang.StartsWith("id") ? banner.JudulTextAnimatedOnBanner_ID : banner.JudulTextAnimatedOnBanner_EN,
                IsiTextAnimatedOnBanner = currentLang.StartsWith("id") ? banner.IsiTextAnimatedOnBanner_ID : banner.IsiTextAnimatedOnBanner_EN,
                // Pemetaan Kolom Multi-Bahasa Baru
                JudulTextAnimatedOnBanner_ID = banner.JudulTextAnimatedOnBanner_ID,
                JudulTextAnimatedOnBanner_EN = banner.JudulTextAnimatedOnBanner_EN,
                IsiTextAnimatedOnBanner_ID = banner.IsiTextAnimatedOnBanner_ID,
                IsiTextAnimatedOnBanner_EN = banner.IsiTextAnimatedOnBanner_EN,
                UrlGambar = banner.UrlGambar
            };
        }

        public async Task InsertAsync(BannerHomeVM bannerVM)
        {
            var entity = new BannerHome
            {
                // Pemetaan Kolom Multi-Bahasa Baru dari VM ke Entity
                JudulTextAnimatedOnBanner_ID = bannerVM.JudulTextAnimatedOnBanner_ID,
                JudulTextAnimatedOnBanner_EN = bannerVM.JudulTextAnimatedOnBanner_EN,
                IsiTextAnimatedOnBanner_ID = bannerVM.IsiTextAnimatedOnBanner_ID,
                IsiTextAnimatedOnBanner_EN = bannerVM.IsiTextAnimatedOnBanner_EN,
                UrlGambar = bannerVM.UrlGambar
            };
            await _context.BannerHome.AddAsync(entity);
        }

        public async Task UpdateAsync(BannerHomeVM bannerVM)
        {
            var entity = await _context.BannerHome.FindAsync(bannerVM.Id);

            if (entity != null)
            {
                // Update properti entity dengan data multi-bahasa baru dari ViewModel
                entity.JudulTextAnimatedOnBanner_ID = bannerVM.JudulTextAnimatedOnBanner_ID;
                entity.JudulTextAnimatedOnBanner_EN = bannerVM.JudulTextAnimatedOnBanner_EN;
                entity.IsiTextAnimatedOnBanner_ID = bannerVM.IsiTextAnimatedOnBanner_ID;
                entity.IsiTextAnimatedOnBanner_EN = bannerVM.IsiTextAnimatedOnBanner_EN;
                entity.UrlGambar = bannerVM.UrlGambar; 

                _context.BannerHome.Update(entity);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var banner = await _context.BannerHome.FindAsync(id);
            if (banner != null)
            {
                if (!string.IsNullOrEmpty(banner.UrlGambar))
                {
                    DeleteOldImage(banner.UrlGambar); 
                }
                _context.BannerHome.Remove(banner);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File tidak boleh kosong.");
            }

            const long maxFileSize = 2 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                throw new BadImageFormatException("Ukuran berkas maksimal adalah 2MB.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            {
                throw new BadImageFormatException("Ekstensi file tidak valid. Hanya diperbolehkan .jpg, .jpeg, dan .png.");
            }

            var uploadDir = Path.Combine(_environment.WebRootPath, "uploads", "banners");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadDir, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/banners/{uniqueFileName}";
        }

        public void DeleteOldImage(string? imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                var filePath = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('/'));

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        public async Task<IEnumerable<SliderFooterHomeVM>> GetAllSliderFooterHomeAsync()
        {
            return await _context.SliderFooterHomes
                .Select(s => new SliderFooterHomeVM
                {
                    Id = s.Id,
                    UrlImage = s.UrlImage
                })
                .ToListAsync();            
        }
    }
}
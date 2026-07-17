using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace company_profile
{
    public class TestimonialRepo : ITestimonialRepo
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public TestimonialRepo(AppDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public async Task<TestimonialVM> CreateTestimonialAsync(TestimonialVM testimonialVM)
        {
            var testimonial = new Testimonial
            {
                Nama = testimonialVM.Nama,
                // Mapping Multi-Bahasa
                Jabatan_ID = testimonialVM.Jabatan_ID,
                Jabatan_EN = testimonialVM.Jabatan_EN,
                IsiTestimoni_ID = testimonialVM.IsiTestimoni_ID,
                IsiTestimoni_EN = testimonialVM.IsiTestimoni_EN,
                CreateAt = DateTime.Now
            };

            if (testimonialVM.UploadGambar != null)
            {
                testimonial.UrlFoto = await SaveImage(testimonialVM.UploadGambar);
            }

            _context.Testimonials.Add(testimonial);
            await _context.SaveChangesAsync();

            testimonialVM.IDTestimonial = testimonial.IDTestimonial;
            testimonialVM.UrlGambar = testimonial.UrlFoto;
            return testimonialVM;
        }

        public async Task<bool> DeleteTestimonialAsync(int idTestimonial)
        {
            var testimonial = await _context.Testimonials.FindAsync(idTestimonial);
            if (testimonial == null) return false;

            DeleteOldImage(testimonial.UrlFoto);
            _context.Testimonials.Remove(testimonial);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TestimonialVM>> GetAllTestimonialsAsync()
        {
           return await GetQuery().ToListAsync();
        }

        public async Task<TestimonialVM> GetTestimonialByIdAsync(int idTestimonial)
        {
            var t = await _context.Testimonials.FindAsync(idTestimonial);
            if (t == null) return null;
            string currentLang = CultureInfo.CurrentCulture.Name;
            return new TestimonialVM
            {
                IDTestimonial = t.IDTestimonial,
                Nama = t.Nama,
                UrlGambar = t.UrlFoto,

                Jabatan = currentLang.StartsWith("id") ? t.Jabatan_ID : t.Jabatan_EN,
                IsiTestimoni = currentLang.StartsWith("id") ? t.IsiTestimoni_ID : t.IsiTestimoni_EN,

                Jabatan_ID = t.Jabatan_ID,
                Jabatan_EN = t.Jabatan_EN,
                IsiTestimoni_ID = t.IsiTestimoni_ID,
                IsiTestimoni_EN = t.IsiTestimoni_EN,
                CreateAt = t.CreateAt
            };
        }

        public async Task<bool> UpdateTestimonialAsync(TestimonialVM testimonialVM)
        {
            var testimonial = await _context.Testimonials.FindAsync(testimonialVM.IDTestimonial);
            if (testimonial == null) return false;

            testimonial.Nama = testimonialVM.Nama;
            testimonial.Jabatan_ID = testimonialVM.Jabatan_ID;
            testimonial.Jabatan_EN = testimonialVM.Jabatan_EN;
            testimonial.IsiTestimoni_ID = testimonialVM.IsiTestimoni_ID;
            testimonial.IsiTestimoni_EN = testimonialVM.IsiTestimoni_EN;

            if (testimonialVM.UploadGambar != null)
            {
                DeleteOldImage(testimonial.UrlFoto);
                testimonial.UrlFoto = await SaveImage(testimonialVM.UploadGambar);
            }

            _context.Testimonials.Update(testimonial);
            return await _context.SaveChangesAsync() > 0;
        }

        // --- Helper Methods ---

        private async Task<string> SaveImage(IFormFile file)
        {
            string folder = Path.Combine(_webHost.WebRootPath, "uploads", "testimonial");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(folder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/uploads/testimonial/" + uniqueFileName;
        }

        private void DeleteOldImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            string fullPath = Path.Combine(_webHost.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        public async Task<TestimonialVM> GetPagedDataAsync(int pageIndex, int pageSize)
        {
            var totalRecords = await CountAsync();
            var data = await GetQuery()
                .OrderByDescending(x => x.IDTestimonial)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new TestimonialVM
            {
                List = data,
                CurrentPage = pageIndex,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };
        }

        public async Task<int> CountAsync()
        {
            return await _context.Testimonials.CountAsync();
        }

        private IQueryable<TestimonialVM> GetQuery()
        {
            var testimonials = _context.Testimonials;
            //Fungsi filter multi bahasa
            string currentLang = CultureInfo.CurrentCulture.Name;
            return testimonials.Select(t => new TestimonialVM
            {
                IDTestimonial = t.IDTestimonial,
                Nama = t.Nama,
                UrlGambar = t.UrlFoto,

                Jabatan = currentLang.StartsWith("id") ? t.Jabatan_ID : t.Jabatan_EN,
                IsiTestimoni = currentLang.StartsWith("id") ? t.IsiTestimoni_ID : t.IsiTestimoni_EN,

                Jabatan_ID = t.Jabatan_ID,
                Jabatan_EN = t.Jabatan_EN,
                IsiTestimoni_ID = t.IsiTestimoni_ID,
                IsiTestimoni_EN = t.IsiTestimoni_EN,
                CreateAt = t.CreateAt
            });
        }
    }
}
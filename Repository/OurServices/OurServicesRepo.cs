using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace company_profile
{
    public class OurServicesRepo : IOurServicesRepo
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public OurServicesRepo(AppDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public async Task<OurServicesVM> CreateOurServicesAsync(OurServicesVM ourServicesVM)
        {
            var model = new OurServices
            {
                // Mapping Properti Multi-Bahasa
                JudulService_ID = ourServicesVM.JudulService_ID,
                JudulService_EN = ourServicesVM.JudulService_EN,
                IsiService_ID = ourServicesVM.IsiService_ID,
                IsiService_EN = ourServicesVM.IsiService_EN,

                UrlGambar = ourServicesVM.UrlGambar,
                CreateAt = DateTime.Now
            };

            if (ourServicesVM.UploadGambar != null)
            {
                model.UrlGambar = await SaveImage(ourServicesVM.UploadGambar);
            }

            _context.OurServices.Add(model);
            await _context.SaveChangesAsync();

            // Kembalikan ID yang baru digenerate ke VM
            ourServicesVM.IDOurServices = model.IDOurServices;
            return ourServicesVM;
        }

        public async Task<bool> DeleteOurServicesAsync(int idOurServices)
        {
            var ourServices = await _context.OurServices.FindAsync(idOurServices);
            if (ourServices == null) return false;

            // Hapus file fisik agar tidak memenuhi storage
            DeleteOldImage(ourServices.UrlGambar);

            _context.OurServices.Remove(ourServices);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OurServicesVM>> GetAllOurServicesAsync()
        {
           return await GetQuery().ToListAsync();
        }
        public async Task<IEnumerable<OurServicesVM>> GetAllOurServicesAsyncTake()
        {
            return await GetQuery().Take(6).ToListAsync();
        }
        public async Task<OurServicesVM> GetOurServicesByIdAsync(int idOurServices)
        {
            var os = await _context.OurServices.FindAsync(idOurServices);

            if (os == null) return null;
            string currentLang = CultureInfo.CurrentCulture.Name;
            return new OurServicesVM
            {
                IDOurServices = os.IDOurServices,

                JudulService = currentLang.StartsWith("id") ? os.JudulService_ID : os.JudulService_EN,
                IsiService = currentLang.StartsWith("id") ? os.IsiService_ID : os.IsiService_EN,

                // Mapping eksplisit untuk Form Edit
                JudulService_ID = os.JudulService_ID,
                JudulService_EN = os.JudulService_EN,
                IsiService_ID = os.IsiService_ID,
                IsiService_EN = os.IsiService_EN,
                UrlGambar = os.UrlGambar,
                CreateAt = os.CreateAt,

                // Jika butuh list layanan lain (misal untuk sidebar/navigasi detail)
                ListOurServices = await _context.OurServices.Select(os => new OurServicesVM // Proyeksikan ke ViewModel
                {
                    IDOurServices = os.IDOurServices,
                    // Gunakan logika yang sama agar JudulService terisi
                    JudulService = CultureInfo.CurrentCulture.Name.StartsWith("id") ? os.JudulService_ID : os.JudulService_EN
                }).Take(6).ToListAsync()
            };
        }

        public async Task<bool> UpdateOurServicesAsync(OurServicesVM ourServicesVM)
        {
            var ourServices = await _context.OurServices.FindAsync(ourServicesVM.IDOurServices);
            if (ourServices == null) return false;

            // Update Kolom Multi-Bahasa
            ourServices.JudulService_ID = ourServicesVM.JudulService_ID;
            ourServices.JudulService_EN = ourServicesVM.JudulService_EN;
            ourServices.IsiService_ID = ourServicesVM.IsiService_ID;
            ourServices.IsiService_EN = ourServicesVM.IsiService_EN;

            // Pertahankan/Update Metadata
            ourServices.CreateAt = DateTime.Now;

            if (ourServicesVM.UploadGambar != null)
            {
                // Hapus gambar lama jika admin mengunggah file baru
                DeleteOldImage(ourServices.UrlGambar);
                // Simpan gambar baru ke disk
                ourServices.UrlGambar = await SaveImage(ourServicesVM.UploadGambar);
            }

            _context.OurServices.Update(ourServices);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Helper Methods ---

        private async Task<string> SaveImage(IFormFile file)
        {
            string folder = Path.Combine(_webHost.WebRootPath, "uploads", "ourservices");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(folder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Path disimpan dalam format URL untuk kemudahan akses di UI
            return "/uploads/ourservices/" + uniqueFileName;
        }

        private void DeleteOldImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            // Hapus awalan '/' jika ada agar Path.Combine bekerja dengan root web
            string relativePath = imageUrl.TrimStart('/');
            string fullPath = Path.Combine(_webHost.WebRootPath, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public async Task<OurServicesVM> GetPagedDataAsync(int pageIndex, int pageSize)
        {
            var totalRecords = await CountAsync();

            var data = await GetQuery()
                .OrderByDescending(x => x.IDOurServices)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new OurServicesVM
            {
                ListOurServices = data,
                CurrentPage = pageIndex,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };
        }

        public async Task<int> CountAsync()
        {
            return await _context.OurServices.CountAsync();
        }

        private IQueryable<OurServicesVM> GetQuery()
        {
            var ourServicesList = _context.OurServices;
            string currentLang = CultureInfo.CurrentCulture.Name;
            return ourServicesList.Select(os => new OurServicesVM
            {
                IDOurServices = os.IDOurServices,

                JudulService = currentLang.StartsWith("id") ? os.JudulService_ID : os.JudulService_EN,
                IsiService = currentLang.StartsWith("id") ? os.IsiService_ID : os.IsiService_EN,

                // Mapping Data ke VM untuk kebutuhan Form/Display
                JudulService_ID = os.JudulService_ID,
                JudulService_EN = os.JudulService_EN,
                IsiService_ID = os.IsiService_ID,
                IsiService_EN = os.IsiService_EN,
                UrlGambar = os.UrlGambar,
                CreateAt = os.CreateAt
            });
        }

    }
}
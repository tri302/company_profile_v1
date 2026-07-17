using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace company_profile
{
    public class PhotoGaleryRepo : IPhotoGaleryRepo
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public PhotoGaleryRepo(AppDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }
        public async Task<PhotoGalleryVM> CreatePhotoGaleryAsync(PhotoGalleryVM photoGaleryVM)
        {
            var model = new PhotoGallery
            {
                UrlGambar = photoGaleryVM.UrlGambar,
                CreateAt = DateTime.Now
            };
            if (photoGaleryVM.UploadGambar != null)
            {
                model.UrlGambar = await SaveImage(photoGaleryVM.UploadGambar);
            }
            _context.PhotoGalleries.Add(model);
            await _context.SaveChangesAsync();
            return model != null ? new PhotoGalleryVM
            {
                IDPhotoGallery = model.IDPhotoGallery,
                UrlGambar = model.UrlGambar,
                CreateAt = model.CreateAt
            } : null;
        }

        public async Task<bool> DeletePhotoGaleryAsync(int idPhotoGalery)
        {
            var photoGalery = await _context.PhotoGalleries.FindAsync(idPhotoGalery);
            if (photoGalery == null) return false;
                // Hapus gambar dari folder uploads/photo_galery
            DeleteOldImage(photoGalery.UrlGambar);            
            _context.PhotoGalleries.Remove(photoGalery);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PhotoGalleryVM>> GetAllPhotoGaleryAsync()
        {
           return await GetQuery().ToListAsync();
        }

        public async Task<PhotoGalleryVM> GetPhotoGaleryByIdAsync(int idPhotoGalery)
        {
            var photoGalery = await _context.PhotoGalleries.FindAsync(idPhotoGalery);
            return new PhotoGalleryVM
            {
                IDPhotoGallery = photoGalery.IDPhotoGallery,
                UrlGambar = photoGalery.UrlGambar,
                CreateAt = photoGalery.CreateAt
            };
        }

        public async Task<bool> UpdatePhotoGaleryAsync(PhotoGalleryVM photoGaleryVM)
        {
            var photoGalery = await _context.PhotoGalleries.FindAsync(photoGaleryVM.IDPhotoGallery);
            if (photoGalery == null) return false;

            // Hapus gambar lama jika ada upload baru
            if (photoGaleryVM.UploadGambar != null)
            {
                DeleteOldImage(photoGalery.UrlGambar);
                photoGalery.UrlGambar = await SaveImage(photoGaleryVM.UploadGambar);
            }

            _context.PhotoGalleries.Update(photoGalery);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Helper Methods ---
        // Simpan gambar ke folder uploads/photo_galery dan kembalikan URL-nya
        private async Task<string> SaveImage(IFormFile file)
        {
            // Gunakan Path.Combine agar otomatis menyesuaikan sistem operasi (Windows/Linux)
            string folder = Path.Combine(_webHost.WebRootPath, "uploads", "photo_galery");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Ambil ekstensi saja agar nama file di server rapi (contoh: 74a2...8b.jpg)
            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";

            string filePath = Path.Combine(folder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Kembalikan path dengan format URL (pake forward slash '/')
            return "/uploads/photo_galery/" + uniqueFileName;
        }

        private void DeleteOldImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            string fullPath = Path.Combine(_webHost.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        public async Task<PhotoGalleryVM> GetPagedDataAsync(int pageIndex, int pageSize)
        {
            var totalRecords = await CountAsync();
            var data = await GetQuery()
                .OrderByDescending(x => x.IDPhotoGallery)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PhotoGalleryVM
            {
                ListOurPhotoGallery = data,
                CurrentPage = pageIndex,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };

        }

        public async Task<int> CountAsync()
        {
            return await _context.PhotoGalleries.CountAsync();
        }

        private IQueryable<PhotoGalleryVM> GetQuery()
        {
            var photoGaleries = _context.PhotoGalleries;
            return photoGaleries.Select(pg => new PhotoGalleryVM
            {
                IDPhotoGallery = pg.IDPhotoGallery,
                UrlGambar = pg.UrlGambar,
                CreateAt = pg.CreateAt
            });
        }
    }
}


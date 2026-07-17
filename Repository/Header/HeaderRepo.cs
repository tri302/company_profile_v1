using Microsoft.EntityFrameworkCore;
using company_profile.Models;
using company_profile.ViewModels;

namespace company_profile
{
    public class HeaderRepo : IHeaderRepo
    {
        private readonly AppDbContext _context;

        public HeaderRepo(AppDbContext context)
        {
            _context = context;
        }

        // 1. Ambil Semua Data
        public async Task<IEnumerable<HeaderVM>> GetAllAsync()
        {
            return await _context.Header
                .Select(h => new HeaderVM
                {
                    ID = h.ID,
                    UrlFb = h.UrlFb,
                    UrlTwiter = h.UrlTwiter,
                    UrlLinkedIn = h.UrlLinkedIn,
                    UrlPinterest = h.UrlPinterest,
                    UrlYoutube = h.UrlYoutube,
                    UrlInstagram = h.UrlInstagram,
                    UrlTiktok = h.UrlTiktok,
                    EmailAddress = h.EmailAddress,
                    PhoneNumber = h.PhoneNumber,
                    IsShow = h.IsShow
                })
                .ToListAsync();
        }

        // 2. Ambil Data Berdasarkan ID
        public async Task<HeaderVM> GetByIdAsync(int id)
        {
            var header = await _context.Header.FindAsync(id);
            if (header == null) return null;

            return new HeaderVM
            {
                ID = header.ID,
                UrlFb = header.UrlFb,
                UrlTwiter = header.UrlTwiter,
                UrlLinkedIn = header.UrlLinkedIn,
                UrlPinterest = header.UrlPinterest,
                UrlYoutube = header.UrlYoutube,
                UrlInstagram = header.UrlInstagram,
                UrlTiktok = header.UrlTiktok,
                EmailAddress = header.EmailAddress,
                PhoneNumber = header.PhoneNumber,
                IsShow = header.IsShow
            };
        }

        // 3. Tambah Data Baru (Proteksi terhadap nilai null dengan ?? "")
        public async Task<bool> CreateAsync(HeaderVM model)
        {
            try
            {
                var entity = new Header
                {
                    // Menghindari error 'cannot be null' jika DB masih NOT NULL
                    UrlFb = model.UrlFb ?? "",
                    UrlTwiter = model.UrlTwiter ?? "",
                    UrlLinkedIn = model.UrlLinkedIn ?? "",
                    UrlPinterest = model.UrlPinterest ?? "",
                    UrlYoutube = model.UrlYoutube ?? "",
                    UrlInstagram = model.UrlInstagram ?? "",
                    UrlTiktok = model.UrlTiktok ?? "",
                    EmailAddress = model.EmailAddress,
                    PhoneNumber = model.PhoneNumber,
                    IsShow = model.IsShow
                };

                await _context.Header.AddAsync(entity);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                // Trik: Tulis ex.Message di sini saat debugging agar tahu error aslinya
                Console.WriteLine($"Error saat Create: {ex.Message}");
                return false;
            }
        }

        // 4. Ubah Data (Proteksi terhadap nilai null dengan ?? "")
        public async Task<bool> UpdateAsync(HeaderVM model)
        {
            try
            {
                var entity = await _context.Header.FindAsync(model.ID);
                if (entity == null) return false;

                entity.UrlFb = model.UrlFb ?? "";
                entity.UrlTwiter = model.UrlTwiter ?? "";
                entity.UrlLinkedIn = model.UrlLinkedIn ?? "";
                entity.UrlPinterest = model.UrlPinterest ?? "";
                entity.UrlYoutube = model.UrlYoutube ?? "";
                entity.UrlInstagram = model.UrlInstagram ?? "";
                entity.UrlTiktok = model.UrlTiktok ?? "";
                entity.EmailAddress = model.EmailAddress;
                entity.PhoneNumber = model.PhoneNumber;
                entity.IsShow = model.IsShow;

                _context.Header.Update(entity);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saat Update: {ex.Message}");
                return false;
            }
        }

        // Delete tetap sama karena tidak butuh mapping property
        public async Task<bool> DeleteAsync(int id)
        {
            var data = await _context.Header.FindAsync(id);
            if (data == null) return false;

            _context.Header.Remove(data);
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
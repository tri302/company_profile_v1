using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace company_profile
{
    public class KategoriRepo : IKategoriRepo
    {
        private readonly AppDbContext _context;        
        
        public KategoriRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<KategoriVM> CreateKategoriAsync(KategoriVM kategoriVM)
        {
            // Memetakan input multi-bahasa dari VM ke Entity fisik Database
            var model = new Kategori
            {
                NamaKategori_ID = kategoriVM.NamaKategori_ID,
                NamaKategori_EN = kategoriVM.NamaKategori_EN
            };

            _context.Kategoris.Add(model);
            await _context.SaveChangesAsync();

            // Mengembalikan VM lengkap dengan ID barunya yang digenerate oleh DB
            return new KategoriVM
            {
                IDKategori = model.IDKategori,
                NamaKategori_ID = model.NamaKategori_ID,
                NamaKategori_EN = model.NamaKategori_EN,
                NamaKategori = model.NamaKategori_ID // Jadikan versi Indonesia sebagai default display
            };
        }

        public async Task<bool> DeleteKategoriAsync(int idKategori)
        {
            var kategori = await _context.Kategoris.FindAsync(idKategori);
            if (kategori == null) return false;

            _context.Kategoris.Remove(kategori);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<KategoriVM>> GetAllKategoriAsync()
        {
            //Fungsi filter multi bahasa
            string currentLang = CultureInfo.CurrentCulture.Name;
            // Menggunakan Select Projection untuk kalkulasi TotalNews secara efisien di level query database
            return await _context.Kategoris
                .Select(k => new KategoriVM
                {
                    IDKategori = k.IDKategori,
                    NamaKategori = currentLang.StartsWith("id") ? k.NamaKategori_ID : k.NamaKategori_EN,
                    NamaKategori_ID = k.NamaKategori_ID,
                    NamaKategori_EN = k.NamaKategori_EN,                                       
                    TotalNews = k.News != null ? k.News.Count : 0
                })
                .ToListAsync();
        }

        public async Task<KategoriVM?> GetKategoriByIdAsync(int idKategori)
        {
            var k = await _context.Kategoris
                .Include(x => x.News)
                .FirstOrDefaultAsync(x => x.IDKategori == idKategori);

            if (k == null) return null;
            string currentLang = CultureInfo.CurrentCulture.Name;
            return new KategoriVM
            {
                IDKategori = k.IDKategori,
                NamaKategori = currentLang.StartsWith("id") ? k.NamaKategori_ID : k.NamaKategori_EN,
                NamaKategori_ID = k.NamaKategori_ID,
                NamaKategori_EN = k.NamaKategori_EN,                
                TotalNews = k.News?.Count ?? 0
            };
        }

        public async Task<bool> UpdateKategoriAsync(KategoriVM kategoriVM)
        {
            var kategori = await _context.Kategoris.FindAsync(kategoriVM.IDKategori);
            if (kategori == null) return false;

            // Memperbarui kolom multi-bahasa asli di database
            kategori.NamaKategori_ID = kategoriVM.NamaKategori_ID;
            kategori.NamaKategori_EN = kategoriVM.NamaKategori_EN;

            // State otomatis dilacak oleh EF Core, cukup eksekusi SaveChanges
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
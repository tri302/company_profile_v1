using System.Globalization;
using company_profile;
using company_profile.ViewModels.HomeVM;
using Microsoft.EntityFrameworkCore;

public class HomeRepo : IHomeRepo
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string _uploadFolder;

    public HomeRepo(AppDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        // Folder target: wwwroot/uploads/home
        _uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "home");
    }

    public async Task<IEnumerable<HomeVM>> GetAllAsync()
    {
        return await GetQuery().ToListAsync();
    }    
    public async Task<HomeVM?> GetByIdAsync(int id)
    {
        // DRY Principle: Memanfaatkan fungsi DetailsAsync yang memiliki logika mapping identik
        return await DetailsAsync(id);
    }

    public async Task<HomeVM?> DetailsAsync(int id)
    {
        var h = await _context.Home
            .Include(h => h.ProgressBars)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (h == null) return null;
        ////Fungsi filter multi bahasa
        string currentLang = CultureInfo.CurrentCulture.Name;
        return new HomeVM
        {
            Id = h.Id,
            JudulAboutStart = currentLang.StartsWith("id") ? h.JudulAboutStart_ID : h.JudulAboutStart_EN,
            IsiAboutStart = currentLang.StartsWith("id") ? h.IsiAboutStart_ID : h.IsiAboutStart_EN,            
            EmbededUrlVidio = h.EmbededUrlVidio,

            JudulAboutStart_ID = h.JudulAboutStart_ID,
            JudulAboutStart_EN = h.JudulAboutStart_EN,
            IsiAboutStart_ID = h.IsiAboutStart_ID,
            IsiAboutStart_EN = h.IsiAboutStart_EN,            

            //UrlGambar = h.UrlGambar,
            ProgressBars = h.ProgressBars.Select(p => new ProgressBarItemVM
            {
                Id = p.Id,
                Judul = currentLang.StartsWith("id") ? p.Judul_ID : p.Judul_EN,
                Judul_ID = p.Judul_ID,
                Judul_EN = p.Judul_EN,
                Value = p.Value
            }).ToList()
        };
    }

    public async Task<bool> InsertAsync(HomeVM model)
    {
        //string? uniqueFileName = null;

        //if (model.File != null)
        //{
        //    if (!ValidateImage(model.File)) return false;
        //    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
        //}
        var homeEntity = new Home
        {
            JudulAboutStart = model.JudulAboutStart,
            IsiAboutStart = model.IsiAboutStart,

            JudulAboutStart_ID = model.JudulAboutStart_ID,
            JudulAboutStart_EN = model.JudulAboutStart_EN,
            IsiAboutStart_ID = model.IsiAboutStart_ID,
            IsiAboutStart_EN = model.IsiAboutStart_EN,            

            EmbededUrlVidio = model.EmbededUrlVidio,
            //UrlGambar = uniqueFileName
        };

        if (model.ProgressBars != null && model.ProgressBars.Any())
        {
            foreach (var item in model.ProgressBars)
            {                
                homeEntity.ProgressBars.Add(new HomeProgressBar
                {                    
                    Judul_ID = item.Judul_ID,
                    Judul_EN = item.Judul_EN,
                    Value = item.Value
                });
            }
        }

        _context.Home.Add(homeEntity);
        var result = await _context.SaveChangesAsync() > 0;

        // Tulis berkas fisik ke disk HANYA JIKA data berhasil amandemen di database
        //if (result && model.File != null && uniqueFileName != null)
        //{
        //    await SaveImageAsync(model.File, uniqueFileName);
        //}

        return result;
    }

    public async Task<bool> UpdateAsync(HomeVM model)
    {
        var homeEntity = await _context.Home
            .Include(h => h.ProgressBars)
            .FirstOrDefaultAsync(h => h.Id == model.Id);

        if (homeEntity == null) return false;

        //string? oldFileName = homeEntity.UrlGambar;
        //string? newFileName = null;

        // 1. Persiapkan nama file baru jika ada file yang diunggah
        //if (model.File != null)
        //{
        //    if (!ValidateImage(model.File)) return false;
        //    newFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
        //}

        // 2. Update Field Teks Utama
        homeEntity.JudulAboutStart = model.JudulAboutStart;
        homeEntity.IsiAboutStart = model.IsiAboutStart;
        homeEntity.EmbededUrlVidio = model.EmbededUrlVidio;

        //Kolom multi bahasa
        homeEntity.JudulAboutStart_ID = model.JudulAboutStart_ID;
        homeEntity.JudulAboutStart_EN = model.JudulAboutStart_EN;
        homeEntity.IsiAboutStart_ID = model.IsiAboutStart_ID;
        homeEntity.IsiAboutStart_EN = model.IsiAboutStart_EN;        

        //if (newFileName != null)
        //{
        //    homeEntity.UrlGambar = newFileName;
        //}

        // 3. Logika Sinkronisasi Progress Bar (Hapus tracker lama, masukkan yang baru)
        if (homeEntity.ProgressBars.Any())
        {
            _context.HomeProgressBar.RemoveRange(homeEntity.ProgressBars);
        }

        if (model.ProgressBars != null && model.ProgressBars.Any())
        {
            foreach (var item in model.ProgressBars)
            {
                // Sesuaikan dengan nama properti di ViewModel Anda
                if (string.IsNullOrEmpty(item.Judul_ID) || string.IsNullOrEmpty(item.Judul_EN)) continue;

                homeEntity.ProgressBars.Add(new HomeProgressBar
                {                    
                    Value = item.Value,
                    Judul_ID = item.Judul_ID,
                    Judul_EN = item.Judul_EN
                });
            }
        }

        // 4. Simpan perubahan gambar baru ke storage SEBELUM SaveChanges (agar jika gagal I/O, DB tidak telanjur berubah)
        //if (model.File != null && newFileName != null)
        //{
        //    await SaveImageAsync(model.File, newFileName);
        //}

        await _context.SaveChangesAsync();

        // 5. Hapus file lama HANYA JIKA database sukses diperbarui (Mencegah hilangnya file jika DB error)
        //if (model.File != null)
        //{
        //    DeleteOldImage(oldFileName);
        //}
        // Kondisi khusus: Jika data teks berubah tapi tidak ganti gambar, SaveChanges tetap mengembalikan true
        //else if (model.File != null && newFileName != null)
        //{
            // Rollback file baru di storage jika SaveChanges ke database gagal
        //    DeleteOldImage(newFileName);
        //}

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var homeEntity = await _context.Home
            .Include(h => h.ProgressBars)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (homeEntity == null) return false;

        //string? currentFileName = homeEntity.UrlGambar;

        // 1. Hapus data anak secara eksplisit jika cascade delete tidak dikonfigurasi di Fluent API
        if (homeEntity.ProgressBars.Any())
        {
            _context.HomeProgressBar.RemoveRange(homeEntity.ProgressBars);
        }

        // 2. Hapus record induk dari konteks tracking
        _context.Home.Remove(homeEntity);

        var result = await _context.SaveChangesAsync() > 0;

        // 3. Hapus file fisik dari server HANYA JIKA database benar-benar sukses terhapus
        //if (result)
        //{
        //    DeleteOldImage(currentFileName);
        //}

        return result;
    }    

    public async Task<HomeVM> GetPagedDataAsync(int pageIndex, int pageSize)
    {
        var totalRecords = await CountAsync();

        var data = await GetQuery()
            .OrderByDescending(x => x.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new HomeVM
        {
            List = data,
            CurrentPage = pageIndex,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
        };
    }

    public async Task<int> CountAsync()
    {
        return await _context.Home.CountAsync();
    }

    private IQueryable<HomeVM> GetQuery()
    {
        //Fungsi filter multi bahasa
        string currentLang = CultureInfo.CurrentCulture.Name;
        // Projection menggunakan Select tidak memerlukan explicit .Include() karena EF Core otomatis memetakan navigasi
        return _context.Home
            .Select(h => new HomeVM
            {
                Id = h.Id,

                JudulAboutStart = currentLang.StartsWith("id") ? h.JudulAboutStart_ID : h.JudulAboutStart_EN,
                IsiAboutStart = currentLang.StartsWith("id") ? h.IsiAboutStart_ID : h.IsiAboutStart_EN,

                EmbededUrlVidio = h.EmbededUrlVidio,
                //UrlGambar = h.UrlGambar,

                JudulAboutStart_ID = h.JudulAboutStart_ID,
                JudulAboutStart_EN = h.JudulAboutStart_EN,
                IsiAboutStart_ID = h.IsiAboutStart_ID,
                IsiAboutStart_EN = h.IsiAboutStart_EN,

                ProgressBars = h.ProgressBars.Select(p => new ProgressBarItemVM
                {
                    Id = p.Id,
                    Judul = currentLang.StartsWith("id") ? p.Judul_ID : p.Judul_EN,
                    Judul_ID = p.Judul_ID,
                    Judul_EN = p.Judul_EN,
                    Value = p.Value
                }).ToList()
            });
    }
}
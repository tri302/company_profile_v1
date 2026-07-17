using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace company_profile
{
    public class NewsRepo : INewsRepo
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public NewsRepo(AppDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public async Task<NewsVM> CreateNewsAsync(NewsVM newsVM)
        {
            var news = new News
            {
                // Mapping Multi-Bahasa
                Judul_ID = newsVM.Judul_ID,
                Judul_EN = newsVM.Judul_EN,
                IsiContent_ID = newsVM.IsiContent_ID,
                IsiContent_EN = newsVM.IsiContent_EN,

                IDKategori = newsVM.IDKategori,
                CreateAt = DateTime.Now,
                IsSent = false
            };

            if (newsVM.UploadGambar != null)
            {
                news.UrlGambar = await SaveImage(newsVM.UploadGambar);
            }

            _context.News.Add(news);
            await _context.SaveChangesAsync();

            return newsVM;
        }

        public async Task<bool> DeleteNewsAsync(int idNews)
        {
            var news = await _context.News.FindAsync(idNews);
            if (news == null) return false;

            DeleteOldImage(news.UrlGambar);
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return true;
        }        

        public async Task<NewsVM> GetDetailNewsByIdAsync(int idNews)
        {
            var newsDetail = await _context.News
                .Include(n => n.Kategori)                
                .FirstOrDefaultAsync(x => x.IDNews == idNews);

            if (newsDetail == null) return null;

            var allCategories = await _context.Kategoris.ToListAsync();
            var recentPosts = await _context.News.Include(x=>x.Kategori).OrderByDescending(x => x.CreateAt).Take(5).ToListAsync();
            string currentLang = CultureInfo.CurrentCulture.Name;
            return new NewsVM
            {
                IDNews = newsDetail.IDNews,

                Judul = currentLang.StartsWith("id") ? newsDetail.Judul_ID : newsDetail.Judul_EN,
                IsiContent = currentLang.StartsWith("id") ? newsDetail.IsiContent_ID : newsDetail.IsiContent_EN,

                Judul_ID = newsDetail.Judul_ID,
                Judul_EN = newsDetail.Judul_EN,
                IsiContent_ID = newsDetail.IsiContent_ID,
                IsiContent_EN = newsDetail.IsiContent_EN,
                
                UrlGambar = newsDetail.UrlGambar,
                CreateAt = newsDetail.CreateAt,
                KategoriBerita = newsDetail.Kategori != null ? new KategoriVM
                {
                    IDKategori = newsDetail.Kategori.IDKategori,
                    // Isi dengan logika pemilihan bahasa yang sama
                    NamaKategori = currentLang.StartsWith("id")
                   ? newsDetail.Kategori.NamaKategori_ID
                   : newsDetail.Kategori.NamaKategori_EN
                } : null,
                ListKategoriBerita = allCategories.Select(c => new KategoriVM
                {
                    IDKategori = c.IDKategori,
                    NamaKategori_ID = c.NamaKategori_ID,
                    NamaKategori_EN = c.NamaKategori_EN,

                    // Logika pemilihan bahasa                    
                    NamaKategori = currentLang.StartsWith("id") ? c.NamaKategori_ID : c.NamaKategori_EN,
                }).ToList(),
                ListRecentPostsBerita = recentPosts.Select(c => new NewsVM
                {
                    IDNews = c.IDNews,
                    Judul = currentLang.StartsWith("id") ? c.Judul_ID : c.Judul_EN,
                    UrlGambar = c.UrlGambar, 
                    CreateAt = c.CreateAt    
                }).ToList()
            };
        }

        public async Task<NewsVM> GetNewsByIdAsync(int idNews)
        {
            var news = await _context.News
                .Include(n => n.Kategori)
                .FirstOrDefaultAsync(n => n.IDNews == idNews);

            if (news == null) return null;
            string currentLang = CultureInfo.CurrentCulture.Name;
            return new NewsVM
            {
                IDNews = news.IDNews,

                Judul = currentLang.StartsWith("id") ? news.Judul_ID : news.Judul_EN,
                IsiContent = currentLang.StartsWith("id") ? news.IsiContent_ID : news.IsiContent_EN,

                Judul_ID = news.Judul_ID,
                Judul_EN = news.Judul_EN,
                IsiContent_ID = news.IsiContent_ID,
                IsiContent_EN = news.IsiContent_EN,
                UrlGambar = news.UrlGambar,
                IDKategori = news.IDKategori,
                CreateAt = news.CreateAt
            };
        }

        public async Task<bool> UpdateNewsAsync(NewsVM newsVM)
        {
            var news = await _context.News.FindAsync(newsVM.IDNews);
            if (news == null) return false;

            // Update Data Multi-Bahasa
            news.Judul_ID = newsVM.Judul_ID;
            news.Judul_EN = newsVM.Judul_EN;
            news.IsiContent_ID = newsVM.IsiContent_ID;
            news.IsiContent_EN = newsVM.IsiContent_EN;
            news.IDKategori = newsVM.IDKategori;

            // Logika Gambar
            if (newsVM.UploadGambar != null)
            {
                DeleteOldImage(news.UrlGambar);
                news.UrlGambar = await SaveImage(newsVM.UploadGambar);
            }

            _context.News.Update(news);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Helper Methods ---

        private async Task<string> SaveImage(IFormFile file)
        {
            string folder = Path.Combine(_webHost.WebRootPath, "uploads", "news");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(folder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/uploads/news/" + uniqueFileName;
        }

        private void DeleteOldImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            string fullPath = Path.Combine(_webHost.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        public async Task<IEnumerable<NewsVM>> GetAllNewIndexAsync()
        {
           return await GetQuery().ToListAsync();
        }

        public async Task<NewsVM> GetPagedDataAsync(int pageIndex, int pageSize)
        {
            var totalRecords = await CountAsync();

            var data = await GetQuery()
                .OrderByDescending(x => x.IDNews)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new NewsVM
            {
                ListBerita = data,
                CurrentPage = pageIndex,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };
        }
       
        public async Task<int> CountAsync()
        {
            return await _context.News.CountAsync();
        }

        private IQueryable<NewsVM> GetQuery()
        {
            var newsList = _context.News
                .Include(n => n.Kategori)
                .OrderByDescending(n => n.CreateAt);
            // 1. Ambil bahasa aktif saat ini
            string currentLang = CultureInfo.CurrentCulture.Name;
            return newsList.Select(news => new NewsVM
            {
                IDNews = news.IDNews,
                Judul_ID = news.Judul_ID,
                Judul_EN = news.Judul_EN,

                Judul = currentLang.StartsWith("id") ? news.Judul_ID : news.Judul_EN,

                UrlGambar = news.UrlGambar,
                IDKategori = news.IDKategori,
                CreateAt = news.CreateAt,
                NamaKategori = news.Kategori != null ? news.Kategori.NamaKategori_ID : "Uncategorized"
            });
        }
             
        private IQueryable<News> GetQueryFE()
        {
            return _context.News.Include(n => n.Kategori);
        }

        private NewsVM MapToNewsVM(List<News> newsList, List<Kategori> kategoriList, int pageIndex = 1, int totalPages = 1)
        {
            string currentLang = CultureInfo.CurrentCulture.Name;

            var mappedNews = newsList.Select(n => new NewsVM
            {
                IDNews = n.IDNews,
                UrlGambar = n.UrlGambar,
                Judul = currentLang.StartsWith("id") ? n.Judul_ID : n.Judul_EN,
                IsiContent = currentLang.StartsWith("id") ? n.IsiContent_ID : n.IsiContent_EN,
                Judul_ID = n.Judul_ID,
                Judul_EN = n.Judul_EN,
                IsiContent_ID = n.IsiContent_ID,
                IsiContent_EN = n.IsiContent_EN,
                CreateAt = n.CreateAt,
                Kategori = n.Kategori != null ? new Kategori
                {
                    IDKategori = n.Kategori.IDKategori,
                    NamaKategori = currentLang.StartsWith("id") ? n.Kategori.NamaKategori_ID : n.Kategori.NamaKategori_EN
                } : null
            }).ToList();

            var mappedKategori = kategoriList.Select(k => new KategoriVM
            {
                IDKategori = k.IDKategori,
                NamaKategori_ID = k.NamaKategori_ID,
                NamaKategori_EN = k.NamaKategori_EN,
                NamaKategori = currentLang.StartsWith("id") ? k.NamaKategori_ID : k.NamaKategori_EN
            }).ToList();

            // Ambil 5 berita terbaru dari database untuk Recent Posts
            var recentNewsRaw = _context.News.OrderByDescending(n => n.CreateAt).Take(5).ToList();
            var mappedRecent = recentNewsRaw.Select(n => new NewsVM
            {
                IDNews = n.IDNews,
                UrlGambar = n.UrlGambar,
                Judul = currentLang.StartsWith("id") ? n.Judul_ID : n.Judul_EN,
                CreateAt = n.CreateAt
            }).ToList();

            return new NewsVM
            {
                ListBerita = mappedNews,
                ListKategoriBerita = mappedKategori,
                ListRecentPostsBerita = mappedRecent,
                CurrentPage = pageIndex,
                TotalPages = totalPages
            };
        }
        
        public NewsVM GetPagedDataFE(int pageIndex, int pageSize)
        {
            // Hitung total record
            var totalRecords = _context.News.Count();

            // Ambil data halaman aktif dari database secara sinkron
            var rawData = GetQueryFE()
                .OrderByDescending(x => x.CreateAt) // Urutkan berdasarkan tanggal buat terupdate
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var rawKategori = _context.Kategoris.OrderBy(k => k.IDKategori).ToList();

            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Lakukan mapping data
            return MapToNewsVM(rawData, rawKategori, pageIndex, totalPages);
        }
        
        public NewsVM GetAllNews()
        {
            var rawData = GetQueryFE().OrderByDescending(n => n.CreateAt).ToList();
            var rawKategori = _context.Kategoris.OrderBy(k => k.IDKategori).ToList();
            return MapToNewsVM(rawData, rawKategori);
        }


        // Helper untuk mendapatkan query dasar yang terfilter berdasarkan kategori
        private IQueryable<News> GetQueryFENewsByCategory(string namaKategori)
        {
            return _context.News
                .Include(n => n.Kategori)
                .Where(n => n.Kategori.NamaKategori_ID.ToLower() == namaKategori.ToLower() ||
                            n.Kategori.NamaKategori_EN.ToLower() == namaKategori.ToLower());
        }

        public async Task<NewsVM> GetPagedDataFENewsByCategory(string namaKategori, int pageIndex, int pageSize)
        {
            // 1. Hitung total record khusus untuk kategori ini saja
            var totalRecords = await GetQueryFENewsByCategory(namaKategori).CountAsync();

            // 2. Ambil data halaman aktif yang sudah di-page dan filter secara Async
            var rawData = await GetQueryFENewsByCategory(namaKategori)
                .OrderByDescending(x => x.CreateAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var rawKategori = await _context.Kategoris.ToListAsync();
            var recent = await _context.News.OrderByDescending(x => x.CreateAt).Take(5).ToListAsync();

            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // 3. Lakukan mapping data
            return MapToNewsVMNewsByCategory(rawData, rawKategori, recent, namaKategori, pageIndex, totalPages);
        }

        // Method mapping murni tanpa melakukan query database lagi di dalamnya
        private NewsVM MapToNewsVMNewsByCategory(List<News> newsList, List<Kategori> kategoriList, List<News> recentList, string namaKategori, int pageIndex, int totalPages)
        {
            string currentLang = CultureInfo.CurrentCulture.Name;

            var mappedNews = newsList.Select(n => new NewsVM
            {
                IDNews = n.IDNews,
                UrlGambar = n.UrlGambar,
                Judul = currentLang.StartsWith("id") ? n.Judul_ID : n.Judul_EN,
                IsiContent = currentLang.StartsWith("id") ? n.IsiContent_ID : n.IsiContent_EN,
                Judul_ID = n.Judul_ID,
                Judul_EN = n.Judul_EN,
                IsiContent_ID = n.IsiContent_ID,
                IsiContent_EN = n.IsiContent_EN,
                CreateAt = n.CreateAt,
                Kategori = n.Kategori != null ? new Kategori
                {
                    IDKategori = n.Kategori.IDKategori,
                    NamaKategori = currentLang.StartsWith("id") ? n.Kategori.NamaKategori_ID : n.Kategori.NamaKategori_EN
                } : null
            }).ToList();

            var mappedKategori = kategoriList.Select(k => new KategoriVM
            {
                IDKategori = k.IDKategori,
                NamaKategori_ID = k.NamaKategori_ID,
                NamaKategori_EN = k.NamaKategori_EN,
                NamaKategori = currentLang.StartsWith("id", StringComparison.OrdinalIgnoreCase) ? k.NamaKategori_ID : k.NamaKategori_EN
            }).ToList();

            return new NewsVM
            {
                NamaKategori = namaKategori,
                ListBerita = mappedNews,
                ListKategoriBerita = mappedKategori,
                ListRecentPostsBerita = recentList.Select(x => new NewsVM
                {
                    IDNews = x.IDNews,
                    Judul = currentLang.StartsWith("id", StringComparison.OrdinalIgnoreCase) ? x.Judul_ID : x.Judul_EN,
                    UrlGambar = x.UrlGambar,
                    CreateAt = x.CreateAt
                }).ToList(),

                // PENTING: Pastikan properti pagination ini ada di NewsVM Anda agar View bisa merender tombol info halaman
                CurrentPage = pageIndex,
                TotalPages = totalPages
            };
        }

    }
}
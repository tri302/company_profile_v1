using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace company_profile
{
    public class PortfolioRepo : IPortfolioRepo
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public PortfolioRepo(AppDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public async Task<PortfolioVM> CreatePortfolioAsync(PortfolioVM portfolioVM)
        {
            var portfolio = new Portfolio
            {
                UrlGambar = portfolioVM.UrlGambar,

                //Kolom multi bahasa
                Judul_ID = portfolioVM.Judul_ID,
                Judul_EN = portfolioVM.Judul_EN,
                IsiContentPortfolio_ID = portfolioVM.IsiContentPortfolio_ID,
                IsiContentPortfolio_EN = portfolioVM.IsiContentPortfolio_EN,
                ClientComment_ID = portfolioVM.ClientComment_ID,
                ClientComment_EN = portfolioVM.ClientComment_EN,

                CreateAt = DateTime.Now,
                ClientName = portfolioVM.ClientName,
                ClientCompanyName = portfolioVM.ClientCompanyName,
                ProjectStartDate = portfolioVM.ProjectStartDate,
                ProjectEndDate = portfolioVM.ProjectEndDate

                //Judul = portfolioVM.Judul,
                //IsiContentPortfolio = portfolioVM.IsiContentPortfolio,
                //ClientComment = portfolioVM.ClientComment
            };

            if (portfolioVM.UploadGambar != null)
            {
                portfolio.UrlGambar = await SaveImage(portfolioVM.UploadGambar);
            }

            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            if (portfolioVM.UploadFotoProject != null && portfolioVM.UploadFotoProject.Count > 0)
            {
                foreach (var file in portfolioVM.UploadFotoProject)
                {
                    var urlFoto = await SaveImage(file);
                    var portfolioGambar = new PortfolioGambar
                    {
                        IDPortfolio = portfolio.IDPortfolio,
                        UrlGambar = urlFoto
                    };
                    _context.PortfolioGambars.Add(portfolioGambar);
                }
                await _context.SaveChangesAsync();
            }

            return new PortfolioVM
            {
                IDPortfolio = portfolio.IDPortfolio
                //UrlGambar = portfolio.UrlGambar,
                //UrlFotoProject = portfolioVM.UrlFotoProject,
                //Judul = portfolio.Judul,
                //IsiContentPortfolio = portfolio.IsiContentPortfolio,
                //CreateAt = portfolio.CreateAt,
                //ClientName = portfolio.ClientName,
                //ClientCompanyName = portfolio.ClientCompanyName,
                //ProjectStartDate = portfolio.ProjectStartDate,
                //ProjectEndDate = portfolio.ProjectEndDate,
                //ClientComment = portfolio.ClientComment,
                // ✅ PERBAIKAN: Gunakan ToListAsync() agar tidak memblokir thread (blocking call)
                //RecentPortfolios = await _context.Portfolios.ToListAsync()
            };
        }

        public async Task<bool> DeletePortfolioAsync(int idPortfolio)
        {
            var portfolio = await _context.Portfolios
                .Include(p => p.PortfolioGambars)
                .FirstOrDefaultAsync(p => p.IDPortfolio == idPortfolio);

            if (portfolio == null) return false;

            DeleteOldImage(portfolio.UrlGambar);

            if (portfolio.PortfolioGambars != null)
            {
                foreach (var gambar in portfolio.PortfolioGambars)
                {
                    DeleteOldImage(gambar.UrlGambar);
                }
            }

            _context.Portfolios.Remove(portfolio);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<PortfolioVM>> GetAllPortfolioAsync()
        {
           return await GetQuery().ToListAsync();
        }

        public async Task<IEnumerable<PortfolioVM>> GetAllPortfolioAsyncTake()
        {
            return await GetQuery().Take(6).ToListAsync();
        }

        public async Task<PortfolioVM> GetPortfolioByIdAsync(int idPortfolio)
        {
            var portfolio = await _context.Portfolios
                .Include(p => p.PortfolioGambars)
                .FirstOrDefaultAsync(p => p.IDPortfolio == idPortfolio);

            if (portfolio == null) return null;

            var recentPortfolios = await _context.Portfolios
                .Where(p => p.IDPortfolio != idPortfolio)
                .OrderByDescending(p => p.CreateAt)
                .Take(5)
                .ToListAsync();
            string currentLang = CultureInfo.CurrentCulture.Name;
            return new PortfolioVM
            {
                IDPortfolio = portfolio.IDPortfolio,
                UrlGambar = portfolio.UrlGambar,

                Judul = currentLang.StartsWith("id") ? portfolio.Judul_ID : portfolio.Judul_EN,
                IsiContentPortfolio = currentLang.StartsWith("id") ? portfolio.IsiContentPortfolio_ID : portfolio.IsiContentPortfolio_EN,
                ClientComment = currentLang.StartsWith("id") ? portfolio.ClientComment_ID : portfolio.ClientComment_EN,

                //Kolom multi bahasa
                Judul_ID = portfolio.Judul_ID,
                Judul_EN = portfolio.Judul_EN,
                IsiContentPortfolio_ID = portfolio.IsiContentPortfolio_ID,
                IsiContentPortfolio_EN = portfolio.IsiContentPortfolio_EN,
                ClientComment_ID = portfolio.ClientComment_ID,
                ClientComment_EN = portfolio.ClientComment_EN,

                CreateAt = portfolio.CreateAt,
                ClientName = portfolio.ClientName,
                ClientCompanyName = portfolio.ClientCompanyName,
                ProjectStartDate = portfolio.ProjectStartDate,
                ProjectEndDate = portfolio.ProjectEndDate,
                ListPortfolio = portfolio.PortfolioGambars
                    .Select(x => new PortfolioGambarVM
                    {
                        IDPortfolio = x.IDPortfolio,
                        UrlGambar = x.UrlGambar
                    }).ToList(),
                RecentPortfolios = recentPortfolios
            };
        }

        public async Task<PortfolioVMEdit> GetPortfolioForEditAsync(int idPortfolio)
        {
            var portfolio = await _context.Portfolios
                .Include(p => p.PortfolioGambars)
                .FirstOrDefaultAsync(p => p.IDPortfolio == idPortfolio);

            if (portfolio == null) return null;

            return new PortfolioVMEdit
            {
                IDPortfolio = portfolio.IDPortfolio,
                UrlGambar = portfolio.UrlGambar,
                CreateAt = portfolio.CreateAt,
                ClientName = portfolio.ClientName,

                //Kolom multi bahasa
                Judul_ID = portfolio.Judul_ID,
                Judul_EN = portfolio.Judul_EN,
                IsiContentPortfolio_ID = portfolio.IsiContentPortfolio_ID,
                IsiContentPortfolio_EN = portfolio.IsiContentPortfolio_EN,
                ClientComment_ID = portfolio.ClientComment_ID,
                ClientComment_EN = portfolio.ClientComment_EN,

                ClientCompanyName = portfolio.ClientCompanyName,
                ProjectStartDate = portfolio.ProjectStartDate,
                ProjectEndDate = portfolio.ProjectEndDate,
                PortfolioGambars = portfolio.PortfolioGambars.ToList()
            };
        }

        public async Task<bool> UpdatePortfolioAsync(PortfolioVMEdit portfolioVM)
        {
            // ✅ PERBAIKAN: Sertakan .Include() saat query induk agar EF melacak entitas anaknya sekalian
            var portfolio = await _context.Portfolios
                .Include(p => p.PortfolioGambars)
                .FirstOrDefaultAsync(p => p.IDPortfolio == portfolioVM.IDPortfolio);

            if (portfolio == null) return false;

            // Proses Upload Gambar Utama Baru
            if (portfolioVM.UploadGambar != null)
            {
                DeleteOldImage(portfolio.UrlGambar);
                portfolio.UrlGambar = await SaveImage(portfolioVM.UploadGambar);
            }

            // Update field entitas utama
            portfolio.Judul = portfolioVM.Judul;
            portfolio.IsiContentPortfolio = portfolioVM.IsiContentPortfolio;
            portfolio.ClientName = portfolioVM.ClientName;
            portfolio.ClientCompanyName = portfolioVM.ClientCompanyName;
            portfolio.ProjectStartDate = portfolioVM.ProjectStartDate;
            portfolio.ProjectEndDate = portfolioVM.ProjectEndDate;
            portfolio.ClientComment = portfolioVM.ClientComment;

            //Kolom multi bahasa
            portfolio.Judul_ID = portfolio.Judul_ID;
            portfolio.Judul_EN = portfolioVM.Judul_EN;
            portfolio.IsiContentPortfolio_ID = portfolioVM.IsiContentPortfolio_ID;
            portfolio.IsiContentPortfolio_EN = portfolioVM.IsiContentPortfolio_EN;
            portfolio.ClientComment_ID = portfolioVM.ClientComment_ID;
            portfolio.ClientComment_EN = portfolioVM.ClientComment_EN;

            // Logika timpa gambar pendukung (Jika user memilih berkas galeri baru)
            if (portfolioVM.UploadFotoProject != null && portfolioVM.UploadFotoProject.Count > 0)
            {
                // 1. Hapus berkas fisik yang lama dari server lokal
                if (portfolio.PortfolioGambars != null && portfolio.PortfolioGambars.Any())
                {
                    // Gunakan ToList() untuk menyalin state agar tidak terjadi "Collection was modified" error saat loop
                    var oldImagesList = portfolio.PortfolioGambars.ToList();
                    foreach (var img in oldImagesList)
                    {
                        DeleteOldImage(img.UrlGambar);
                    }

                    // 2. Bersihkan record lama langsung dari koleksi relasi objek (Di-track otomatis oleh EF)
                    _context.PortfolioGambars.RemoveRange(oldImagesList);
                }

                // 3. Masukkan berkas gambar baru ke media penyimpanan dan database
                foreach (var file in portfolioVM.UploadFotoProject)
                {
                    var urlFoto = await SaveImage(file);
                    portfolio.PortfolioGambars.Add(new PortfolioGambar
                    {
                        IDPortfolio = portfolio.IDPortfolio,
                        UrlGambar = urlFoto
                    });
                }
            }

            // ✅ PERBAIKAN: Satukan eksekusi ke database. Cukup panggil SaveChangesAsync SEKALI 
            // di akhir agar proses berjalan secara Atomik (Jika satu gagal, semua rollback).
            _context.Portfolios.Update(portfolio);
            await _context.SaveChangesAsync();

            return true;
        }

        // --- Helper Methods ---
        private async Task<string> SaveImage(IFormFile file)
        {
            string folder = Path.Combine(_webHost.WebRootPath, "uploads", "portfolio");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(folder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/uploads/portfolio/" + uniqueFileName;
        }

        private void DeleteOldImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            string fullPath = Path.Combine(_webHost.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        public async Task<PortfolioVM> GetPagedDataAsync(int pageIndex, int pageSize)
        {
            var totalRecords = await CountAsync();
            var data = await GetQuery()
                .OrderByDescending(x => x.IDPortfolio)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PortfolioVM
            {
                List = data,
                CurrentPage = pageIndex,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };
        }

        public async Task<int> CountAsync()
        {
            return await _context.Portfolios.CountAsync();
        }

        private IQueryable<PortfolioVM> GetQuery()
        {
            var portfolios = _context.Portfolios;
            string currentLang = CultureInfo.CurrentCulture.Name;
            return portfolios.Select(p => new PortfolioVM
            {
                IDPortfolio = p.IDPortfolio,
                UrlGambar = p.UrlGambar,

                Judul = currentLang.StartsWith("id") ? p.Judul_ID : p.Judul_EN,
                IsiContentPortfolio = currentLang.StartsWith("id") ? p.IsiContentPortfolio_ID : p.IsiContentPortfolio_EN,
                ClientComment = currentLang.StartsWith("id") ? p.ClientComment_ID : p.ClientComment_EN,

                //Kolom multi bahasa
                Judul_ID = p.Judul_ID,
                Judul_EN = p.Judul_EN,
                IsiContentPortfolio_ID = p.IsiContentPortfolio_ID,
                IsiContentPortfolio_EN = p.IsiContentPortfolio_EN,
                ClientComment_ID = p.ClientComment_ID,
                ClientComment_EN = p.ClientComment_EN,

                CreateAt = p.CreateAt,
                ClientName = p.ClientName,
                ClientCompanyName = p.ClientCompanyName,
                ProjectStartDate = p.ProjectStartDate,
                ProjectEndDate = p.ProjectEndDate,

            });
        }

    }
}
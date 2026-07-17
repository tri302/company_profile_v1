using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace company_profile
{
    public class TeamRepo : ITeamRepo
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public TeamRepo(AppDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public async Task<TeamVM> CreateTeamAsync(TeamVM teamVM)
        {
            var team = new Team
            {
                Nama = teamVM.Nama,
                NoHP = teamVM.NoHP,
                Email = teamVM.Email,
                Website = teamVM.Website,
                UrlFb = teamVM.UrlFb,
                UrlLinkedIn = teamVM.UrlLinkedIn,
                UrlYoutube = teamVM.UrlYoutube,
                UrlTwiter = teamVM.UrlTwiter,
                Jabatan_ID = teamVM.Jabatan_ID,
                Jabatan_EN = teamVM.Jabatan_EN,
                Deskripsi_ID = teamVM.Deskripsi_ID,
                Deskripsi_EN = teamVM.Deskripsi_EN,
                UrlFoto = teamVM.UploadGambar != null ? await SaveImage(teamVM.UploadGambar) : null
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            
            teamVM.IDTeam = team.IDTeam;
            teamVM.UrlGambar = team.UrlFoto;
            return teamVM;
        }

        public async Task<bool> DeleteTeamAsync(int idTeam)
        {
            var team = await _context.Teams.FindAsync(idTeam);
            if (team == null) return false;

            DeleteOldImage(team.UrlFoto);
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TeamVM>> GetAllTeamAsync()
        {
            return await GetQuery().ToListAsync();
        }

        public async Task<TeamVM> GetTeamByIdAsync(int idTeam)
        {
            var team = await _context.Teams.FindAsync(idTeam);
            if (team == null) return null;
            string currentLang = CultureInfo.CurrentCulture.Name;
            return new TeamVM
            {
                IDTeam = team.IDTeam,
                Nama = team.Nama,
                NoHP = team.NoHP,
                Email = team.Email,
                Website = team.Website,
                UrlGambar = team.UrlFoto,
                UrlFb = team.UrlFb,
                UrlLinkedIn = team.UrlLinkedIn,
                UrlYoutube = team.UrlYoutube,
                UrlTwiter = team.UrlTwiter,

                Jabatan = currentLang.StartsWith("id") ? team.Jabatan_ID : team.Jabatan_EN,
                Deskripsi = currentLang.StartsWith("id") ? team.Deskripsi_ID : team.Deskripsi_EN,


                Jabatan_ID = team.Jabatan_ID,
                Jabatan_EN = team.Jabatan_EN,
                Deskripsi_ID = team.Deskripsi_ID,
                Deskripsi_EN = team.Deskripsi_EN
            };
        }

        public async Task<bool> UpdateTeamAsync(TeamVM teamVM)
        {
            var team = await _context.Teams.FindAsync(teamVM.IDTeam);
            if (team == null) return false;

            team.Nama = teamVM.Nama;
            team.NoHP = teamVM.NoHP;
            team.Email = teamVM.Email;
            team.Website = teamVM.Website;
            team.UrlFb = teamVM.UrlFb;
            team.UrlLinkedIn = teamVM.UrlLinkedIn;
            team.UrlYoutube = teamVM.UrlYoutube;
            team.UrlTwiter = teamVM.UrlTwiter;
            team.Jabatan_ID = teamVM.Jabatan_ID;
            team.Jabatan_EN = teamVM.Jabatan_EN;
            team.Deskripsi_ID = teamVM.Deskripsi_ID;
            team.Deskripsi_EN = teamVM.Deskripsi_EN;

            if (teamVM.UploadGambar != null)
            {
                DeleteOldImage(team.UrlFoto);
                team.UrlFoto = await SaveImage(teamVM.UploadGambar);
            }

            _context.Teams.Update(team);
            return await _context.SaveChangesAsync() > 0;
        }

        // --- Helper Methods ---

        private async Task<string> SaveImage(IFormFile file)
        {
            string folder = Path.Combine(_webHost.WebRootPath, "uploads", "team");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(folder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/uploads/team/" + uniqueFileName;
        }

        private void DeleteOldImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            string fullPath = Path.Combine(_webHost.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
        public async Task<TeamVM> GetPagedDataAsync(int pageIndex, int pageSize)
        {
            var totalRecords = await CountAsync();
            var data = await GetQuery()
                .OrderByDescending(x => x.IDTeam)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new TeamVM
            {
                List = data,
                CurrentPage = pageIndex,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };
        }

        public async Task<int> CountAsync()
        {
            return await _context.Teams.CountAsync();
        }

        private IQueryable<TeamVM> GetQuery()
        {
            var teams = _context.Teams;
            string currentLang = CultureInfo.CurrentCulture.Name;
            return teams.Select(team => new TeamVM
            {
                IDTeam = team.IDTeam,
                Nama = team.Nama,
                NoHP = team.NoHP,
                Email = team.Email,
                Website = team.Website,
                UrlGambar = team.UrlFoto,
                UrlFb = team.UrlFb,
                UrlLinkedIn = team.UrlLinkedIn,
                UrlYoutube = team.UrlYoutube,
                UrlTwiter = team.UrlTwiter,

                Jabatan = currentLang.StartsWith("id") ? team.Jabatan_ID : team.Jabatan_EN,
                Deskripsi = currentLang.StartsWith("id") ? team.Deskripsi_ID : team.Deskripsi_EN,

                Jabatan_ID = team.Jabatan_ID,
                Jabatan_EN = team.Jabatan_EN,
                Deskripsi_ID = team.Deskripsi_ID,
                Deskripsi_EN = team.Deskripsi_EN
            });
        }
    }
}
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace company_profile
{
    public class SliderFooterRepo : ISliderFooterRepo
    {
        private readonly AppDbContext _context;

        public SliderFooterRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SliderFooterHomeVM>> GetAllAsync()
        {
            // Jika nama entitas berbeda, lakukan mapping dari Entity ke VM di sini.
            return await GetQuery().ToListAsync();
        }

        public async Task<SliderFooterHomeVM?> GetByIdAsync(int id)
        {
            return await _context.SliderFooterHomes
                .AsNoTracking()
                .Select(s => new SliderFooterHomeVM
                {
                    Id = s.Id,
                    UrlImage = s.UrlImage
                })
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> InsertAsync(SliderFooterHomeVM model)
        {
            // Buat objek entitas database riil untuk diinsert
            var entity = new SliderFooterHome
            {
                UrlImage = model.UrlImage
            };

            await _context.SliderFooterHomes.AddAsync(entity);
            return await SaveAsync();
        }

        public async Task<bool> UpdateAsync(SliderFooterHomeVM model)
        {
            var entity = await _context.SliderFooterHomes.FindAsync(model.Id);
            if (entity == null) return false;

            // Update nama file gambarnya saja sesuai model terbaru
            entity.UrlImage = model.UrlImage;

            _context.SliderFooterHomes.Update(entity);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.SliderFooterHomes.FindAsync(id);
            if (entity == null) return false;

            _context.SliderFooterHomes.Remove(entity);
            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                // Anda bisa menaruh logging error di sini jika diperlukan
                return false;
            }
        }

        public async Task<IEnumerable<SliderFooterHomeVM>> GetPagedDataAsync(int pageIndex, int pageSize)
        {
            return await GetQuery()
                .OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.SliderFooterHomes.CountAsync();
        }

        private IQueryable<SliderFooterHomeVM> GetQuery()
        {
            //Fungsi filter multi bahasa
            string currentLang = CultureInfo.CurrentCulture.Name;
            return _context.SliderFooterHomes
                .AsNoTracking()
                .Select(s => new SliderFooterHomeVM
                {
                    Id = s.Id,
                    UrlImage = s.UrlImage
                });
        }
    }
}
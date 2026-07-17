using System.Globalization;
using company_profile.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace company_profile
{
    public class SpecialFeaturesRepo : ISpecialFeaturesRepo
    {
        private readonly AppDbContext _context;

        public SpecialFeaturesRepo(AppDbContext context)
        {
            _context = context;    
        }        

        public async Task<SpecialFeaturesVM> CreateSpecialFeaturesAsync(SpecialFeaturesVM vm)
        {
            var entity = new SpecialFeatures
            {
                JudulSpecialFeatures_ID = vm.JudulSpecialFeatures_ID,
                JudulSpecialFeatures_EN = vm.JudulSpecialFeatures_EN,
                IsiContentSpecialFeatures_ID = vm.IsiContentSpecialFeatures_ID,
                IsiContentSpecialFeatures_EN = vm.IsiContentSpecialFeatures_EN
            };

            _context.SpecialFeatures.Add(entity);
            await _context.SaveChangesAsync();
            
            vm.Id = entity.Id;
            return vm;
        }

        public async Task<bool> DeleteSpecialFeaturesAsync(int id)
        {
            var entity = await _context.SpecialFeatures.FindAsync(id);
            if (entity == null) return false;
            
            _context.SpecialFeatures.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SpecialFeaturesVM>> GetAllSpecialFeaturesAsync()
        {
            return await GetQuery().ToListAsync();
        }

        public async Task<SpecialFeaturesVM> GetSpecialFeaturesByIdAsync(int id)
        {
            var entity = await _context.SpecialFeatures.FindAsync(id);
            if (entity == null) return null;
            string currentLang = CultureInfo.CurrentCulture.Name;
            return new SpecialFeaturesVM
            {
                Id = entity.Id,

                JudulSpecialFeatures = currentLang.StartsWith("id") ? entity.JudulSpecialFeatures_ID : entity.JudulSpecialFeatures_EN,
                IsiContentSpecialFeatures = currentLang.StartsWith("id") ? entity.IsiContentSpecialFeatures_ID : entity.IsiContentSpecialFeatures_EN,

                JudulSpecialFeatures_ID = entity.JudulSpecialFeatures_ID,
                JudulSpecialFeatures_EN = entity.JudulSpecialFeatures_EN,
                IsiContentSpecialFeatures_ID = entity.IsiContentSpecialFeatures_ID,
                IsiContentSpecialFeatures_EN = entity.IsiContentSpecialFeatures_EN
            };
        }

        public async Task<bool> UpdateSpecialFeaturesAsync(SpecialFeaturesVM vm)
        {
            var entity = await _context.SpecialFeatures.FindAsync(vm.Id);
            if (entity == null) return false;
            
            entity.JudulSpecialFeatures_ID = vm.JudulSpecialFeatures_ID;
            entity.JudulSpecialFeatures_EN = vm.JudulSpecialFeatures_EN;
            entity.IsiContentSpecialFeatures_ID = vm.IsiContentSpecialFeatures_ID;
            entity.IsiContentSpecialFeatures_EN = vm.IsiContentSpecialFeatures_EN;
            
            _context.SpecialFeatures.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SpecialFeaturesVM>> GetPagedDataAsync(int pageIndex, int pageSize)
        {
            return await GetQuery()
                .OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.SpecialFeatures.CountAsync();
        }

        private IQueryable<SpecialFeaturesVM> GetQuery()
        {            
            //Fungsi filter multi bahasa
            string currentLang = CultureInfo.CurrentCulture.Name;
            return _context.SpecialFeatures.Select(p => new SpecialFeaturesVM
            {
                Id = p.Id,

                JudulSpecialFeatures = currentLang.StartsWith("id") ? p.JudulSpecialFeatures_ID : p.JudulSpecialFeatures_EN,
                IsiContentSpecialFeatures = currentLang.StartsWith("id") ? p.IsiContentSpecialFeatures_ID : p.IsiContentSpecialFeatures_EN,

                //Kolom multi bahasa
                JudulSpecialFeatures_ID = p.JudulSpecialFeatures_ID,
                JudulSpecialFeatures_EN = p.JudulSpecialFeatures_EN,
                IsiContentSpecialFeatures_ID = p.IsiContentSpecialFeatures_ID,
                IsiContentSpecialFeatures_EN = p.IsiContentSpecialFeatures_EN
            });
        }
    }
}
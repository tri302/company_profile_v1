using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace company_profile
{
    public class PricingRepo : IPricingRepo
    {
        private readonly AppDbContext _context;

        public PricingRepo(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<PricingVM> CreatePricingAsync(PricingVM pricingVM)
        {
            var pricing = new Pricing
            {
                // Mapping Properti Fisik Multi-Bahasa ke Entity
                JenisLayanan_ID = pricingVM.JenisLayanan_ID,
                JenisLayanan_EN = pricingVM.JenisLayanan_EN,
                DeskripsiJenisLayanan_ID = pricingVM.DeskripsiJenisLayanan_ID,
                DeskripsiJenisLayanan_EN = pricingVM.DeskripsiJenisLayanan_EN,
                
                Harga = pricingVM.Harga,
                CreateAt = DateTime.Now
            };

            _context.Pricings.Add(pricing);
            await _context.SaveChangesAsync();

            // Kembalikan ID hasil generasi DB ke properti VM asli
            pricingVM.IDPricing = pricing.IDPricing;
            pricingVM.CreateAt = pricing.CreateAt;
            return pricingVM;
        }

        public async Task<bool> DeletePricingAsync(int idPricing)
        {
            var pricing = await _context.Pricings.FindAsync(idPricing);
            if (pricing == null) return false;

            _context.Pricings.Remove(pricing);
            await _context.SaveChangesAsync();
            return true;
        }        

        public async Task<PricingVM> GetPricingByIdAsync(int idPricing)
        {
            var pricing = await _context.Pricings.FindAsync(idPricing);
            if (pricing == null) return null;
            string currentLang = CultureInfo.CurrentCulture.Name;
            return new PricingVM
            {
                IDPricing = pricing.IDPricing,

                JenisLayanan = currentLang.StartsWith("id") ? pricing.JenisLayanan_ID : pricing.JenisLayanan_EN,
                DeskripsiJenisLayanan = currentLang.StartsWith("id") ? pricing.DeskripsiJenisLayanan_ID : pricing.DeskripsiJenisLayanan_EN,

                // Mapping Data untuk Kebutuhan Binding ke Form Edit
                JenisLayanan_ID = pricing.JenisLayanan_ID,
                JenisLayanan_EN = pricing.JenisLayanan_EN,
                DeskripsiJenisLayanan_ID = pricing.DeskripsiJenisLayanan_ID,
                DeskripsiJenisLayanan_EN = pricing.DeskripsiJenisLayanan_EN,
                
                Harga = pricing.Harga.GetValueOrDefault(),
                CreateAt = pricing.CreateAt
            };
        }

        public async Task<bool> UpdatePricingAsync(PricingVM pricingVM)
        {
            var pricing = await _context.Pricings.FindAsync(pricingVM.IDPricing);
            if (pricing == null) return false;

            // Memperbarui Kolom Data Multi-Bahasa
            pricing.JenisLayanan_ID = pricingVM.JenisLayanan_ID;
            pricing.JenisLayanan_EN = pricingVM.JenisLayanan_EN;
            pricing.DeskripsiJenisLayanan_ID = pricingVM.DeskripsiJenisLayanan_ID;
            pricing.DeskripsiJenisLayanan_EN = pricingVM.DeskripsiJenisLayanan_EN;
            
            // Perbarui Data Global
            pricing.Harga = pricingVM.Harga;

            _context.Pricings.Update(pricing);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<PricingVM> GetPagedDataAsync(int pageIndex, int pageSize)
        {
            var totalRecords = await CountAsync();
            var data = await GetQuery()
                .OrderByDescending(x => x.IDPricing)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PricingVM
            {
                List = data,
                CurrentPage = pageIndex,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };
        }

        public async Task<int> CountAsync()
        {
            return await _context.Pricings.CountAsync();
        }

        private IQueryable<PricingVM> GetQuery()
        {
            var pricings = _context.Pricings;
            string currentLang = CultureInfo.CurrentCulture.Name;
            return pricings.Select(p => new PricingVM
            {
                IDPricing = p.IDPricing,

                JenisLayanan = currentLang.StartsWith("id") ? p.JenisLayanan_ID : p.JenisLayanan_EN,
                DeskripsiJenisLayanan = currentLang.StartsWith("id") ? p.DeskripsiJenisLayanan_ID : p.DeskripsiJenisLayanan_EN,

                // Distribusi Kolom Multi-Bahasa
                JenisLayanan_ID = p.JenisLayanan_ID,
                JenisLayanan_EN = p.JenisLayanan_EN,
                DeskripsiJenisLayanan_ID = p.DeskripsiJenisLayanan_ID,
                DeskripsiJenisLayanan_EN = p.DeskripsiJenisLayanan_EN,

                Harga = p.Harga.GetValueOrDefault(),
                CreateAt = p.CreateAt
            });
        }

        public async Task<IEnumerable<PricingVM>> GetAllPricingAsync()
        {
            return await GetQuery().ToListAsync();
        }
        public async Task<IEnumerable<PricingVM>> GetAllPricingAsyncTake()
        {
            return await GetQuery().Take(3).ToListAsync();
        }
    }
}
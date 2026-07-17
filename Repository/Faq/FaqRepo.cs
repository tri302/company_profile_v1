using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace company_profile
{
    public class FaqRepo : IFaqRepo
    {
        private readonly AppDbContext _context;

        public FaqRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Faqs.CountAsync();
        }

        public async Task<FaqVM> CreateFaqAsync(FaqVM faqVM)
        {
            var faq = new Faq
            {
                // Pemetaan Properti Bahasa Indonesia
                JudulFaq_ID = faqVM.JudulFaq_ID,
                IsiFaq_ID = faqVM.IsiFaq_ID,

                // Pemetaan Properti Bahasa Inggris
                JudulFaq_EN = faqVM.JudulFaq_EN,
                IsiFaq_EN = faqVM.IsiFaq_EN,

                // Properti Universal
                CreateAt = faqVM.CreateAt
            };

            _context.Faqs.Add(faq);
            await _context.SaveChangesAsync();

            // Kembalikan VM lengkap dengan ID baru dari database & fallback data
            faqVM.IDFaq = faq.IDFaq;
            faqVM.JudulFaq = faq.JudulFaq_ID;
            faqVM.IsiFaq = faq.IsiFaq_ID;

            return faqVM;
        }

        public async Task<bool> DeleteFaqAsync(int idFaq)
        {
            var faq = await _context.Faqs.FindAsync(idFaq);
            if (faq == null) return false;

            _context.Faqs.Remove(faq);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<FaqVM>> GetAllFaqAsync()
        {
           return await GetQuery().ToListAsync();
        }

        public async Task<IEnumerable<FaqVM>> GetAllFaqAsyncTake()
        {
            return await GetQuery().Take(4).ToListAsync();
        }

        public async Task<FaqVM> GetFaqByIdAsync(int idFaq)
        {
            var faq = await _context.Faqs.FindAsync(idFaq);
            string currentLang = CultureInfo.CurrentCulture.Name;
            if (faq == null) return null;

            return new FaqVM // Pastikan return sesuai ViewModel
            {
                IDFaq = faq.IDFaq,
                CreateAt = faq.CreateAt,

                JudulFaq = currentLang.StartsWith("id") ? faq.JudulFaq_ID : faq.JudulFaq_EN,
                IsiFaq = currentLang.StartsWith("id") ? faq.IsiFaq_ID : faq.IsiFaq_EN,

                // Pemetaan Multi-Bahasa ke ViewModel Form Edit
                JudulFaq_ID = faq.JudulFaq_ID,
                IsiFaq_ID = faq.IsiFaq_ID,
                JudulFaq_EN = faq.JudulFaq_EN,
                IsiFaq_EN = faq.IsiFaq_EN                
            };
        }

        public async Task<IEnumerable<FaqVM>> GetPagedDataAsync(int pageIndex, int pageSize)
        {
            return await GetQuery()
               .OrderByDescending(x => x.IDFaq)
               .Skip((pageIndex - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
        }

        public async Task<bool> UpdateFaqAsync(FaqVM faqVM)
        {
            var faq = await _context.Faqs.FindAsync(faqVM.IDFaq);
            if (faq == null) return false;

            // Update Data Versi Bahasa Indonesia
            faq.JudulFaq_ID = faqVM.JudulFaq_ID;
            faq.IsiFaq_ID = faqVM.IsiFaq_ID;

            // Update Data Versi Bahasa Inggris
            faq.JudulFaq_EN = faqVM.JudulFaq_EN;
            faq.IsiFaq_EN = faqVM.IsiFaq_EN;

            // Pertahankan nilai waktu pembuatan asli dari form/hidden field
            faq.CreateAt = faqVM.CreateAt;

            _context.Entry(faq).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        private IQueryable<FaqVM> GetQuery()
        {
            //Fungsi filter multi bahasa
            string currentLang = CultureInfo.CurrentCulture.Name;
            return _context.Faqs.Select(f => new FaqVM
            {
                IDFaq = f.IDFaq,
                CreateAt = f.CreateAt,

                JudulFaq = currentLang.StartsWith("id") ? f.JudulFaq_ID : f.JudulFaq_EN,
                IsiFaq = currentLang.StartsWith("id") ? f.IsiFaq_ID : f.IsiFaq_EN,

                // Pemetaan Multi-Bahasa ke ViewModel List (Untuk kebutuhan UI Baru)
                JudulFaq_ID = f.JudulFaq_ID,
                IsiFaq_ID = f.IsiFaq_ID,
                JudulFaq_EN = f.JudulFaq_EN,
                IsiFaq_EN = f.IsiFaq_EN,
            });
        }
    }
}
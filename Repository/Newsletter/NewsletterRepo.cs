using Microsoft.EntityFrameworkCore;
namespace company_profile
{
    public class NewsletterRepo : INewsletterRepo
    {
        private readonly AppDbContext _context;

        public NewsletterRepo(AppDbContext context)
        {
            _context = context;            
        }
        public async Task AddNewsletterAsync(NewsletterVM newsletter)
        {
            var newsletterEntity = new Newsletter
            {
                Email = newsletter.Email
            };
            _context.Newsletters.Add(newsletterEntity);
            await _context.SaveChangesAsync();
        }
    }
}
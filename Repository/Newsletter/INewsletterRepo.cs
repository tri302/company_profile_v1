namespace company_profile
{
    public interface INewsletterRepo
    {
        Task AddNewsletterAsync(NewsletterVM newsletter);
    }
}
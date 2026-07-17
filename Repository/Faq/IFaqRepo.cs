

namespace company_profile
{
    public interface IFaqRepo
    {
        //Get All Faq
        Task<IEnumerable<FaqVM>> GetAllFaqAsync();

        //Update Faq
        Task<bool> UpdateFaqAsync(FaqVM faqVM);

        //Create Faq    
        Task<FaqVM> CreateFaqAsync(FaqVM faqVM);

        //Delete Faq
        Task<bool> DeleteFaqAsync(int idFaq);

        //Get Faq by Id
        Task<FaqVM> GetFaqByIdAsync(int idFaq);

        Task<IEnumerable<FaqVM>> GetPagedDataAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
        Task<IEnumerable<FaqVM>> GetAllFaqAsyncTake();
    }
}


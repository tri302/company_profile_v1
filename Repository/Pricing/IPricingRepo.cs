namespace company_profile
{
    public interface IPricingRepo
    {
        //Get All Pricing
        Task<IEnumerable<PricingVM>> GetAllPricingAsync();

        //Update Pricing
        Task<bool> UpdatePricingAsync(PricingVM pricingVM);

        //Create Pricing    
        Task<PricingVM> CreatePricingAsync(PricingVM pricingVM);

        //Delete Pricing
        Task<bool> DeletePricingAsync(int idPricing);

        //Get Pricing by Id
        Task<PricingVM> GetPricingByIdAsync(int idPricing);

        Task<PricingVM> GetPagedDataAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
        Task<IEnumerable<PricingVM>> GetAllPricingAsyncTake();
    }
}




namespace company_profile
{
    public interface ISpecialFeaturesRepo
    {
        //Get All Pricing
        Task<IEnumerable<SpecialFeaturesVM>> GetAllSpecialFeaturesAsync();

        //Update Pricing
        Task<bool> UpdateSpecialFeaturesAsync(SpecialFeaturesVM specialFeaturesVM);

        //Create Pricing    
        Task<SpecialFeaturesVM> CreateSpecialFeaturesAsync(SpecialFeaturesVM specialFeaturesVM);

        //Delete Pricing
        Task<bool> DeleteSpecialFeaturesAsync(int id);

        //Get Pricing by Id
        Task<SpecialFeaturesVM> GetSpecialFeaturesByIdAsync(int id);

        Task<IEnumerable<SpecialFeaturesVM>> GetPagedDataAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
    }
}


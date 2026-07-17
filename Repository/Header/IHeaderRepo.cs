using company_profile.ViewModels; 

namespace company_profile
{
    public interface IHeaderRepo
    {
        Task<IEnumerable<HeaderVM>> GetAllAsync();
        Task<HeaderVM> GetByIdAsync(int id);
        Task<bool> CreateAsync(HeaderVM model);
        Task<bool> UpdateAsync(HeaderVM model);
        Task<bool> DeleteAsync(int id);
    }
}